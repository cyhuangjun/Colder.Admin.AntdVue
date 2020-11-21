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
            var result = await this._db.GetIQueryable<Coin>().Where(e =>e.IsAvailable).ToListAsync();
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
    }
}
