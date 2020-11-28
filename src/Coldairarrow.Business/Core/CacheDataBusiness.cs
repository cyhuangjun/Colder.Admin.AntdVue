using CacheManager.Core;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Util;
using EFCore.Sharding;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Coldairarrow.Entity.Base_Manage;

namespace Coldairarrow.Business.Core
{
    public class CacheDataBusiness : ICacheDataBusiness, ITransientDependency
    {
        IDbAccessor _db;
        ICacheManager<object> _cache;

        public CacheDataBusiness(IDbAccessor db,
                                ICacheManager<object> cache)
        {
            this._db = db;
            this._cache = cache;
        }

        public async Task<List<Coin>> GetCoinsAsync()
        {
            string cacheKey = $"Cache_{GetType().FullName}_GetCoins_";
            if (this._cache.Exists(cacheKey))
            {
                var cache = this._cache.Get(cacheKey).ChangeType<List<Coin>>();
                return await Task.FromResult(cache);
            }
            var result = await this._db.GetIQueryable<Coin>().Where(e => e.IsAvailable).ToListAsync();
            if (result != null)
                this._cache.Add(cacheKey, result);
            return result;
        }

        public async Task<Coin> GetCoinAsync(string coinId)
        {
            string cacheKey = $"Cache_{GetType().FullName}_GetCoin_{coinId}_";
            if (this._cache.Exists(cacheKey))
            {
                var cache = this._cache.Get(cacheKey).ChangeType<Coin>();
                return await Task.FromResult(cache);
            }
            var result = await this._db.GetIQueryable<Coin>().Where(e => e.Id == coinId).FirstOrDefaultAsync();
            if (result != null)
                this._cache.Add(cacheKey, result);
            return result;
        }

        public async Task<Base_User> GetUserAsync(string userId)
        {
            string cacheKey = $"Cache_{GetType().FullName}_GetUserAsync_{userId}_";
            if (this._cache.Exists(cacheKey))
            {
                var cache = this._cache.Get(cacheKey).ChangeType<Base_User>();
                return await Task.FromResult(cache);
            }
            var result = await this._db.GetIQueryable<Base_User>().Where(e => e.Id == userId).FirstOrDefaultAsync();
            if (result != null)
                this._cache.Add(cacheKey, result);
            return result;
        }

        public async Task<Wallet> GetWalletByAddress(string address)
        {
            string cacheKey = $"Cache_{GetType().FullName}_GetWalletByAddress_{address}_";
            if (this._cache.Exists(cacheKey))
            {
                var cache = this._cache.Get(cacheKey).ChangeType<Wallet>();
                return await Task.FromResult(cache);
            }
            var result = await this._db.GetIQueryable<Wallet>().Where(e => e.Address == address).FirstOrDefaultAsync();
            if (result != null)
                this._cache.Add(cacheKey, result);
            return result;
        }

        public async Task<Wallet> GetWallet(string userId, string clientUid, string coinId)
        {
            string cacheKey = $"Cache_{GetType().FullName}_GetWallet_{userId}_{clientUid}_{coinId}_";
            if (this._cache.Exists(cacheKey))
            {
                var cache = this._cache.Get(cacheKey).ChangeType<Wallet>();
                return await Task.FromResult(cache);
            }
            var result = await this._db.GetIQueryable<Wallet>().Where(e => e.UserID == userId && e.UID == clientUid && e.CoinID == coinId).FirstOrDefaultAsync();
            if (result != null)
                this._cache.Add(cacheKey, result);
            return result;
        }
    }
}