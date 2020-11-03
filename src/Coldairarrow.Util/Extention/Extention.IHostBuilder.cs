﻿using CacheManager.Core;
using CSRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Coldairarrow.Util
{
    /// <summary>
    /// 拓展类
    /// </summary>
    public static partial class Extention
    {
        /// <summary>
        /// 使用IdHelper
        /// </summary>
        /// <param name="hostBuilder">建造者</param>
        /// <returns></returns>
        public static IHostBuilder UseIdHelper(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((buidler, services) =>
            {
                new IdHelperBootstrapper()
                    //设置WorkerId
                    .SetWorkderId(buidler.Configuration["WorkerId"].ToLong())
                    //使用Zookeeper
                    //.UseZookeeper("127.0.0.1:2181", 200, GlobalSwitch.ProjectName)
                    .Boot();
            });

            return hostBuilder;
        }

        /// <summary>
        /// 使用缓存
        /// </summary>
        /// <param name="hostBuilder">建造者</param>
        /// <returns></returns>
        public static IHostBuilder UseCache(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((buidlerContext, services) =>
            {
                var cacheOption = buidlerContext.Configuration.GetSection("Cache").Get<CacheOption>();
                switch (cacheOption.CacheType)
                {
                    case CacheType.Memory: services.AddDistributedMemoryCache(); break;
                    case CacheType.Redis:
                        {
                            var csredis = new CSRedisClient(cacheOption.RedisEndpoint);
                            RedisHelper.Initialization(csredis);
                            services.AddSingleton(csredis);
                            services.AddSingleton<IDistributedCache>(new CSRedisCache(RedisHelper.Instance));
                            services.AddCacheManagerConfiguration(ConfigHelper.Configuration, cfg => cfg.WithMicrosoftLogging(services))
                                    .AddCacheManager<int>(ConfigHelper.Configuration, configure: builder => builder.WithJsonSerializer())
                                    .AddCacheManager<DateTime>(inline => inline.WithDictionaryHandle())
                                    .AddCacheManager(); 
                        }; break;
                    default: throw new Exception("缓存类型无效");
                }
            });

            return hostBuilder;
        }

        class CacheOption
        {
            public CacheType CacheType { get; set; }
            public string RedisEndpoint { get; set; }
        }
    }
}
