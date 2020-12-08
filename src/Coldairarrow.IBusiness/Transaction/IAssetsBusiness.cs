using Coldairarrow.Entity.DTO;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public interface IAssetsBusiness
    {
        Task<PageResult<AssetsOutDTO>> GetDataListAsync(PageInput<AssetsInputDTO> input);

        Task<Assets> GetTheDataAsync(string id);

        Task<decimal> GetBalance(string tenantId, string coinId);

        Task<AjaxResult> UpdateAssets(params AssetsChangeItemDTO[] assetsChangeItems);

        Task InitTenantAssets(string tenantId);
    }

    public class AssetsInputDTO
    {
        public string TenantId { get; set; }
        public string CoinID { get; set; }
    }

    [Map(typeof(Assets))]
    public class AssetsOutDTO : Assets
    {
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { set; get; }
        /// <summary>
        /// 商户
        /// </summary>
        public string Tenant { set; get; }
    }
}