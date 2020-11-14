using Autofac;
using Coldairarrow.Business.Foundation;
using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.Enum;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Scheduler.Core;
using Exceptionless;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Coldairarrow.Util;

namespace Coldairarrow.Scheduler.Job
{
    public class ContractTransactionJob : IJob
    {
        #region VARIABLE 
        private static bool _isRun = false;
        /// <summary>
        /// 运行状态
        /// </summary>
        private static int _runStatus = 0;
        #endregion

        #region DI
        private readonly ILifetimeScope _container;
        private readonly ILogger<DepositSynchronizationJob> _logger;
        private readonly ICacheDataBusiness _cacheDataBusiness;
        private readonly ICryptocurrencyBusiness _cryptocurrencyBusiness;
        private readonly ICoinTransactionInBusiness _coinTransactionInBusiness;
        private readonly ICoinContranctTransactionInBusiness _coinContranctTransactionInBusiness;
        private readonly IWalletBusiness _walletBusiness;
        private readonly ICoinConfigBusiness _coinConfigBusiness;
        #endregion

        public ContractTransactionJob()
        {
            this._container = RmesAutoFacModule.GetContainer();
            this._logger = this._container.Resolve<ILogger<DepositSynchronizationJob>>();
            this._cacheDataBusiness = this._container.Resolve<ICacheDataBusiness>();
            this._coinTransactionInBusiness = this._container.Resolve<ICoinTransactionInBusiness>();
            this._cryptocurrencyBusiness = this._container.Resolve<ICryptocurrencyBusiness>();
            this._coinContranctTransactionInBusiness = this._container.Resolve<ICoinContranctTransactionInBusiness>();
            this._walletBusiness = this._container.Resolve<IWalletBusiness>();
            this._coinConfigBusiness = this._container.Resolve<ICoinConfigBusiness>();
        }

