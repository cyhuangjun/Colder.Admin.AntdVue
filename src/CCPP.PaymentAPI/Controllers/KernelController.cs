using Coldairarrow.Business.Foundation;
using Coldairarrow.Entity.Request;
using Coldairarrow.Entity.Response;
using Coldairarrow.IBusiness.Market;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCPP.PaymentAPI.Controllers
{
    /// <summary>
    /// Cryptocurrency Payments API
    /// </summary>
    [Route("api/v1/[action]")]
    [ApiController]
    public class KernelController : ControllerBase
    {
        #region DI
        private readonly ICoinBusiness _coinBus;
        private readonly ICurrencyBusiness _currencyBusiness;
        private readonly IMarketBusiness _marketBus;
        /// <summary>
        /// Cryptocurrency Payments API Constructor
        /// </summary> 
        public KernelController(ICoinBusiness coinBus, 
                                ICurrencyBusiness currencyBusiness,
                                IMarketBusiness marketBus)
        {
            _coinBus = coinBus;
            _currencyBusiness = currencyBusiness;
            _marketBus = marketBus;
        }  
        #endregion
        /// <summary>
        /// Get available Fiat currencies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult<IEnumerable<string>>> FiatCurrencies()
        {
            return await _currencyBusiness.GetFiatCurrenciesAsync();
        }
        /// <summary>
        /// Get available Cryptocurrencies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult<IEnumerable<string>>> CryptoCurrencies()
        {
            return await _coinBus.GetCryptoCurrenciesAsync();
        }
        /// <summary>
        /// Get estimated price
        /// This is a method for calculating the approximate price in cryptocurrency for a given value in Fiat currency     
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult<CurrencyEstimateViewDto>> Estimate(EstimateRequest request)
        {
            return await _marketBus.EstimateAsync(request);
        }
        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult<PaymentViewDto>> Payment(PaymentRequest request)
        {
            var userId = string.Empty;
            return await _marketBus.PaymentAsync(userId, request);
        }
        /// <summary>
        /// Get payment status
        /// </summary>
        /// <param name="paymentId">payment Id</param>
        /// <returns></returns>
        [Route("api/v1/payment/{paymentId}")]
        [HttpGet]
        public async Task<AjaxResult<PaymentResultViewDto>> Payment(string paymentId)
        {
            var userId = string.Empty;
            return await _marketBus.PaymentAsync(userId, paymentId);
        }
        /// <summary>
        /// Get the minimum payment amount
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        [Route("api/v1/minamount/{currency}")]
        [HttpGet]
        public async Task<AjaxResult<MinAmountViewDto>> MinAmount(string currency)
        {
            var userId = string.Empty;
            return await _marketBus.MinAmountAsync(userId, currency);
        }
        /// <summary>
        /// Transfers Currency
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult<TransfersViewDto>> Transfers(TransfersRequest request)
        {
            var userId = string.Empty;
            return await _marketBus.TransfersAsync(userId, request);
        }
        /// <summary>
        /// Get Transfers status
        /// </summary>
        /// <param name="transfersId"></param>
        /// <returns></returns>
        [Route("api/v1/transfers/{transfersId}")]
        [HttpGet]
        public async Task<AjaxResult<TransfersViewDto>> Transfers(string transfersId)
        {
            var userId = string.Empty;
            return await _marketBus.TransfersAsync(userId, transfersId);
        }
    }
}
