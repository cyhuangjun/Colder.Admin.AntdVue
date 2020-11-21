using CacheManager.Core;
using Coldairarrow.Entity.Request;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Util;
using Coldairarrow.Util.Helper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Coldairarrow.Business.Core
{
    public class EvaluateMinefeeRateBusiness : IEvaluateMinefeeRateBusiness, ITransientDependency
    {
        ICacheManager<object> _cache;
        ILogger<EvaluateMinefeeRateBusiness> _logger;
        public EvaluateMinefeeRateBusiness(ILogger<EvaluateMinefeeRateBusiness> logger,
                                ICacheManager<object> cache)
        {
            this._logger = logger;
            this._cache = cache;
        }

        public decimal GetBTCFeeRate()
        {
            var cacheKey = "_btc_fee_rate_";
            var result = this._cache.Get<Dictionary<string, decimal>>(cacheKey);
            string key = "fastestFee";
            if (result != null)
            {
                return result[key];
            }
            result = GetBtcRecommendedFeeRate();
            this._cache.Add(cacheKey, result);
            return result[key];
        }

        public GasPriceInfo GetETHGasPrice()
        {
            var key = "_eth_gas_price_";
            var gasPriceResult = this._cache.Get<GasPriceInfo>(key);
            if (gasPriceResult != null)
            {
                return gasPriceResult;
            }
            var url = "https://ethgasstation.info/json/ethgasAPI.json";
            var result = RestSharpHttpHelper.RestAction<EthgasstationInfo>(url, string.Empty);
            if (result.success)
            {
                gasPriceResult = new GasPriceInfo()
                {
                    BlockNum = result.data.BlockNum,
                    BlockTime = result.data.BlockTime,
                    Fast = result.data.Fast / 10m,
                    Fastest = result.data.Fastest / 10m,
                    SafeLow = result.data.SafeLow / 10m,
                    Standard = result.data.Standard / 10m
                };
                var priceMulity = (decimal)Math.Pow(10, 9);
                gasPriceResult.Fast = gasPriceResult.Fast * priceMulity;
                gasPriceResult.Fastest = gasPriceResult.Fastest * priceMulity;
                gasPriceResult.SafeLow = gasPriceResult.SafeLow * priceMulity;
                gasPriceResult.Standard = gasPriceResult.Standard * priceMulity;
                gasPriceResult.Recommend = gasPriceResult.Fast;
                this._cache.Add(key, gasPriceResult);
                return gasPriceResult;
            }
            throw new Exception($"GetETHGasPrice 错误. {result.error}");
        }

        /// <summary>
        /// 比特币推荐费率
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, decimal> GetBtcRecommendedFeeRate()
        {
            var fastestFee = 250;
            var halfHourFee = 200;
            var hourFee = 80;
            Dictionary<string, decimal> feeRate = new Dictionary<string, decimal>();
            var url = "https://bitcoinfees.earn.com/api/v1/fees/recommended";
            var result = RestSharpHttpHelper.RestAction<Dictionary<string, decimal>>(url, string.Empty);
            if (result.success)
                feeRate = result.data;
            else
            {
                feeRate = new Dictionary<string, decimal>();
                feeRate.Add("fastestFee", fastestFee);
                feeRate.Add("halfHourFee", halfHourFee);
                feeRate.Add("hourFee", hourFee);
            }
            return feeRate;
        }


    }
}
