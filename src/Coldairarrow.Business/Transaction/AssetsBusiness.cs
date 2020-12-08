using CacheManager.Core;
using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Business.Foundation;
using Coldairarrow.Entity;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Entity.DTO;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.Entity.Transaction;
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

namespace Coldairarrow.Business.Transaction
{
    public class AssetsBusiness : BaseBusiness<Assets>, IAssetsBusiness, ITransientDependency
    {
        const string RedisLockAssetsKey = "LockAssets";
        IDbAccessor _db;
        ICacheManager<object> _cache;
        ICoinBusiness _coinBusiness;
        IBase_UserBusiness _base_UserBusiness;
        ICacheDataBusiness _cacheDataBusiness;
        public AssetsBusiness(IDbAccessor db,
                                ICacheManager<object> cache,
                                ICoinBusiness coinBusiness,
                                IBase_UserBusiness base_UserBusiness,
                                ICacheDataBusiness cacheDataBusiness) : base(db)
        {
            _db = db;
            _cache = cache;
            _coinBusiness = coinBusiness;
            _base_UserBusiness = base_UserBusiness;
            _cacheDataBusiness = cacheDataBusiness;
        }

        #region 外部接口
        public async Task<PageResult<AssetsOutDTO>> GetDataListAsync(PageInput<AssetsInputDTO> input)
        {
            Expression<Func<Assets, Coin, Base_Department, AssetsOutDTO>> select = (a, b, c) => new AssetsOutDTO
            {
                Currency = b.Code,
                Tenant = c.Name
            };
            select = select.BuildExtendSelectExpre();
            var q = from a in this.Db.GetIQueryable<Assets>().AsExpandable()
                    join b in Db.GetIQueryable<Coin>() on a.CoinID equals b.Id into ab
                    from b in ab.DefaultIfEmpty()
                    join c in Db.GetIQueryable<Base_Department>() on a.TenantId equals c.Id into ac
                    from c in ac.DefaultIfEmpty()
                    select @select.Invoke(a, b, c);
            //筛选
            var where = LinqHelper.True<AssetsOutDTO>();
            var search = input.Search;
            if (!string.IsNullOrEmpty(search.CoinID))
                where = where.And(x => x.CoinID == search.CoinID);
            if (!string.IsNullOrEmpty(search.TenantId))
                where = where.And(x => x.TenantId == search.TenantId);
            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<Assets> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task<decimal> GetBalance(string tenantId, string coinId)
        {
            var coin = await this._cacheDataBusiness.GetCoinAsync(coinId);
            if (coin == null) return 0m; 
            var cacheKey = BuildCacheKey(tenantId, coin.Id);
            if (this._cache.Exists(cacheKey))
            {
                var result = this._cache.Get(cacheKey).ChangeType<decimal>();
                return result;
            }
            else
            {
                var userAssets = await this.Db.GetIQueryable<Assets>().FirstOrDefaultAsync(e => e.TenantId == tenantId && e.CoinID == coinId);
                var balance = userAssets?.Balance ?? 0;
                this._cache.Add(cacheKey, balance);
                return balance;
            }
        }

        [Transactional]
        public async Task<AjaxResult> UpdateAssets(params AssetsChangeItemDTO[] assetsChangeItems)
        { 
            AjaxResult result = new AjaxResult();
            if (assetsChangeItems == null || !assetsChangeItems.Any())
            {
                result.ErrorCode = ErrorCodeDefine.ParameterInvalid;
                result.Msg = "parameter invalid";
                return result;
            }
            List<string> assetsCacheKeyList = new List<string>();
            foreach (var item in assetsChangeItems)
            {
                var checkResult = await CheckAssetsChangeItem(item);
                if (!checkResult.Success)
                    return checkResult;
            }
            var redis = RedisHelper.Instance;
            using (var srv = redis.Lock(RedisLockAssetsKey, 15))
            {
                if (srv == null)
                {
                    result.ErrorCode = ErrorCodeDefine.OperationTimeout;
                    result.Msg = "operation timeout.";
                    return result;
                }
                List<Assets> inserts = new List<Assets>();
                List<Assets> updates = new List<Assets>();
                List<AssetsWasteBook> wasteBooks = new List<AssetsWasteBook>();
                foreach (var item in assetsChangeItems)
                {
                    if (item.TenantId.IsNullOrEmpty() || item.CoinID.IsNullOrEmpty())
                    {
                        result.ErrorCode = ErrorCodeDefine.ParameterInvalid;
                        result.Msg = "parameter invalid";
                        return result;
                    } 
                    var wasteBook = new AssetsWasteBook()
                    {
                        ID = Guid.NewGuid().GuidTo16String(),
                        AssetsWasteBookType = item.AssetsWasteBookType,
                        CoinID = item.CoinID,
                        TenantId = item.TenantId,
                        Remarks = item.Remark,
                        RelateBusinessID = item.RelateID,
                        CreateTime = DateTime.Now,
                        ChangeAmount = item.ChangeAvailableAmount,
                        ChangeFrozenAmount = item.ChangeFrozenAmount, 
                    };
                    var assets = await this.Db.GetIQueryable<Assets>().Where(e => e.TenantId == item.TenantId && e.CoinID == item.CoinID).FirstOrDefaultAsync();
                    if (assets == null)
                    {
                        assets = new Assets()
                        {
                            Id = Guid.NewGuid().GuidTo16String(),
                            CoinID = item.CoinID,
                            TenantId = item.TenantId,
                            Balance = item.ChangeAvailableAmount,
                            FrozenAmount = item.ChangeFrozenAmount,
                            TotalAmount = (item.ChangeAvailableAmount + item.ChangeFrozenAmount),
                        };
                        inserts.Add(assets); 
                    }
                    else
                    {
                        wasteBook.OriginalBalance = assets.Balance;
                        wasteBook.OriginalFrozenAmount = assets.FrozenAmount;

                        assets.Balance += item.ChangeAvailableAmount;
                        assets.FrozenAmount += item.ChangeFrozenAmount;
                        assets.TotalAmount = (assets.Balance + assets.FrozenAmount);
                        updates.Add(assets); 
                    } 
                    if (assets.Balance < 0 || assets.FrozenAmount < 0)
                    {
                        result.ErrorCode = ErrorCodeDefine.AssetsNotEnought;
                        result.Msg = "assets not enought.";
                        return result;
                    }

                    wasteBook.Balance = assets.Balance;
                    wasteBook.FrozenAmount = assets.FrozenAmount;
                    wasteBooks.Add(wasteBook);

                    var cacheKey = BuildCacheKey(item.TenantId, item.CoinID);
                    this._cache.Remove(cacheKey);
                }
                if (inserts.Any())
                    await this.Db.InsertAsync(inserts); ;
                if (updates.Any())
                    await this.Db.UpdateAsync(updates);
                if (wasteBooks.Any())
                    await this.Db.InsertAsync(wasteBooks);
            }
            result.Success = true;
            result.ErrorCode = ErrorCodeDefine.Success;
            return result;
        }

        /// <summary>
        /// 检查资产变动信息是否有效
        /// </summary>
        /// <param name="assetsChangeItem">资产变动</param>
        /// <returns></returns>
        private async Task<AjaxResult> CheckAssetsChangeItem(AssetsChangeItemDTO assetsChangeItem)
        {
            AjaxResult result = new AjaxResult();
            if (string.IsNullOrEmpty(assetsChangeItem.CoinID))
            {
                result.ErrorCode = ErrorCodeDefine.AssetsIDRequired;
                result.Msg = "assets id required";
                return result;
            }
            if (string.IsNullOrEmpty(assetsChangeItem.TenantId))
            {
                result.ErrorCode = ErrorCodeDefine.TenantIDRequired;
                result.Msg = "TenantId required";
                return result;
            }
            if (string.IsNullOrEmpty(assetsChangeItem.RelateID))
            {
                result.ErrorCode = ErrorCodeDefine.RelateIDRequired;
                result.Msg = "relateid required";
                return result;
            }
            var tenant = await this._cacheDataBusiness.GetTenantByUserIDAsync(assetsChangeItem.TenantId);
            if(tenant == null)
            {
                result.ErrorCode = ErrorCodeDefine.TenantIDRequired;
                result.Msg = "tenantId not exist.";
                return result;
            }
            assetsChangeItem.ChangeAvailableAmount = Math.Round(assetsChangeItem.ChangeAvailableAmount, GlobalData.CurrencyPrecision);
            assetsChangeItem.ChangeFrozenAmount = Math.Round(assetsChangeItem.ChangeFrozenAmount, GlobalData.CurrencyPrecision); 
            if (assetsChangeItem.ChangeAvailableAmount == 0 && assetsChangeItem.ChangeFrozenAmount == 0)
            {
                result.ErrorCode = ErrorCodeDefine.AssetsChangeAmountZero;
                result.Msg = "assets changeamount zero";
                return result;
            }
            result.Success = true;
            result.ErrorCode = ErrorCodeDefine.Success;
            return result;
        }

        public async Task InitTenantAssets(string tenantId)
        {
            var coinIds = (await this._cacheDataBusiness.GetCoinsAsync()).Select(e => e.Id);
            var isExistedCoinIds = (await this.GetListAsync(e => e.TenantId == tenantId)).Select(e => e.CoinID);
            coinIds = coinIds.Except(isExistedCoinIds);
            foreach (var coinId in coinIds)
            {
                await CreateTenantAssets(tenantId, coinId);
            }
        }

        private async Task CreateTenantAssets(string tenantId, string coinId)
        {
            var assets = new Assets() { Id = Guid.NewGuid().GuidTo16String(), CoinID = coinId, TenantId = tenantId };
            await this.InsertAsync(assets);
        }
        #endregion

        #region 私有成员
        private string BuildCacheKey(string tenantId, string coinId)
        {
            var assetsCacheKey = string.Format("Cache_Assets_Balance_{0}_{1}", tenantId, coinId);
            return assetsCacheKey;
        }
        #endregion
    }
}