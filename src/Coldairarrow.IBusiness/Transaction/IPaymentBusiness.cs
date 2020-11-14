using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        Payment GetEntity(Expression<Func<Payment, bool>> expression);
        Task<Payment> GetEntityAsync(Expression<Func<Payment, bool>> expression);
        List<Payment> GetList(Expression<Func<Payment, bool>> expression);
        Task<List<Payment>> GetListAsync(Expression<Func<Payment, bool>> expression);
        List<Payment> GetList<TKey>(Expression<Func<Payment, bool>> expression, Expression<Func<Payment, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<Payment>> GetListAsync<TKey>(Expression<Func<Payment, bool>> expression, Expression<Func<Payment, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }
}