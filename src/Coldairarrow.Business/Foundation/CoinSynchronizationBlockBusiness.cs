using Coldairarrow.Entity.Foundation;
using Coldairarrow.Util;
using EFCore.Sharding;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Foundation
{
    public class CoinSynchronizationBlockBusiness : BaseBusiness<CoinSynchronizationBlock>, ICoinSynchronizationBlockBusiness, ITransientDependency
    {
        public CoinSynchronizationBlockBusiness(IDbAccessor db)
            : base(db)
        {
        }

        #region 外部接口

        public async Task<PageResult<CoinSynchronizationBlock>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<CoinSynchronizationBlock>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<CoinSynchronizationBlock, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<CoinSynchronizationBlock> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(CoinSynchronizationBlock data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(CoinSynchronizationBlock data)
        {
            await UpdateAsync(data);
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        public async Task<IEnumerable<CoinSynchronizationBlock>> GetDataListAsync(IEnumerable<string> syncCoinIDs)
        {
            var result = this.GetList(e => syncCoinIDs.Contains(e.Id));
            var exceptSyncBlocks = syncCoinIDs.Except(result.Select(e => e.Id)).Select(e => new CoinSynchronizationBlock()
                                                                                                {
                                                                                                    Id = e,
                                                                                                    ConfirmCount = 0,
                                                                                                    LastBlockHeight = 0,
                                                                                                    LastUpdateTime = DateTime.Now,
                                                                                                });
            if (exceptSyncBlocks.Any())
            {
                await this.Db.InsertAsync(exceptSyncBlocks);
                result.AddRange(exceptSyncBlocks);
            }
            return result;
        }

        #endregion

        #region 私有成员

        #endregion
    }
}