using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using EFCore.Sharding;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public class AssetsWasteBookBusiness : BaseBusiness<AssetsWasteBook>, IAssetsWasteBookBusiness, ITransientDependency
    {
        public AssetsWasteBookBusiness(IDbAccessor db)
            : base(db)
        {
        }

        #region 外部接口

        public async Task<PageResult<AssetsWasteBookOutDTO>> GetDataListAsync(PageInput<AssetsWasteBookInputDTO> input)
        {
            Expression<Func<AssetsWasteBook, Coin, Base_Department, AssetsWasteBookOutDTO>> select = (a, b, c) => new AssetsWasteBookOutDTO
            {
                Currency = b.Code,
                Tenant = c.Name
            };
            select = select.BuildExtendSelectExpre();
            var q = from a in this.Db.GetIQueryable<AssetsWasteBook>().AsExpandable()
                    join b in Db.GetIQueryable<Coin>() on a.CoinID equals b.Id into ab
                    from b in ab.DefaultIfEmpty()
                    join c in Db.GetIQueryable<Base_Department>() on a.TenantId equals c.Id into ac
                    from c in ac.DefaultIfEmpty()
                    select @select.Invoke(a, b, c);
            //筛选
            var where = LinqHelper.True<AssetsWasteBookOutDTO>();
            var search = input.Search;
            if (!string.IsNullOrEmpty(search.CoinID))
                where = where.And(x => x.CoinID == search.CoinID);
            if (!string.IsNullOrEmpty(search.TenantId))
                where = where.And(x => x.TenantId == search.TenantId);
            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<AssetsWasteBook> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(AssetsWasteBook data)
        {
            await InsertAsync(data);
        }
        #endregion

        #region 私有成员

        #endregion
    }
}