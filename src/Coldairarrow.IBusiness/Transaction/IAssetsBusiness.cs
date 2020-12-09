using Coldairarrow.Entity.DTO;
using Coldairarrow.Entity.Foundation;
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

        Task<PageResult<WalletOutDTO>> GetWalletDataList(PageInput<WalletInputDTO> input);

        Task<PageResult<WalletSystemAddressOutDTO>> GetWalletSystemAddressDataList(PageInput<WalletInputDTO> input);

        Task CreateSystemAddress(string coinId);
    }

    public class AssetsInputDTO
    {
        public string TenantId { get; set; }
        public string CoinID { get; set; }
    }

    public class WalletInputDTO
    {
        public string CoinID { get; set; }
    }

    public class WalletOutDTO
    {
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { set; get; }

        public decimal Amount { set; get; }
    }

    [Map(typeof(SysWallet))]
    public class WalletSystemAddressOutDTO : SysWallet
    {
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { set; get; }

        public decimal Amount { set; get; }

        public string IsEnabledStr { get { return this.Enabled ? "是" : "否"; } }

        public string IsDeletedStr { get { return this.Deleted ? "是" : "否"; } }
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