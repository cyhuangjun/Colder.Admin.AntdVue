using CacheManager.Core;
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
    public class CoinBusiness : BaseBusiness<Coin>, ICoinBusiness, ITransientDependency
    {
        ICacheManager<object> _cache;
        public CoinBusiness(IDbAccessor db,
                            ICacheManager<object> cache)
            : base(db)
        {
            _cache = cache;
        }

        #region 外部接口
        public async Task<AjaxResult<IEnumerable<string>>> GetCryptoCurrenciesAsync()
        {
            try
            {
                IEnumerable<string> result;
                result = await this.GetIQueryable(e => e.IsAvailable).Select(e => e.Code).ToListAsync();
                return this.Success(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new AjaxResult<IEnumerable<string>>() { Success = false, ErrorCode = 500, Msg = e.Message };
            }
        }

        public async Task<PageResult<Coin>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<Coin>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<Coin, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<Coin> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(Coin data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(Coin data)
        {
            await UpdateAsync(data);
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        public async Task<Coin> GetCoinByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code)) return null;
            string cacheKey = $"Cache_{GetType().FullName}_GetCoinByCodeAsync_{code}";
            if (this._cache.Exists(cacheKey))
            {
                var cache = this._cache.Get(cacheKey).ChangeType<Coin>();
                return await Task.FromResult(cache);
            }
            var result = await GetIQueryable().FirstOrDefaultAsync(e => e.Code == code && e.IsAvailable);
            if(result != null)
                this._cache.Add(cacheKey, result);
            return result;
        }

        public Task<decimal> GetFeeRate(string userId, string coinId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 私有成员

        #endregion
    }
}