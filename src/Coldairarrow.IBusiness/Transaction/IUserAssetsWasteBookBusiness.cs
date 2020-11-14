using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public interface IUserAssetsWasteBookBusiness
    {
        Task<PageResult<UserAssetsWasteBook>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<UserAssetsWasteBook> GetTheDataAsync(string id);
        Task AddDataAsync(UserAssetsWasteBook data);       
    }
}