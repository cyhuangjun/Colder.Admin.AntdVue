using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        Transfers GetEntity(Expression<Func<Transfers, bool>> expression);
        Task<Transfers> GetEntityAsync(Expression<Func<Transfers, bool>> expression);
        List<Transfers> GetList(Expression<Func<Transfers, bool>> expression);
        Task<List<Transfers>> GetListAsync(Expression<Func<Transfers, bool>> expression);
        List<Transfers> GetList<TKey>(Expression<Func<Transfers, bool>> expression, Expression<Func<Transfers, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<Transfers>> GetListAsync<TKey>(Expression<Func<Transfers, bool>> expression, Expression<Func<Transfers, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }
}