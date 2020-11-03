using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public interface IPaymentBusiness
    {
        Task<PageResult<Payment>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<Payment> GetTheDataAsync(string id);
        Task AddDataAsync(Payment data);
        Task UpdateDataAsync(Payment data);
        Task DeleteDataAsync(List<string> ids);
    }
}