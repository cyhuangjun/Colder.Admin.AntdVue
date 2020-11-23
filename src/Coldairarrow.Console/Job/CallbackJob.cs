using Autofac;
using CCPP.Cryptocurrency.Common;
using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Business.Foundation;
using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.DTO;
using Coldairarrow.Entity.Enum;
using Coldairarrow.Entity.Request;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Scheduler.Core;
using Coldairarrow.Util;
using Coldairarrow.Util.Helper;
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
    public class CallbackJob : IJob
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
        private readonly ILogger<DepositAccountingJob> _logger;
        private readonly ICacheDataBusiness _cacheDataBusiness;
        private readonly ITransfersBusiness _transfersBusiness;
        private readonly IPaymentBusiness _paymentBusiness;
        private readonly IBase_UserBusiness _base_UserBusiness;
        #endregion

        public CallbackJob()
        {
            this._container = RmesAutoFacModule.GetContainer();
            this._logger = this._container.Resolve<ILogger<DepositAccountingJob>>();
            this._cacheDataBusiness = this._container.Resolve<ICacheDataBusiness>();
            this._transfersBusiness = this._container.Resolve<ITransfersBusiness>();
            this._paymentBusiness = this._container.Resolve<IPaymentBusiness>();
            this._base_UserBusiness = this._container.Resolve<IBase_UserBusiness>();
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
                    await ExcuteRecharge();
                    await ExcuteWithdrawal();
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

        private async Task ExcuteRecharge()
        {
            var records = await this._paymentBusiness.GetListAsync(e => e.Status == PaymentStatus.Finished && !string.IsNullOrEmpty(e.CallbackUrl) && (e.CallBackStatus == null || e.CallBackStatus == APICallBackStatus.None));
            foreach (var record in records)
            {
                record.CallBackStatus = await ExcutePaymentCallBack(record);
                await this._paymentBusiness.UpdateDataAsync(record);
            }
        } 

        private async Task ExcuteWithdrawal()
        {
            var records = await this._transfersBusiness.GetListAsync(e => e.Status == TransfersStatus.Finished  && !string.IsNullOrEmpty(e.CallbackUrl) && (e.CallBackStatus == null || e.CallBackStatus == APICallBackStatus.None));
            foreach (var record in records)
            {
                record.CallBackStatus = await ExcuteTransfersCallBack(record);
                await this._transfersBusiness.UpdateDataAsync(record);
            }
        }

        private async Task<APICallBackStatus> ExcuteTransfersCallBack(Transfers transfers)
        {
            try
            {
                if (string.IsNullOrEmpty(transfers.CallbackUrl)) return APICallBackStatus.NotCallBackConfigured;
                var customer = await this._base_UserBusiness.GetTheDataAsync(transfers.UserID);
                var request = new CashOutRequest()
                {
                    AddressTo = transfers.AddressTo,
                    AmountFrom = transfers.AmountFrom,
                    AmountTo = transfers.AmountTo,
                    CallbackUrl = transfers.CallbackUrl,
                    CurrencyFrom = transfers.CurrencyFrom,
                    CurrencyTo = transfers.CurrencyTo,
                    OrderDescription = transfers.OrderDescription,
                    OrderId = transfers.OrderId,
                    Status = transfers.Status,
                    WithdrawId = transfers.Id,
                };
                var signParameters = request.ToDictionary();
                if (!string.IsNullOrEmpty(customer.SecretKey))
                {
                    request.Mac = signParameters.Sign(customer.SecretKey);
                    signParameters.Add("mac", request.Mac);
                }
                var content = RestSharpHttpHelper.RestAction(transfers.CallbackUrl, string.Empty, signParameters, RestSharp.Method.POST);
                return APICallBackStatus.Succeeded;
            }
            catch (Exception e)
            {
                e.ToExceptionless().Submit();
                return APICallBackStatus.Failed;
            }
        }

        private async Task<APICallBackStatus> ExcutePaymentCallBack(Payment payment)
        {
            try
            {
                if (string.IsNullOrEmpty(payment.CallbackUrl)) return APICallBackStatus.NotCallBackConfigured;
                var customer = await this._base_UserBusiness.GetTheDataAsync(payment.UserID);
                var request = new CashInRequest()
                {
                    CreatedAt = payment.CreatedAt,
                    OrderDescription = payment.OrderDescription,
                    UID = payment.UID,
                    PayAddress = payment.PayAddress,
                    PayAmount = payment.PayAmount,
                    PayCurrency = payment.PayCurrency,
                    PaymentId = payment.Id,
                    PriceAmount = payment.PriceAmount,
                    PriceCurrency = payment.PriceCurrency,
                    PurchaseId = payment.PurchaseId,
                    UpdatedAt = payment.UpdatedAt,
                    Status = payment.Status,
                    ActuallyPaid = payment.ActuallyPaid,
                    ActualAmount = payment.ActualAmount,
                    OrderId = payment.OrderId
                };
                var signParameters = request.ToDictionary();
                if (!string.IsNullOrEmpty(customer.SecretKey))
                {
                    request.Mac = signParameters.Sign(customer.SecretKey);
                    signParameters.Add("mac", request.Mac);
                }
                var content = RestSharpHttpHelper.RestAction(payment.CallbackUrl, string.Empty, signParameters, RestSharp.Method.POST);
                return APICallBackStatus.Succeeded;
            }
            catch (Exception e)
            {
                e.ToExceptionless().Submit();
                return APICallBackStatus.Failed;
            }
        }
    }
}
