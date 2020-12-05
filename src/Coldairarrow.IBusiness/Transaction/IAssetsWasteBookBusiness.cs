using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public interface IAssetsWasteBookBusiness
    {
        Task<PageResult<AssetsWasteBook>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<AssetsWasteBook> GetTheDataAsync(string id);
        Task AddDataAsync(AssetsWasteBook data);

        AssetsWasteBook GetEntity(Expression<Func<AssetsWasteBook, bool>> expression);
        Task<AssetsWasteBook> GetEntityAsync(Expression<Func<AssetsWasteBook, bool>> expression);
        List<AssetsWasteBook> GetList(Expression<Func<AssetsWasteBook, bool>> expression);
        Task<List<AssetsWasteBook>> GetListAsync(Expression<Func<AssetsWasteBook, bool>> expression);
        List<AssetsWasteBook> GetList<TKey>(Expression<Func<AssetsWasteBook, bool>> expression, Expression<Func<AssetsWasteBook, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<AssetsWasteBook>> GetListAsync<TKey>(Expression<Func<AssetsWasteBook, bool>> expression, Expression<Func<AssetsWasteBook, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }
}