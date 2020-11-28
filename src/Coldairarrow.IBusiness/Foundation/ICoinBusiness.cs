using Coldairarrow.Entity.Foundation;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Foundation
{
    public interface ICoinBusiness
    {
        Task<PageResult<Coin>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<Coin> GetTheDataAsync(string id);
        Task AddDataAsync(Coin data);
        Task UpdateDataAsync(Coin data);
        Task DeleteDataAsync(List<string> ids);
        Task<AjaxResult<IEnumerable<string>>> GetCryptoCurrenciesAsync();

        Task<Coin> GetCoinByCodeAsync(string code);


        Coin GetEntity(Expression<Func<Coin, bool>> expression);
        Task<Coin> GetEntityAsync(Expression<Func<Coin, bool>> expression);
        List<Coin> GetList(Expression<Func<Coin, bool>> expression);
        Task<List<Coin>> GetListAsync(Expression<Func<Coin, bool>> expression);
        List<Coin> GetList<TKey>(Expression<Func<Coin, bool>> expression, Expression<Func<Coin, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<Coin>> GetListAsync<TKey>(Expression<Func<Coin, bool>> expression, Expression<Func<Coin, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }
}