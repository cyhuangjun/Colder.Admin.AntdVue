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
using LinqKit;

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
            string cacheKey = $"Cache_{GetType().FullName}_GetCoinsAsync_";
            if (this._cache.Exists(cacheKey))
            {
                var cache = this._cache.Get(cacheKey).ChangeType<List<Coin>>();
                return await Task.FromResult(cache);
            }
            var result = await this._db.GetIQueryable<Coin>().AsExpandable().Where(e => e.IsAvailable).ToListAsync();
            if (result != null)
                this._cache.Add(cacheKey, result);
            return result;
        }

        public async Task<Coin> GetCoinAsync(string coinId)
        {
            string cacheKey = $"Cache_{GetType().FullName}_GetCoinAsync_{coinId}_";
            if (this._cache.Exists(cacheKey))
            {
                var cache = this._cache.Get(cacheKey).ChangeType<Coin>();
                return await Task.FromResult(cache);
            }
            var result = await this._db.GetIQueryable<Coin>().AsExpandable().Where(e => e.Id == coinId).FirstOrDefaultAsync();
            if (result != null)
                this._cache.Add(cacheKey, result);
            return result;
        }

        public async Task<Coin> GetCoinByCodeAsync(string code)
        {
            string cacheKey = $"Cache_{GetType().FullName}_GetCoinByCodeAsync_{code}_";
            if (this._cache.Exists(cacheKey))
            {
                var cache = this._cache.Get(cacheKey).ChangeType<Coin>();
                return await Task.FromResult(cache);
            }
            var result = await this._db.GetIQueryable<Coin>().AsExpandable().Where(e => e.Code == code).FirstOrDefaultAsync();
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
            var result = await this._db.GetIQueryable<Base_User>().AsExpandable().Where(e => e.Id == userId).FirstOrDefaultAsync();
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
            var result = await this._db.GetIQueryable<Wallet>().AsExpandable().Where(e => e.Address == address).FirstOrDefaultAsync();
            if (result != null)
                this._cache.Add(cacheKey, result);
            return result;
        }

        public async Task<Wallet> GetWallet(string tenantId, string clientUid, string coinId)
        {
            string cacheKey = $"Cache_{GetType().FullName}_GetWallet_{tenantId}_{clientUid}_{coinId}_";
            if (this._cache.Exists(cacheKey))
            {
                var cache = this._cache.Get(cacheKey).ChangeType<Wallet>();
                return await Task.FromResult(cache);
            }
            var result = await this._db.GetIQueryable<Wallet>().AsExpandable().Where(e => e.TenantId == tenantId && e.UID == clientUid && e.CoinID == coinId).FirstOrDefaultAsync();
            if (result != null)
                this._cache.Add(cacheKey, result);
            return result;
        }

        public async Task<Base_Department> GetTenantByUserIDAsync(string userId)
        {
            string cacheKey = $"Cache_{GetType().FullName}_GetTenantByUserIDAsync_{userId}_";
            if (this._cache.Exists(cacheKey))
            {
                var cache = this._cache.Get(cacheKey).ChangeType<Base_Department>();
                return await Task.FromResult(cache);
            }
            var user = await this.GetUserAsync(userId);
            var result = await this._db.GetIQueryable<Base_Department>().AsExpandable().Where(e => e.Id == user.DepartmentId).FirstOrDefaultAsync();
            if (result != null)
                this._cache.Add(cacheKey, result);
            return result;
        }

        public async Task<Base_Department> GetTenantAsync(string tenantId)
        {
            string cacheKey = $"Cache_{GetType().FullName}_GetTenantAsync_{tenantId}_";
            if (this._cache.Exists(cacheKey))
            {
                var cache = this._cache.Get(cacheKey).ChangeType<Base_Department>();
                return await Task.FromResult(cache);
            } 
            var result = await this._db.GetIQueryable<Base_Department>().AsExpandable().Where(e => e.Id == tenantId).FirstOrDefaultAsync();
            if (result != null)
                this._cache.Add(cacheKey, result);
            return result;
        }
    }
}