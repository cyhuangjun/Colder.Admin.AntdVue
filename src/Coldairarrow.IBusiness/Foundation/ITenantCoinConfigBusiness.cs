using Coldairarrow.Entity.Foundation;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Foundation
{
    public interface ITenantCoinConfigBusiness
    {
        Task<PageResult<TenantCoinConfig>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<TenantCoinConfig> GetTheDataAsync(string id);
        Task AddDataAsync(TenantCoinConfig data);
        Task UpdateDataAsync(TenantCoinConfig data);
        Task DeleteDataAsync(List<string> ids);
    }
}