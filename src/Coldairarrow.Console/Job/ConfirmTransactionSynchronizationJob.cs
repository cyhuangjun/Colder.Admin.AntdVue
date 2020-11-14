using Autofac;
using CCPP.Cryptocurrency.Common;
using Coldairarrow.Business.Foundation;
using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.Enum;
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
    public class ConfirmTransactionSynchronizationJob : IJob
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
        private readonly ILogger<ConfirmTransactionSynchronizationJob> _logger;
        private readonly ICacheDataBusiness _cacheDataBusiness;
        private readonly ICoinTransactionInBusiness _coinTransactionInBusiness;
        private readonly ICoinSynchronizationBlockBusiness _coinSynchronizationBlockBusiness;
        private readonly ICryptocurrencyBusiness _cryptocurrencyBusiness;
        #endregion

        public ConfirmTransactionSynchronizationJob()
        {
            this._container = RmesAutoFacModule.GetContainer();
            this._logger = this._container.Resolve<ILogger<ConfirmTransactionSynchronizationJob>>();
            this._cacheDataBusiness = this._container.Resolve<ICacheDataBusiness>();
            this._coinTransactionInBusiness = this._container.Resolve<ICoinTransactionInBusiness>();
            this._coinSynchronizationBlockBusiness = this._container.Resolve<ICoinSynchronizationBlockBusiness>();
            this._cryptocurrencyBusiness = this._container.Resolve<ICryptocurrencyBusiness>();
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
            var coins = await this._cacheDataBusiness.GetCoinsAsync();
            List<CoinConfirmatioItem> coinConfirmationItemList = new List<CoinConfirmatioItem>();
            foreach (var item in coins)
            {
                if (!item.IsSupportWallet) continue;
                CoinConfirmatioItem coinConfirmatioItem = new CoinConfirmatioItem
                {
                    CoinID = item.Id,
                    MinConfirmation = item.MinConfirms
                };
                coinConfirmationItemList.Add(coinConfirmatioItem);
            }
            var dataList = await this._coinTransactionInBusiness.GetListAsync(f => coinConfirmationItemList.Exists(t => t.CoinID == f.CoinID && t.MinConfirmation > f.Confirmations) && f.Status == TransactionStatus.WaitConfirm, f => f.LastUpdateTime, 1, GlobalData.MaxPageSize);
            if (dataList == null || !dataList.Any()) return;
            var synchronizeCoinRecordList = await this._coinSynchronizationBlockBusiness.GetListAsync(null);
            var dataGroupList = dataList.GroupBy(f => f.CoinID).ToList();
            foreach (var transactionCoinG in dataGroupList)
            {
                try
                {
                    var providerCoin = coins.First(f => f.Id == transactionCoinG.Key);
                    var cryptocurrencyProvider = await this._cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(providerCoin);                    
                    foreach (var transaction in transactionCoinG)
                    {
                        transaction.LastUpdateTime = DateTime.Now;
                        try
                        {
                            if (cryptocurrencyProvider.HasTransactionReceipt)
                            {
                                var txReceipt = cryptocurrencyProvider.GetTransactionReceipt(transaction.TXID);
                                if (!string.IsNullOrEmpty(txReceipt.Error) || txReceipt.Result == null || !txReceipt.Result.IsSuccess)
                                {
                                    continue;
                                }
                            }
                            var result = cryptocurrencyProvider.GetTransaction(transaction.TXID);
                            if (result != null && result.Result != null)
                            {
                                var txResult = result.Result;
                                if (cryptocurrencyProvider.HasNotBlockCount)
                                {
                                    if (txResult.Valid)
                                        transaction.Confirmations = providerCoin.MinConfirms;
                                    else
                                        transaction.Status = TransactionStatus.Exception;
                                }
                                else
                                {
                                    transaction.Confirmations = (int)txResult.Confirmations;
                                }
                                await this._coinTransactionInBusiness.UpdateDataAsync(transaction);
                            }
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogError(ex, $"获取交易ID信息失败,TXID:{transaction.TXID}");
                            ex.ToExceptionless().Submit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, $"交易币种出错:{transactionCoinG.Key} {ex.Message}");
                    ex.ToExceptionless().Submit();
                }
            }
        }
    }

    /// <summary>
    /// 虚拟币确认信息
    /// </summary>
    public class CoinConfirmation
    {
        /// <summary>
        /// 默认确认个数
        /// </summary>
        public int DefaultConfirmation { get; set; }
        /// <summary>
        /// 虚拟币确认特殊信息
        /// </summary>
        public List<CoinConfirmatioItem> CoinConfirmationItemList { get; set; }
    }
    public class CoinConfirmatioItem
    {
        public string CoinID { get; set; }
        public int MinConfirmation { get; set; }
    }
}
