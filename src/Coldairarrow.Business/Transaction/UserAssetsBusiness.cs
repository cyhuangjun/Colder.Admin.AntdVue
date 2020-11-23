using CacheManager.Core;
using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Business.Foundation;
using Coldairarrow.Entity;
using Coldairarrow.Entity.DTO;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using EFCore.Sharding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public class UserAssetsBusiness : BaseBusiness<UserAssets>, IUserAssetsBusiness, ITransientDependency
    {
        const string RedisLockAssetsKey = "LockAssets";
        IDbAccessor _db;
        ICacheManager<object> _cache;
        ICoinBusiness _coinBusiness;
        IBase_UserBusiness _base_UserBusiness;
        public UserAssetsBusiness(IDbAccessor db,
                                ICacheManager<object> cache,
                                ICoinBusiness coinBusiness,
                                IBase_UserBusiness base_UserBusiness) : base(db)
        {
            _db = db;
            _cache = cache;
            _coinBusiness = coinBusiness;
            _base_UserBusiness = base_UserBusiness;
        }

        #region 外部接口

        public async Task<decimal> GetBalance(string userId, string coinCode)
        {
            var coin = await this._coinBusiness.GetCoinByCodeAsync(coinCode);
            if (coin == null) return 0m;
            var cacheKey = BuildCacheKey(userId, coin.Id);
            if (this._cache.Exists(cacheKey))
            {
                var result = this._cache.Get(cacheKey).ChangeType<decimal>();
                return result;
            }
            else
            {
                var userAssets = await this.Db.GetIQueryable<UserAssets>().FirstOrDefaultAsync(e => e.UserID == userId && e.CoinID == coinCode);
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
                List<UserAssets> inserts = new List<UserAssets>();
                List<UserAssets> updates = new List<UserAssets>();
                List<UserAssetsWasteBook> wasteBooks = new List<UserAssetsWasteBook>();
                foreach (var item in assetsChangeItems)
                {
                    if (item.UserID.IsNullOrEmpty() || item.CoinID.IsNullOrEmpty())
                    {
                        result.ErrorCode = ErrorCodeDefine.ParameterInvalid;
                        result.Msg = "parameter invalid";
                        return result;
                    }
                    var wasteBook = new UserAssetsWasteBook()
                    {
                        ID = Guid.NewGuid().GuidTo16String(),
                        AssetsWasteBookType = item.AssetsWasteBookType,
                        CoinID = item.CoinID,
                        UserID = item.UserID,
                        Remarks = item.Remark,
                        RelateBusinessID = item.RelateID,
                        CreateTime = DateTime.Now,
                        ChangeAmount = item.ChangeAvailableAmount,
                        ChangeFrozenAmount = item.ChangeFreeAmount, 
                    };
                    var userassets = await this.Db.GetIQueryable<UserAssets>().Where(e => e.UserID == item.UserID && e.CoinID == item.CoinID).FirstOrDefaultAsync();
                    if (userassets == null)
                    {
                        userassets = new UserAssets()
                        {
                            Id = Guid.NewGuid().GuidTo16String(),
                            CoinID = item.CoinID,
                            UserID = item.UserID,
                            Balance = item.ChangeAvailableAmount,
                            FrozenAmount = item.ChangeFreeAmount,
                            TotalAmount = (item.ChangeAvailableAmount + item.ChangeFreeAmount),
                        };
                        inserts.Add(userassets); 
                    }
                    else
                    {
                        wasteBook.OriginalBalance = userassets.Balance;
                        wasteBook.OriginalFrozenAmount = userassets.FrozenAmount;

                        userassets.Balance += item.ChangeAvailableAmount;
                        userassets.FrozenAmount += item.ChangeFreeAmount;
                        userassets.TotalAmount = (userassets.Balance + userassets.FrozenAmount);
                        updates.Add(userassets); 
                    } 
                    if (userassets.Balance < 0 || userassets.FrozenAmount < 0)
                    {
                        result.ErrorCode = ErrorCodeDefine.AssetsNotEnought;
                        result.Msg = "assets not enought.";
                        return result;
                    }

                    wasteBook.Balance = userassets.Balance;
                    wasteBook.FrozenAmount = userassets.FrozenAmount;
                    wasteBooks.Add(wasteBook);

                    var cacheKey = BuildCacheKey(item.UserID, item.CoinID);
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
            if (string.IsNullOrEmpty(assetsChangeItem.UserID))
            {
                result.ErrorCode = ErrorCodeDefine.UserIDRequired;
                result.Msg = "userid required";
                return result;
            }
            if (string.IsNullOrEmpty(assetsChangeItem.RelateID))
            {
                result.ErrorCode = ErrorCodeDefine.RelateIDRequired;
                result.Msg = "relateid required";
                return result;
            }
            var user = await this._base_UserBusiness.GetTheDataAsync(assetsChangeItem.UserID);
            if(user == null)
            {
                result.ErrorCode = ErrorCodeDefine.UserIDNotExist;
                result.Msg = "userid not exist.";
                return result;
            }
            assetsChangeItem.ChangeAvailableAmount = Math.Round(assetsChangeItem.ChangeAvailableAmount, GlobalData.CurrencyPrecision);
            assetsChangeItem.ChangeFreeAmount = Math.Round(assetsChangeItem.ChangeFreeAmount, GlobalData.CurrencyPrecision); 
            if (assetsChangeItem.ChangeAvailableAmount == 0 && assetsChangeItem.ChangeFreeAmount == 0)
            {
                result.ErrorCode = ErrorCodeDefine.AssetsChangeAmountZero;
                result.Msg = "assets changeamount zero";
                return result;
            }
            result.Success = true;
            result.ErrorCode = ErrorCodeDefine.Success;
            return result;
        }
        #endregion

        #region 私有成员
        private string BuildCacheKey(string userId, string coinId)
        {
            var assetsCacheKey = string.Format("Cache_Assets_Balance_{0}_{1}", userId, coinId);
            return assetsCacheKey;
        }
        #endregion
    }
}