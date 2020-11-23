using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public interface IUserAssetsWasteBookBusiness
    {
        Task<PageResult<UserAssetsWasteBook>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<UserAssetsWasteBook> GetTheDataAsync(string id);
        Task AddDataAsync(UserAssetsWasteBook data);

        UserAssetsWasteBook GetEntity(Expression<Func<UserAssetsWasteBook, bool>> expression);
        Task<UserAssetsWasteBook> GetEntityAsync(Expression<Func<UserAssetsWasteBook, bool>> expression);
        List<UserAssetsWasteBook> GetList(Expression<Func<UserAssetsWasteBook, bool>> expression);
        Task<List<UserAssetsWasteBook>> GetListAsync(Expression<Func<UserAssetsWasteBook, bool>> expression);
        List<UserAssetsWasteBook> GetList<TKey>(Expression<Func<UserAssetsWasteBook, bool>> expression, Expression<Func<UserAssetsWasteBook, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<UserAssetsWasteBook>> GetListAsync<TKey>(Expression<Func<UserAssetsWasteBook, bool>> expression, Expression<Func<UserAssetsWasteBook, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }
}