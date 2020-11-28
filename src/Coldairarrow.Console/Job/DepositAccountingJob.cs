using Autofac;
using CCPP.Cryptocurrency.Common;
using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Business.Foundation;
using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.DTO;
using Coldairarrow.Entity.Enum;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Scheduler.Core;
using Coldairarrow.Util;
using Exceptionless;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coldairarrow.Scheduler.Job
{
    public class DepositAccountingJob : IJob
    {
        #region VARIABLE 
        private static bool _isRun = false;
        /// <summary>
        /// 运行状态
        /// </summary>
        private static int _runStatus = 0;
        #endregion

        #region DI
        private readonly TransactionContainer _transactionContainer;
        private readonly ILifetimeScope _container;
        private readonly ILogger<DepositAccountingJob> _logger;
        private readonly ICacheDataBusiness _cacheDataBusiness; 
        private readonly ICryptocurrencyBusiness _cryptocurrencyBusiness; 
        private readonly IWalletBusiness _walletBusiness;
        private readonly ICoinTransactionInBusiness _coinTransactionInBusiness;
        private readonly ICoinConfigBusiness _coinConfigBusiness;
        private readonly IUserAssetsWasteBookBusiness _userAssetsWasteBookBusiness;
        private readonly IBase_UserBusiness _base_UserBusiness;
        private readonly IUserAssetsBusiness _userAssetsBusiness;
        #endregion

        public DepositAccountingJob()
        {
            this._container = RmesAutoFacModule.GetContainer();
            this._logger = this._container.Resolve<ILogger<DepositAccountingJob>>();
            this._cacheDataBusiness = this._container.Resolve<ICacheDataBusiness>(); 
            this._cryptocurrencyBusiness = this._container.Resolve<ICryptocurrencyBusiness>(); 
            this._walletBusiness = this._container.Resolve<IWalletBusiness>();
            this._coinTransactionInBusiness = this._container.Resolve<ICoinTransactionInBusiness>();
            this._coinConfigBusiness = this._container.Resolve<ICoinConfigBusiness>();
            this._userAssetsWasteBookBusiness = this._container.Resolve<IUserAssetsWasteBookBusiness>();
            this._base_UserBusiness = this._container.Resolve<IBase_UserBusiness>();
            this._userAssetsBusiness = this._container.Resolve<IUserAssetsBusiness>();
            this._transactionContainer = this._container.Resolve<TransactionContainer>();
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
                    await HandleTransactionInAccounting();
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

        private async Task HandleTransactionInAccounting()
        {
            var coins = (await this._cacheDataBusiness.GetCoinsAsync()).Where(e => e.IsSupportWallet && e.IsAvailable);
            var transactionIns = await GetPendingTransactions(coins);
            if (!transactionIns.Any()) return;
            transactionIns = await VerifyLegality(transactionIns);
            if (!transactionIns.Any()) return; 
            #region 余额校验,是否有变动 
            await HandleETHToken(coins, transactionIns); 
            await HandleETH(coins, transactionIns);
            #endregion  
            await VerifyBlockChainTxId(coins, transactionIns);
            try
            {
                await this._transactionContainer.BeginTransactionAsync(System.Data.IsolationLevel.ReadUncommitted);
                await Accounting(coins, transactionIns);
                this._transactionContainer.CommitTransaction();
            }
            catch(Exception e)
            {
                this._transactionContainer.RollbackTransaction();
                this._logger.LogError(e, e.Message);
            } 
        }
        /// <summary>
        /// 合法性检查
        /// </summary>
        /// <param name="transactionIns"></param>
        /// <returns></returns>
        private async Task<List<CoinTransactionIn>> VerifyLegality(List<CoinTransactionIn> transactionIns)
        {
            var transactionInIds = transactionIns.Select(f => f.Id).ToList();
            var transactionInChecks = await this._userAssetsWasteBookBusiness.GetListAsync(e => transactionInIds.Contains(e.RelateBusinessID));
            if (transactionInChecks.Any())
            {
                List<CoinTransactionIn> needDataList = new List<CoinTransactionIn>();
                foreach (var item in transactionIns)
                {
                    var transactionInCheck = transactionInChecks.FirstOrDefault(f => f.RelateBusinessID == item.Id);
                    if (transactionInCheck != null)
                    {
                        item.Status = TransactionStatus.ArrivedAccount;
                        item.LastUpdateTime = DateTime.Now;
                        item.ArrivalAmount = transactionInCheck.ChangeAmount;
                        await this._coinTransactionInBusiness.UpdateDataAsync(item);
                        continue;
                    }
                    needDataList.Add(item);
                }
               return needDataList;
            } 
            return transactionIns;
        }

        private async Task<List<CoinTransactionIn>> GetPendingTransactions(IEnumerable<Entity.Foundation.Coin> coins)
        {
            List<CoinConfirmatioItem> coinConfirmationItemList = new List<CoinConfirmatioItem>();
            foreach (var item in coins)
            {
                CoinConfirmatioItem coinConfirmatioItem = new CoinConfirmatioItem
                {
                    CoinID = item.Id,
                    MinConfirmation = item.MinConfirms
                };
                coinConfirmationItemList.Add(coinConfirmatioItem);
            }
            var transactionIns = await this._coinTransactionInBusiness.GetListAsync(e => coinConfirmationItemList.Exists(f => f.CoinID == e.CoinID && e.Confirmations >= f.MinConfirmation) && e.Status == TransactionStatus.WaitConfirm && e.MoveStatus == MoveStatus.WaitMoveToSys);
            return transactionIns;
        }

        private async Task Accounting(IEnumerable<Entity.Foundation.Coin> coins, List<CoinTransactionIn> transactionIns)
        {
            if (transactionIns.Any())
            {
                var needCoinIds = transactionIns.Select(f => f.CoinID).Distinct().ToList();
                var allUserIds = transactionIns.Select(f => f.UserID).Distinct().ToList();
                var txIDList = transactionIns.Select(f => f.TXID).Distinct();
                //系统移动到用户地址上的矿工费
                var moveTokenRecordList = await this._coinTransactionInBusiness.GetListAsync(e => txIDList.Contains(e.SysToUserTXID));
                foreach (var item in transactionIns)
                {
                    if (moveTokenRecordList != null && moveTokenRecordList.Any())
                    {
                        if (moveTokenRecordList.Any(f => f.SysToUserTXID == item.TXID))
                        {
                            item.Status = TransactionStatus.Exception;
                            item.LastUpdateTime = DateTime.Now;
                            await this._coinTransactionInBusiness.UpdateDataAsync(item);
                            continue;
                        }
                    }
                    //手续费
                    decimal fee = 0;
                    //实际到账
                    decimal receiveQuantity = item.Amount;
                    var coin = coins.First(e => e.Id == item.CoinID);
                    var coinConfig = await this._coinConfigBusiness.GetEntityAsync(item.UserID, coin.Code); 
                    var minimumDeposit = this._coinConfigBusiness.MinAmountAsync(item.UserID, coin.Code);
                    if (coinConfig != null)
                    {
                        // 固定手续费
                        if (coinConfig.CoinInHandlingFeeModeType == FeeMode.Fixed)
                        {
                            fee = coinConfig.CoinInHandlingFee ?? 0;
                        }
                        else
                        {
                            fee = item.Amount * coinConfig.CoinInHandlingFeeRate ?? 0;
                            fee = Math.Round(fee, GlobalData.QuantityPercision);
                            fee = Math.Max(fee, coinConfig.CoinInHandlingMinFee ?? 0);
                        }
                        receiveQuantity = Math.Round(receiveQuantity - fee, GlobalData.QuantityPercision);
                        if (Math.Round(item.Amount - coinConfig.CoinInMinAmount, GlobalData.QuantityPercision) < 0 || receiveQuantity < 0)
                        {
                            item.LastUpdateTime = DateTime.Now;
                            item.Status = TransactionStatus.NotArriveLessInMinAmount;
                            await this._coinTransactionInBusiness.UpdateDataAsync(item);
                            continue;
                        }
                    }
                    else
                    {
                        var configNullException = new Exception($"没有获取到提现充值设置:用户ID:{item.UserID},虚拟币Code:{coin.Code},交易ID:{item.TXID}");
                        this._logger.LogError(configNullException, configNullException.Message);
                        configNullException.ToExceptionless().Submit();
                        continue;
                    }
                    var assetsChangeItem = new AssetsChangeItemDTO()
                    {
                        CoinID = coin.Id,
                        AssetsWasteBookType = AssetsWasteBookType.CoinIn,
                        ChangeAvailableAmount = receiveQuantity,
                        FeeAmount = fee,
                        RelateID = item.Id,
                        UserID = item.UserID,
                        Remark = "转入虚拟币,手续费:" + fee
                    };
                    var assetsResult = await this._userAssetsBusiness.UpdateAssets(assetsChangeItem);
                    if (assetsResult.Success)
                    {
                        item.Status = TransactionStatus.ArrivedAccount;
                        item.LastUpdateTime = DateTime.Now;
                        item.CoinInHandlingFee = fee;
                        item.ArrivalAmount = receiveQuantity;
                        await this._coinTransactionInBusiness.UpdateDataAsync(item);
                    }
                }
            }
        }

        private async Task VerifyBlockChainTxId(IEnumerable<Entity.Foundation.Coin> coins, List<CoinTransactionIn> transactionIns)
        {
            if (transactionIns.Any())
            {
                var coinTXGroups = transactionIns.GroupBy(f => f.CoinID).ToList();
                foreach (var item in coinTXGroups)
                {
                    var coin = coins.FirstOrDefault(f => f.Id == item.Key);
                    var coinProvider = await this._cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(coin);
                    if (coinProvider.HasTransactionReceipt)
                    {
                        foreach (var tx in item)
                        {
                            try
                            {
                                var receiptResult = coinProvider.GetTransactionReceipt(tx.TXID);
                                if (receiptResult == null || !string.IsNullOrEmpty(receiptResult.Error) || receiptResult.Result == null || !receiptResult.Result.IsSuccess)
                                {
                                    transactionIns.Remove(tx);
                                }
                            }
                            catch (Exception ex)
                            {
                                this._logger.LogError(ex, ex.Message);
                                ex.ToExceptionless().Submit();
                                transactionIns.Remove(tx);
                            }
                        }
                    }
                }
            }
        }

        private async Task HandleETH(IEnumerable<Entity.Foundation.Coin> coins, List<CoinTransactionIn> transactionIns)
        {
            var ethCoinIDList = coins.Where(f => f.ProviderType == ProviderType.ETH).Select(f => f.Id).Distinct().ToList();
            var ethTXList = transactionIns.Where(f => ethCoinIDList.Contains(f.CoinID)).ToList();
            if (ethTXList.Any())
            {
                var ethGroupTXList = ethTXList.GroupBy(f => new { f.CoinID, f.Address }).ToList();
                var syncUserAddresList = ethTXList.Select(f => f.Address).Distinct().ToList();
                var waitMoveETHList = await this._coinTransactionInBusiness.GetListAsync(e => ethCoinIDList.Contains(e.CoinID) && e.MoveStatus != MoveStatus.Finish && e.MoveStatus != MoveStatus.Invalid && syncUserAddresList.Contains(e.Address));
                var startTXDate = ethTXList.Min(f => f.CreateTime);
                var useETHBalanceTokens = await this._coinTransactionInBusiness.GetListAsync(e => ethCoinIDList.Contains(e.CoinID) && syncUserAddresList.Contains(e.Address)
                && e.CreateTime >= startTXDate && string.IsNullOrEmpty(e.SysToUserTXID) && (e.MoveStatus == MoveStatus.Finish || e.MoveStatus == MoveStatus.MoveSysWaitConfirm));
                foreach (var item in ethGroupTXList)
                {
                    try
                    {
                        var currentCoin = coins.First(f => f.Id == item.Key.CoinID);
                        var ethProvider = await this._cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(currentCoin);
                        var balanceResult = ethProvider.GetBalance(new BalanceData
                        {
                            Address = item.Key.Address,
                            TokenContractAddress = currentCoin.TokenCoinAddress
                        });
                        decimal orgBalance = 0;
                        if (waitMoveETHList.Any())
                        {
                            orgBalance = waitMoveETHList.Where(f => f.CoinID == item.Key.CoinID && f.FromAddress == item.Key.Address).Sum(f => f.Amount);
                        }
                        var needBalance = item.Sum(f => f.Amount);
                        var realBalance = balanceResult.Result - orgBalance;
                        if (Math.Round(realBalance - needBalance, 15) < 0)
                        {
                            //余额是否足够
                            bool isBalaceEnough = false;
                            if (useETHBalanceTokens.Any())
                            {
                                //被代币用作矿工费
                                var tokenUsedAmount = useETHBalanceTokens.Where(f => f.CoinID == currentCoin.Id && f.Address == item.Key.Address).Sum(f => f.MoveSysMinefee);
                                if (Math.Round(realBalance + tokenUsedAmount - needBalance) >= 0 && tokenUsedAmount < 0.01m)
                                {
                                    isBalaceEnough = true;
                                }
                            }
                            if (!isBalaceEnough)
                            {
                                foreach (var txItem in item)
                                {
                                    string message = $"检测到未能到账TXID:{txItem.TXID},Coin Code:{currentCoin.Code}";
                                    this._logger.LogWarning(message);
                                    ExceptionlessClient.Default.CreateLog("未能到账TXID", message).Submit();
                                    transactionIns.Remove(txItem);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        foreach (var txItem in item)
                        {
                            transactionIns.Remove(txItem);
                        }
                        this._logger.LogError(ex, $"未能到账TXID");
                        ex.ToExceptionless().Submit();
                    }
                }
            }
        }

        private async Task HandleETHToken(IEnumerable<Entity.Foundation.Coin> coins, List<CoinTransactionIn> transactionIns)
        {
            var ethTokenCoinIDList = coins.Where(f => f.ProviderType == ProviderType.ETHTokenCoin).Select(f => f.Id).Distinct().ToList();
            var ethTokenTXList = transactionIns.Where(f => ethTokenCoinIDList.Contains(f.CoinID)).ToList();
            if (ethTokenTXList.Any())
            {
                var txUserAddress = transactionIns.Where(f => ethTokenCoinIDList.Contains(f.CoinID)).Select(f => f.Address).Distinct().ToList();
                var waitMoveTokenList = await this._coinTransactionInBusiness.GetListAsync(e => ethTokenCoinIDList.Contains(e.CoinID) && e.MoveStatus != MoveStatus.Finish && e.MoveStatus != MoveStatus.Invalid && txUserAddress.Contains(e.Address));
                var ethTokenGroupTXList = ethTokenTXList.GroupBy(f => new { f.CoinID, f.Address }).ToList();
                foreach (var item in ethTokenGroupTXList)
                {
                    try
                    {
                        var tokenCoin = coins.First(f => f.Id == item.Key.CoinID);
                        var ethTokenProvider = await this._cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(tokenCoin);
                        var balanceResult = ethTokenProvider.GetBalance(new BalanceData
                        {
                            Address = item.Key.Address,
                            TokenContractAddress = tokenCoin.TokenCoinAddress
                        });
                        decimal orgBalance = 0;
                        if (waitMoveTokenList.Any())
                        {
                            orgBalance = waitMoveTokenList.Where(f => f.CoinID == item.Key.CoinID && f.Address == item.Key.Address).Sum(f => f.Amount);
                        }
                        var needBalance = item.Sum(f => f.Amount);
                        var realBalance = balanceResult.Result - orgBalance;
                        if (Math.Round(realBalance - needBalance, 15) < 0)
                        {
                            foreach (var txItem in item)
                            {
                                var message = $"检测到未能到账ETH Token TXID:{txItem.TXID},Coin Code:{tokenCoin.Code}";
                                this._logger.LogWarning(message);
                                ExceptionlessClient.Default.CreateLog("未能到账TXID", message).Submit();
                                transactionIns.Remove(txItem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        foreach (var txItem in item)
                        {
                            transactionIns.Remove(txItem);
                        }
                        this._logger.LogError(ex, $"未能到账TXID");
                        ex.ToExceptionless().Submit();
                    }
                }
            }
        }
    }
}
