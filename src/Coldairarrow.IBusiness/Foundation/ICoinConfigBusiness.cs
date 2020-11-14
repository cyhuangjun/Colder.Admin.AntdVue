using Coldairarrow.Entity.Foundation;
using Coldairarrow.Entity.Response;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Foundation
{
    public interface ICoinConfigBusiness
    {
        Task<PageResult<CoinConfig>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<CoinConfig> GetTheDataAsync(string id);
        Task AddDataAsync(CoinConfig data);
        Task UpdateDataAsync(CoinConfig data);
        Task DeleteDataAsync(List<string> ids);
        Task<MinAmountViewDto> MinAmountAsync(string userId, string currency);
    }
}