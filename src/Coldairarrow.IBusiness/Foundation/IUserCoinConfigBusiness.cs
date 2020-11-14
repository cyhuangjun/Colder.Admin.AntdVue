using Coldairarrow.Entity.Foundation;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Foundation
{
    public interface IUserCoinConfigBusiness
    {
        Task<PageResult<UserCoinConfig>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<UserCoinConfig> GetTheDataAsync(string id);
        Task AddDataAsync(UserCoinConfig data);
        Task UpdateDataAsync(UserCoinConfig data);
        Task DeleteDataAsync(List<string> ids);
    }
}