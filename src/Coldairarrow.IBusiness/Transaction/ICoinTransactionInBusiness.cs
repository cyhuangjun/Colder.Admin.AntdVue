using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;

namespace Coldairarrow.Business.Transaction
{
    public interface ICoinTransactionInBusiness
    {
        Task<PageResult<CoinTransactionIn>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<CoinTransactionIn> GetTheDataAsync(string id);
        Task AddDataAsync(CoinTransactionIn data);
        Task AddDataAsync(List<CoinTransactionIn> data);
        Task UpdateDataAsync(CoinTransactionIn data);
        Task UpdateDataAsync(List<CoinTransactionIn> data);
        Task DeleteDataAsync(List<string> ids);
        CoinTransactionIn GetEntity(Expression<Func<CoinTransactionIn, bool>> expression);
        Task<CoinTransactionIn> GetEntityAsync(Expression<Func<CoinTransactionIn, bool>> expression);
        List<CoinTransactionIn> GetList(Expression<Func<CoinTransactionIn, bool>> expression);
        Task<List<CoinTransactionIn>> GetListAsync(Expression<Func<CoinTransactionIn, bool>> expression);
        List<CoinTransactionIn> GetList<TKey>(Expression<Func<CoinTransactionIn, bool>> expression, Expression<Func<CoinTransactionIn, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<CoinTransactionIn>> GetListAsync<TKey>(Expression<Func<CoinTransactionIn, bool>> expression, Expression<Func<CoinTransactionIn, TKey>> orderByDescending, int pageIndex, int pageSize = 20);

        Task<AjaxResult> SyncCoinTransactionIn(List<CoinTransactionIn> coinTransactionList, List<CoinContranctTransactionIn> coinContranctTransactionIns);
    }
}