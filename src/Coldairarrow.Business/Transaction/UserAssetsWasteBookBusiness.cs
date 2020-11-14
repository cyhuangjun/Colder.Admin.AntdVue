using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using EFCore.Sharding;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public class UserAssetsWasteBookBusiness : BaseBusiness<UserAssetsWasteBook>, IUserAssetsWasteBookBusiness, ITransientDependency
    {
        public UserAssetsWasteBookBusiness(IDbAccessor db)
            : base(db)
        {
        }

        #region 外部接口

        public async Task<PageResult<UserAssetsWasteBook>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<UserAssetsWasteBook>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<UserAssetsWasteBook, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<UserAssetsWasteBook> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(UserAssetsWasteBook data)
        {
            await InsertAsync(data);
        }
        #endregion

        #region 私有成员

        #endregion
    }
}