        public Task Execute(IJobExecutionContext context)
        {
            if (Interlocked.CompareExchange(ref _runStatus, 1, 0) != 0)
            {
                return Task.FromResult<object>(null);
            }
            Thread.MemoryBarrier();
            if (_isRun)
            {
                return Task.FromResult<object>(null);
            }
            _isRun = true;
            Thread.MemoryBarrier();
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    await SyncContractTransaction();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    ex.ToExceptionless().Submit();
                }
                finally
                {
                    Interlocked.CompareExchange(ref _runStatus, 0, 1);
                    _isRun = false;
                    Thread.MemoryBarrier();
                }
            });
        }

        private async Task SyncContractTransaction()
        {
            var contranctTransactions = await this._coinContranctTransactionInBusiness.GetListAsync(e => e.Status == CoinContranctTransactionStatus.WaitSync);
            if (contranctTransactions.Any())
            {
                await HandleContractTransaction(contranctTransactions);
            }
        }

        private async Task HandleContractTransaction(List<CoinContranctTransactionIn> contranctTransactions)
        {
            var coins = (await this._cacheDataBusiness.GetCoinsAsync()).Where(e => e.IsSupportWallet || !string.IsNullOrEmpty(e.TokenCoinAddress));
            var contractAddressList = contranctTransactions.Select(f => f.ContractAddress).Distinct().ToList();
            var toAddressList = contranctTransactions.Select(f => f.ToAddress).Distinct().ToList();
            var coinIDs = contranctTransactions.Select(f => f.CoinID).Distinct().ToList();
            var wallets = await this._walletBusiness.GetListAsync(e => coinIDs.Contains(e.CoinID) && toAddressList.Contains(e.Address));
            var txIDs = contranctTransactions.Select(f => f.TXID).Distinct().ToList();
            //已经添加的转入记录
            var existedTransactionIns = await this._coinTransactionInBusiness.GetListAsync(e => coinIDs.Contains(e.CoinID) && txIDs.Contains(e.TXID));
            //待审核的交易锲约 
            foreach (var item in contranctTransactions)
            {
                try
                {
                    item.LastUpdateTime = DateTime.Now;
                    if (existedTransactionIns?.Any() ?? false && existedTransactionIns.Exists(f => f.TXID == item.TXID && f.CoinID == item.CoinID))
                    {
                        item.Status = CoinContranctTransactionStatus.Synchronized;
                        await this._coinContranctTransactionInBusiness.UpdateDataAsync(item);
                        continue;
                    }

                    var tokenCoin = coins.Where(e => e.TokenCoinID == item.CoinID && !string.IsNullOrEmpty(e.TokenCoinAddress) && item.ContractAddress.Equals(e.TokenCoinAddress, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (tokenCoin == null)
                    {
                        item.Status = CoinContranctTransactionStatus.Invalid;
                        await this._coinContranctTransactionInBusiness.UpdateDataAsync(item);
                        continue;
                    }
                    var cryptocurrencyProvider = await this._cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(tokenCoin);
                    //ETH 和ETC的合约获取代币交易需要判断交易是否成功
                    if (tokenCoin.ProviderType == ProviderType.BTC || tokenCoin.ProviderType == ProviderType.ETH || tokenCoin.ProviderType == ProviderType.ETHTokenCoin)
                    {
                        var transactionReceipt = cryptocurrencyProvider.GetTransactionReceipt(item.TXID);
                        if (transactionReceipt == null || transactionReceipt.Result == null) continue;
                        //交易失败
                        if (!transactionReceipt.Result.IsSuccess)
                        {
                            item.Status = CoinContranctTransactionStatus.Invalid;
                            await this._coinContranctTransactionInBusiness.UpdateDataAsync(item);
                            string errorMsg = $"合约交易同步无效交易ID:{item.ID},TXID:{item.TXID}";
                            this._logger.LogError(errorMsg);
                            (new Exception(errorMsg)).ToExceptionless().Submit();
                            continue;
                        }
                    }
                    var result = cryptocurrencyProvider.GetContractTransaction(item.TXID);
                    if (result == null) continue;
                    var coinInTransaction = result.Result;
                    if (coinInTransaction != null)
                    {
                        var category = Enum.Parse<TransactionCategory>(coinInTransaction.Category);
                        var customerWallet = wallets.Find(e => e.CoinID == item.CoinID && e.Address == item.ToAddress);
                        if (customerWallet == null)
                        {
                            var customerWalletException = new Exception($"找不到用户地址信息:交易ID:{item.TXID},虚拟币地址{item.ToAddress},虚拟币:{item.CoinID}");
                            customerWalletException.ToExceptionless().Submit();
                        }
                        else
                        {
                            var transaction = new CoinTransactionIn
                            {
                                ID = Guid.NewGuid().GuidTo16String(),
                                Account = coinInTransaction.Account,
                                Address = coinInTransaction.Address,
                                FromAddress = coinInTransaction.FromAddress,
                                Amount = coinInTransaction.Amount,
                                Confirmations = (int)coinInTransaction.Confirmations,
                                BlockIndex = (int)coinInTransaction.Blockindex,
                                BlockTime = coinInTransaction.BlockTime.ToDateTime_From_JsGetTime(),
                                CoinID = tokenCoin.Id,
                                CreateTime = DateTime.Now,
                                UserID = customerWallet.UserID,
                                TXID = coinInTransaction.TXId,
                                Timereceived = coinInTransaction.TimeReceived.ToDateTime_From_JsGetTime(),
                                Status = TransactionStatus.WaitConfirm,
                                LastUpdateTime = DateTime.Now,
                            };
                            if (!existedTransactionIns.Any(f => f.TXID == transaction.TXID && f.CoinID == transaction.CoinID))
                            {
                                var rechargeConfig = await this._coinConfigBusiness.MinAmountAsync(transaction.UserID, tokenCoin.Code);
                                if (rechargeConfig != null)
                                {
                                    if (transaction.Amount < rechargeConfig.MinPaymentAmount)
                                    {
                                        transaction.Status = TransactionStatus.NotArriveLessInMinAmount;
                                    }
                                }
                                await this._coinTransactionInBusiness.AddDataAsync(transaction);
                                existedTransactionIns.Add(transaction);
                            }
                            item.Status = CoinContranctTransactionStatus.Synchronized;
                            await this._coinContranctTransactionInBusiness.UpdateDataAsync(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, $"合约交易同步错误! {ex.Message}");
                    ex.ToExceptionless().Submit();
                }
            }
        }
    }
}