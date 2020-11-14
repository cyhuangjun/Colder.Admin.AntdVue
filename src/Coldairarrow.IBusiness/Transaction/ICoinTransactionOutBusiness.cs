using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;

namespace Coldairarrow.Business.Transaction
{
    public interface ICoinTransactionOutBusiness
    {
        Task<PageResult<CoinTransactionOut>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<CoinTransactionOut> GetTheDataAsync(string id);
        Task AddDataAsync(CoinTransactionOut data);
        Task UpdateDataAsync(CoinTransactionOut data);
        Task DeleteDataAsync(List<string> ids);
        CoinTransactionOut GetEntity(Expression<Func<CoinTransactionOut, bool>> expression);
        Task<CoinTransactionOut> GetEntityAsync(Expression<Func<CoinTransactionOut, bool>> expression);
        List<CoinTransactionOut> GetList(Expression<Func<CoinTransactionOut, bool>> expression);
        Task<List<CoinTransactionOut>> GetListAsync(Expression<Func<CoinTransactionOut, bool>> expression);
        List<CoinTransactionOut> GetList<TKey>(Expression<Func<CoinTransactionOut, bool>> expression, Expression<Func<CoinTransactionOut, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<CoinTransactionOut>> GetListAsync<TKey>(Expression<Func<CoinTransactionOut, bool>> expression, Expression<Func<CoinTransactionOut, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }
}