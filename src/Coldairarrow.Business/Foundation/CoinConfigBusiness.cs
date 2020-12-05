using CacheManager.Core;
using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.Entity.Response;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Util;
using EFCore.Sharding;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Foundation
{
    public class CoinConfigBusiness : BaseBusiness<CoinConfig>, ICoinConfigBusiness, ITransientDependency
    {
        readonly ICacheDataBusiness _cacheDataBusiness;
        readonly ICacheManager<object> _cache;
        readonly IBase_UserBusiness _base_UserBusiness;
        public CoinConfigBusiness(IDbAccessor db,
                                  IBase_UserBusiness base_UserBusiness,
                                  ICacheManager<object> cache,
                                  ICacheDataBusiness cacheDataBusiness)
            : base(db)
        {
            _base_UserBusiness = base_UserBusiness;
            _cache = cache;
            _cacheDataBusiness = cacheDataBusiness;
        }

        #region 外部接口
        public async Task<PageResult<CoinConfig>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<CoinConfig>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<CoinConfig, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<CoinConfig> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(CoinConfig data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(CoinConfig data)
        {
            await UpdateAsync(data);
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        public async Task<MinAmountViewDto> MinAmountAsync(string tenantId, string currency)
        { 
            currency = currency.ToUpper();
            string cacheKey = $"Cache_{GetType().FullName}_MinAmountAsync_{tenantId}_{currency}";
            if (this._cache.Exists(cacheKey))
            {
                var cache = this._cache.Get(cacheKey).ChangeType<MinAmountViewDto>();
                return await Task.FromResult(cache);
            }
            var result = new MinAmountViewDto() { Currency = currency };
            var userCoinConfig = await this.Db.GetIQueryable<TenantCoinConfig>().Where(e => e.TenantId == tenantId && e.Currency == currency).FirstOrDefaultAsync();
            if (userCoinConfig != null)
            {
                result.MinPaymentAmount = userCoinConfig.CoinInMinAmount;
                result.MinTransferAmount = userCoinConfig.CoinOutMinAmount;
            }
            else
            {
                var tenant = await this._cacheDataBusiness.GetTenantAsync(tenantId);
                if (!string.IsNullOrEmpty(tenant?.CoinConfigID))
                {
                    var coinConfig = await this.GetTheDataAsync(tenant.CoinConfigID);
                    result.MinPaymentAmount = coinConfig.CoinInMinAmount;
                    result.MinTransferAmount = coinConfig.CoinOutMinAmount;
                    result.MaxTransferAmount = coinConfig.CoinOutMaxAmount;
                }
                else
                {
                    var coinConfig = await this.Db.GetIQueryable<CoinConfig>().Where(e => e.Currency == currency && e.IsDefault).FirstOrDefaultAsync();
                    if (coinConfig != null)
                    {
                        result.MinPaymentAmount = coinConfig.CoinInMinAmount;
                        result.MinTransferAmount = coinConfig.CoinOutMinAmount;
                        result.MaxTransferAmount = coinConfig.CoinOutMaxAmount; 
                    }
                    else
                        result = null;
                }
            }
            if (result != null)
                this._cache.Add(cacheKey, result);
            return result;
        }

        public async Task<CoinConfig> GetEntityAsync(string tenantId, string currency)
        {
            var tenant = await this._cacheDataBusiness.GetTenantAsync(tenantId);
            var coinConfigId = tenant?.CoinConfigID;
            var coinConfig = await this.GetEntityAsync(e => e.IsDefault && e.Currency == currency);
            if (!string.IsNullOrEmpty(coinConfigId))
                coinConfig = await this.GetTheDataAsync(coinConfigId);
            return coinConfig;
        }
        #endregion

        #region 私有成员

        #endregion
    }
}