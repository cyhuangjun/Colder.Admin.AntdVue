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
        private readonly ICoinTransactionInBusiness _coinTransactionInBusiness;
        private readonly IBase_UserBusiness _base_UserBusiness;
        #endregion

        public CallbackJob()
        {
            this._container = RmesAutoFacModule.GetContainer();
            this._logger = this._container.Resolve<ILogger<DepositAccountingJob>>();
            this._cacheDataBusiness = this._container.Resolve<ICacheDataBusiness>();
            this._transfersBusiness = this._container.Resolve<ITransfersBusiness>();
            this._coinTransactionInBusiness = this._container.Resolve<ICoinTransactionInBusiness>();
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
            var records = await this._coinTransactionInBusiness.GetListAsync(e => e.Status == TransactionStatus.ArrivedAccount && (e.CallBackStatus == null || e.CallBackStatus == APICallBackStatus.None));
            foreach (var record in records)
            {
                var user = await this._cacheDataBusiness.GetUserAsync(record.UserID);
                record.CallBackStatus = await ExcutePaymentCallBack(record, user.PaymentCallbackUrl);
                await this._coinTransactionInBusiness.UpdateDataAsync(record);
            }
        }

        private async Task ExcuteWithdrawal()
        {
            var records = await this._transfersBusiness.GetListAsync(e => e.Status == TransfersStatus.Finished && (e.CallBackStatus == null || e.CallBackStatus == APICallBackStatus.None));
            foreach (var record in records)
            {
                var user = await this._cacheDataBusiness.GetUserAsync(record.UserID);
                record.CallBackStatus = await ExcuteTransfersCallBack(record, user.TransfersCallbackUrl);
                await this._transfersBusiness.UpdateDataAsync(record);
            }
        }

        private async Task<APICallBackStatus> ExcuteTransfersCallBack(Transfers transfers, string callBackUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(callBackUrl)) return APICallBackStatus.NotCallBackConfigured;
                var customer = await this._base_UserBusiness.GetTheDataAsync(transfers.UserID);
                var request = new CashOutRequest()
                {
                    AddressTo = transfers.AddressTo,
                    AmountFrom = transfers.AmountFrom,
                    AmountTo = transfers.AmountTo,
                    CallbackUrl = callBackUrl,
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
                var content = RestSharpHttpHelper.RestAction(callBackUrl, string.Empty, signParameters, RestSharp.Method.POST);
                return APICallBackStatus.Succeeded;
            }
            catch (Exception e)
            {
                e.ToExceptionless().Submit();
                return APICallBackStatus.Failed;
            }
        }

        private async Task<APICallBackStatus> ExcutePaymentCallBack(CoinTransactionIn payment, string callBackUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(callBackUrl)) return APICallBackStatus.NotCallBackConfigured;
                var wallet = await this._cacheDataBusiness.GetWalletByAddress(payment.Address);
                var customer = await this._base_UserBusiness.GetTheDataAsync(payment.UserID);
                var coin = await this._cacheDataBusiness.GetCoinAsync(payment.CoinID);
                var request = new CashInRequest()
                {
                    Amount = payment.Amount,
                    ClientUid = wallet.UID,
                    Fee = payment.CoinInHandlingFee ?? 0m,
                    FromAddress = payment.FromAddress,
                    PaymentId = payment.Id,
                    PayAddress = payment.Address,
                    PayCurrency = coin.Code,
                    TXID = payment.TXID,
                    Status = (payment.Status == TransactionStatus.ArrivedAccount ? PaymentStatus.Finished : PaymentStatus.Waiting)

                };
                var signParameters = request.ToDictionary();
                if (!string.IsNullOrEmpty(customer.SecretKey))
                {
                    request.Mac = signParameters.Sign(customer.SecretKey);
                    signParameters.Add("mac", request.Mac);
                }
                var content = RestSharpHttpHelper.RestAction(callBackUrl, string.Empty, signParameters, RestSharp.Method.POST);
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
