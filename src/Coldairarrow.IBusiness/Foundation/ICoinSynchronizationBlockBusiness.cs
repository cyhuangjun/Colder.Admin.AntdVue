using Coldairarrow.Entity.Foundation;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Foundation
{
    public interface ICoinSynchronizationBlockBusiness
    {
        Task<PageResult<CoinSynchronizationBlock>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<CoinSynchronizationBlock> GetTheDataAsync(string id);
        Task AddDataAsync(CoinSynchronizationBlock data);
        Task UpdateDataAsync(CoinSynchronizationBlock data);
        Task DeleteDataAsync(List<string> ids);
        Task<IEnumerable<CoinSynchronizationBlock>> GetDataListAsync(IEnumerable<string> syncCoinIDs);

        CoinSynchronizationBlock GetEntity(Expression<Func<CoinSynchronizationBlock, bool>> expression);
        Task<CoinSynchronizationBlock> GetEntityAsync(Expression<Func<CoinSynchronizationBlock, bool>> expression);
        List<CoinSynchronizationBlock> GetList(Expression<Func<CoinSynchronizationBlock, bool>> expression);
        Task<List<CoinSynchronizationBlock>> GetListAsync(Expression<Func<CoinSynchronizationBlock, bool>> expression);
        List<CoinSynchronizationBlock> GetList<TKey>(Expression<Func<CoinSynchronizationBlock, bool>> expression, Expression<Func<CoinSynchronizationBlock, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<CoinSynchronizationBlock>> GetListAsync<TKey>(Expression<Func<CoinSynchronizationBlock, bool>> expression, Expression<Func<CoinSynchronizationBlock, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }
}