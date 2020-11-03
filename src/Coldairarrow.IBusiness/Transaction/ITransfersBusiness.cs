using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public interface ITransfersBusiness
    {
        Task<PageResult<Transfers>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<Transfers> GetTheDataAsync(string id);
        Task AddDataAsync(Transfers data);
        Task UpdateDataAsync(Transfers data);
        Task DeleteDataAsync(List<string> ids);
    }
}