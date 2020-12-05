using Autofac;
using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.Enum;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Scheduler.Core;
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
    public class TransfersSyncJob : IJob
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
        private readonly ILogger<TransfersSyncJob> _logger;
        private readonly ICacheDataBusiness _cacheDataBusiness;
        private readonly ITransfersBusiness _transfersBusiness; 
        private readonly IBase_UserBusiness _base_UserBusiness;
        private readonly ICoinTransactionOutBusiness _coinTransactionOutBusiness;
        #endregion

        public TransfersSyncJob()
        {
            this._container = RmesAutoFacModule.GetContainer();
            this._logger = this._container.Resolve<ILogger<TransfersSyncJob>>();
            this._cacheDataBusiness = this._container.Resolve<ICacheDataBusiness>();
            this._transfersBusiness = this._container.Resolve<ITransfersBusiness>(); 
            this._base_UserBusiness = this._container.Resolve<IBase_UserBusiness>();
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
                    await ExcuteSync();
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

        private async Task ExcuteSync()
        {
            var records = await this._transfersBusiness.GetListAsync(e=>e.Status != TransfersStatus.Finished && e.Status != TransfersStatus.Failed);
            var transactionOutIds = records.Select(e=>e.TransactionOutID);
            var transactionOuts = await this._coinTransactionOutBusiness.GetListAsync(e=> transactionOutIds.Contains(e.Id));
            foreach (var item in transactionOuts)
            {
                var record = records.First(e => e.TransactionOutID == item.Id);
                if (item.Status == TransactionStatus.Apply) continue;
                switch (item.Status)
                {
                    case TransactionStatus.Apply:
                        continue;
                    case TransactionStatus.WaitConfirm:
                        if (record.Status == TransfersStatus.Confirming) continue;
                        record.Status = TransfersStatus.Confirming;
                        record.ApproverID = item.ApproverID;
                        record.ApproveTime = item.ApproveTime;
                        record.UpdatedAt = item.LastUpdateTime;
                        record.LastUpdateUserID = item.LastUpdateUserID;
                        await this._transfersBusiness.UpdateDataAsync(record);
                        break;
                    case TransactionStatus.Finished:
                        record.Status = TransfersStatus.Finished;
                        record.UpdatedAt = item.LastUpdateTime;
                        record.LastUpdateUserID = item.LastUpdateUserID;
                        await this._transfersBusiness.UpdateDataAsync(record);
                        break;
                    case TransactionStatus.Cancel:
                        record.Status = TransfersStatus.Cancel;
                        record.ApproverID = item.ApproverID;
                        record.ApproveTime = item.ApproveTime;
                        record.UpdatedAt = item.LastUpdateTime;
                        record.LastUpdateUserID = item.LastUpdateUserID;
                        await this._transfersBusiness.UpdateDataAsync(record);
                        break;
                    case TransactionStatus.ApprovalReject:
                        record.Status = TransfersStatus.Failed;
                        record.ApproverID = item.ApproverID;
                        record.ApproveTime = item.ApproveTime;
                        record.UpdatedAt = item.LastUpdateTime;
                        record.LastUpdateUserID = item.LastUpdateUserID;
                        await this._transfersBusiness.UpdateDataAsync(record);
                        break;
                }
            }
        }
    }
}
