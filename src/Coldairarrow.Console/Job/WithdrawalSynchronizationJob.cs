using Autofac;
using CCPP.Cryptocurrency.Common;
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

namespace Coldairarrow.Scheduler.Job
{
    public class WithdrawalSynchronizationJob : IJob
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
        private readonly ICoinTransactionOutBusiness _coinTransactionOutBusiness;
        #endregion

        public WithdrawalSynchronizationJob()
        {
            this._container = RmesAutoFacModule.GetContainer();
            this._logger = this._container.Resolve<ILogger<DepositSynchronizationJob>>();
            this._cacheDataBusiness = this._container.Resolve<ICacheDataBusiness>();
            this._cryptocurrencyBusiness = this._container.Resolve<ICryptocurrencyBusiness>();
            this._coinTransactionOutBusiness = this._container.Resolve<ICoinTransactionOutBusiness>();
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
                    await SyncConfirmTransaction();
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

        private async Task SyncConfirmTransaction()
        {
            var transationOuts = await this._coinTransactionOutBusiness.GetListAsync(e => e.Status == TransactionStatus.WaitConfirm);
            if(transationOuts.Any())
            {
                await HandlerConfirmTransactions(transationOuts);
            }
        }

        private async Task HandlerConfirmTransactions(List<CoinTransactionOut> transactions)
        { 
            var coinIds = transactions.Select(f => f.CoinID).Distinct().ToList();
            var coins = (await this._cacheDataBusiness.GetCoinsAsync()).Where(f => coinIds.Contains(f.Id) || coinIds.Contains(f.TokenCoinID));
            foreach (var transaction in transactions)
            {
                transaction.LastUpdateTime = DateTime.Now;
                var coin = coins.FirstOrDefault(f => f.Id == transaction.CoinID);
                if (coin == null) continue;
                try
                {
                    var cryptocurrencyProvider = await this._cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(coin);                   
                    var transactionInfo = cryptocurrencyProvider.GetTransaction(transaction.TXID); 
                    if (transactionInfo == null || !string.IsNullOrEmpty(transactionInfo.Error) || transactionInfo.Result == null) 
                        continue; 
                    var coinTransaction = transactionInfo.Result;
                    if (cryptocurrencyProvider.HasNotBlockCount)
                    {
                        if (coinTransaction.Valid)
                        {
                            await UpdateTransaction(transaction);
                        }
                    }
                    else
                    {
                        if (coinTransaction.Confirmations >= coin.MinConfirms)
                        {
                            //ETH 和ETC的合约获取代币交易需要判断交易是否成功
                            if (cryptocurrencyProvider.HasTransactionReceipt)
                            {
                                var transactionReceipResult = cryptocurrencyProvider.GetTransactionReceipt(coinTransaction.TXId);
                                if (transactionReceipResult.Result != null && transactionReceipResult.Result.IsSuccess)
                                {
                                    await UpdateTransaction(transaction);
                                }
                            }
                            else
                            {
                                await UpdateTransaction(transaction);
                            } 
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorMsg = $"转出交易错误,交易ID{transaction.ID}";
                    this._logger.LogError(ex, errorMsg);
                    (new Exception(errorMsg)).ToExceptionless().Submit();
                    ex.ToExceptionless().Submit();
                } 
            } 
        }

        private async Task UpdateTransaction(CoinTransactionOut transaction)
        {
            transaction.LastUpdateTime = DateTime.Now;
            transaction.Status = TransactionStatus.TransactionOut;
            await this._coinTransactionOutBusiness.UpdateDataAsync(transaction);
        }
    }
}
