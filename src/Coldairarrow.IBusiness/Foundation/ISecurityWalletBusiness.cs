using Coldairarrow.Entity.Foundation;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Foundation
{
    public interface ISecurityWalletBusiness
    {
        Task<PageResult<SecurityWallet>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<SecurityWallet> GetTheDataAsync(string id);
        Task AddDataAsync(SecurityWallet data);
        Task UpdateDataAsync(SecurityWallet data);
        Task DeleteDataAsync(List<string> ids);

        SecurityWallet GetEntity(Expression<Func<SecurityWallet, bool>> expression);
        Task<SecurityWallet> GetEntityAsync(Expression<Func<SecurityWallet, bool>> expression);
        List<SecurityWallet> GetList(Expression<Func<SecurityWallet, bool>> expression);
        Task<List<SecurityWallet>> GetListAsync(Expression<Func<SecurityWallet, bool>> expression);
        List<SecurityWallet> GetList<TKey>(Expression<Func<SecurityWallet, bool>> expression, Expression<Func<SecurityWallet, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<SecurityWallet>> GetListAsync<TKey>(Expression<Func<SecurityWallet, bool>> expression, Expression<Func<SecurityWallet, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }
}