using Coldairarrow.Entity.DTO;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public interface IAssetsBusiness
    {
        Task<decimal> GetBalance(string tenantId, string coinCode);

        Task<AjaxResult> UpdateAssets(params AssetsChangeItemDTO[] assetsChangeItems);
    }
}