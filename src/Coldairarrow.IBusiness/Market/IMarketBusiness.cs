using Coldairarrow.Entity.Request;
using Coldairarrow.Entity.Response;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.IBusiness.Market
{
    public interface IMarketBusiness
    {
        Task<AjaxResult<CurrencyEstimateViewDto>> EstimateAsync(EstimateRequest request);
        Task<AjaxResult<PaymentViewDto>> PaymentAsync(string userId, PaymentRequest request);
        Task<AjaxResult<MinAmountViewDto>> MinAmountAsync(string userId, string currency);
        Task<AjaxResult<TransfersViewDto>> TransfersAsync(string userId, TransfersRequest request);
        Task<AjaxResult<TransfersViewDto>> TransfersAsync(string userId, string withdrawId);
    }
}
