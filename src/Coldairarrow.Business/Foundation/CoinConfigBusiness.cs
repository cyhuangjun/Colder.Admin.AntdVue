using CacheManager.Core;
using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.Entity.Response;
using Coldairarrow.IBusiness.Core;
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
                                  ICacheDataBusiness cacheDataBusiness) : base(db)
        {
            _base_UserBusiness = base_UserBusiness;
            _cache = cache;
            _cacheDataBusiness = cacheDataBusiness;
        }

        #region 外部接口

        public async Task<PageResult<CoinConfigDTO>> GetDataListAsync(PageInput<ConditionDTO> input)
        { 
            var where = LinqHelper.True<CoinConfigDTO>();
            Expression<Func<CoinConfig, Coin, CoinConfigDTO>> select = (a, b) => new CoinConfigDTO
            {
                Currency = b.Code,
                IsDefaultStr = (a.IsDefault? "是":"否")
            };
            var search = input.Search;
            select = select.BuildExtendSelectExpre();
            var q = from a in GetIQueryable().AsExpandable()
                    join b in Db.GetIQueryable<Coin>() on a.CoinID equals b.Id into ab
                    from b in ab.DefaultIfEmpty()
                    select @select.Invoke(a, b); 

            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<CoinConfigDTO, bool>(
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
            var result = new MinAmountViewDto() { Currency = currency };
            var coin = await this._cacheDataBusiness.GetCoinByCodeAsync(currency);
            if (coin == null) return result;
            string cacheKey = $"Cache_{GetType().FullName}_MinAmountAsync_{tenantId}_{currency}";
            if (this._cache.Exists(cacheKey))
            {
                var cache = this._cache.Get(cacheKey).ChangeType<MinAmountViewDto>();
                return await Task.FromResult(cache);
            } 
            var tenantCoinConfig = await this.Db.GetIQueryable<TenantCoinConfig>().Where(e => e.TenantId == tenantId && e.CoinID == coin.Id).FirstOrDefaultAsync();
            if (tenantCoinConfig != null)
            { 
                if(!string.IsNullOrEmpty(tenantCoinConfig.CoinConfigID))
                {
                    var coinConfig = await this.GetTheDataAsync(tenantCoinConfig.CoinConfigID);
                    result.MinPaymentAmount = coinConfig.CoinInMinAmount;
                    result.MinTransferAmount = coinConfig.CoinOutMinAmount;
                    result.MaxTransferAmount = coinConfig.CoinOutMaxAmount;
                }
                if (tenantCoinConfig.CoinInMinAmount.HasValue)
                    result.MinPaymentAmount = tenantCoinConfig.CoinInMinAmount.Value;
                if (tenantCoinConfig.CoinOutMinAmount.HasValue)
                    result.MinTransferAmount = tenantCoinConfig.CoinOutMinAmount.Value;
                if (tenantCoinConfig.CoinOutMaxAmount.HasValue)
                    result.MaxTransferAmount = tenantCoinConfig.CoinOutMaxAmount.Value;
            }
            else
            {
                var coinConfig = await this.Db.GetIQueryable<CoinConfig>().Where(e => e.CoinID == coin.Id && e.IsDefault).FirstOrDefaultAsync();
                if (coinConfig != null)
                {
                    result.MinPaymentAmount = coinConfig.CoinInMinAmount;
                    result.MinTransferAmount = coinConfig.CoinOutMinAmount;
                    result.MaxTransferAmount = coinConfig.CoinOutMaxAmount;
                }
            }
            if (result != null)
                this._cache.Add(cacheKey, result);
            return result;
        }

        public async Task<CoinConfig> GetEntityAsync(string tenantId, string currency)
        {
            var coin = await this._cacheDataBusiness.GetCoinByCodeAsync(currency);
            if (coin == null) return null;
            var tenant = await this._cacheDataBusiness.GetTenantAsync(tenantId);
            var tenantCoinConfig = await this.Db.GetIQueryable<TenantCoinConfig>().Where(e => e.TenantId == tenantId && e.CoinID == coin.Id).FirstOrDefaultAsync();
            var coinConfigId = tenantCoinConfig?.CoinConfigID;
            var coinConfig = await this.GetEntityAsync(e => e.IsDefault && e.CoinID == coin.Id);
            if (!string.IsNullOrEmpty(coinConfigId))
                coinConfig = await this.GetTheDataAsync(coinConfigId);
            return coinConfig;
        }
        #endregion

        #region 私有成员

        #endregion
    }
}