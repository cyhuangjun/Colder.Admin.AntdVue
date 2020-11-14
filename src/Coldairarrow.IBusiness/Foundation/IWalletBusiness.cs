using Coldairarrow.Entity.Foundation;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Foundation
{
    public interface IWalletBusiness
    {
        Task<PageResult<Wallet>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<Wallet> GetTheDataAsync(string id);
        Task AddDataAsync(Wallet data);
        Task UpdateDataAsync(Wallet data);
        Task DeleteDataAsync(List<string> ids);

        Wallet GetEntity(Expression<Func<Wallet, bool>> expression);
        Task<Wallet> GetEntityAsync(Expression<Func<Wallet, bool>> expression);
        List<Wallet> GetList(Expression<Func<Wallet, bool>> expression);
        Task<List<Wallet>> GetListAsync(Expression<Func<Wallet, bool>> expression);
        List<Wallet> GetList<TKey>(Expression<Func<Wallet, bool>> expression, Expression<Func<Wallet, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<Wallet>> GetListAsync<TKey>(Expression<Func<Wallet, bool>> expression, Expression<Func<Wallet, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }
}