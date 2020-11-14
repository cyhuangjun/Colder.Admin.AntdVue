using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public interface ICoinContranctTransactionInBusiness
    {
        Task<PageResult<CoinContranctTransactionIn>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<CoinContranctTransactionIn> GetTheDataAsync(string id);
        Task AddDataAsync(CoinContranctTransactionIn data);
        Task UpdateDataAsync(CoinContranctTransactionIn data);
        Task DeleteDataAsync(List<string> ids);
        CoinContranctTransactionIn GetEntity(Expression<Func<CoinContranctTransactionIn, bool>> expression);
        Task<CoinContranctTransactionIn> GetEntityAsync(Expression<Func<CoinContranctTransactionIn, bool>> expression);
        List<CoinContranctTransactionIn> GetList(Expression<Func<CoinContranctTransactionIn, bool>> expression);
        Task<List<CoinContranctTransactionIn>> GetListAsync(Expression<Func<CoinContranctTransactionIn, bool>> expression);
        List<CoinContranctTransactionIn> GetList<TKey>(Expression<Func<CoinContranctTransactionIn, bool>> expression, Expression<Func<CoinContranctTransactionIn, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<CoinContranctTransactionIn>> GetListAsync<TKey>(Expression<Func<CoinContranctTransactionIn, bool>> expression, Expression<Func<CoinContranctTransactionIn, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }
}