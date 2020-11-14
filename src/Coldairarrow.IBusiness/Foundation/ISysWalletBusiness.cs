using Coldairarrow.Entity.Foundation;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Foundation
{
    public interface ISysWalletBusiness
    {
        Task<PageResult<SysWallet>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<SysWallet> GetTheDataAsync(string id);
        Task AddDataAsync(SysWallet data);
        Task UpdateDataAsync(SysWallet data);
        Task DeleteDataAsync(List<string> ids);
        SysWallet GetEntity(Expression<Func<SysWallet, bool>> expression);
        Task<SysWallet> GetEntityAsync(Expression<Func<SysWallet, bool>> expression);
        List<SysWallet> GetList(Expression<Func<SysWallet, bool>> expression);
        Task<List<SysWallet>> GetListAsync(Expression<Func<SysWallet, bool>> expression);
        List<SysWallet> GetList<TKey>(Expression<Func<SysWallet, bool>> expression, Expression<Func<SysWallet, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<SysWallet>> GetListAsync<TKey>(Expression<Func<SysWallet, bool>> expression, Expression<Func<SysWallet, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }
}