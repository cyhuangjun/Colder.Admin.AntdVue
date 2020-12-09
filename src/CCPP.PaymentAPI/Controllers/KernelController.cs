using CCPP.PaymentAPI.ActionFilter;
using Coldairarrow.Api;
using Coldairarrow.Business.Base_Manage;
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
    [CheckApiKey]
    public class KernelController : BaseController
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
                                IMarketBusiness marketBus,
                                IBase_DepartmentBusiness base_DepartmentBusiness) : base(base_DepartmentBusiness)
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
            var response = await _currencyBusiness.GetFiatCurrenciesAsync();
            response.Mac = this.Sign(response.Data, this.CurrentTenant.SecretKey);
            return response;
        }

        /// <summary>
        /// Get available Cryptocurrencies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult<IEnumerable<string>>> CryptoCurrencies()
        {
            var response = await _coinBus.GetCryptoCurrenciesAsync();
            response.Mac = this.Sign(response.Data, this.CurrentTenant.SecretKey);
            return response;
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
            var response = await _marketBus.EstimateAsync(request);
            response.Mac = this.Sign(response.Data, this.CurrentTenant.SecretKey);
            return response;
        }

        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult<PaymentViewDto>> Payment(PaymentRequest request)
        { 
            var response = await _marketBus.PaymentAsync(this.CurrentTenant.Id, request);
            response.Mac = this.Sign(response.Data, this.CurrentTenant.SecretKey);
            return response;
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
            var response = await _marketBus.MinAmountAsync(this.CurrentTenant.Id, currency);
            response.Mac = this.Sign(response.Data, this.CurrentTenant.SecretKey);
            return response;
        }

        /// <summary>
        /// Transfers Currency
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult<TransfersViewDto>> Transfers(TransfersRequest request)
        { 
            var response = await _marketBus.TransfersAsync(this.CurrentTenant.Id, request);
            response.Mac = this.Sign(response.Data, this.CurrentTenant.SecretKey);
            return response;
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
            var response = await _marketBus.TransfersAsync(this.CurrentTenant.Id, transfersId);
            response.Mac = this.Sign(response.Data, this.CurrentTenant.SecretKey);
            return response;
        }
    }
}
