using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Request
{
    /// <summary>
    /// gas price base
    /// </summary>
    public class GasPriceBaseInfo
    {
        /// <summary>
        /// 区块时间
        /// </summary>
        [JsonProperty("block_time")]
        public decimal BlockTime { get; set; }

        /// <summary>
        /// 最快
        /// </summary>
        [JsonProperty("fastest")]
        public decimal Fastest { get; set; }

        /// <summary>
        /// 快
        /// </summary>
        [JsonProperty("fast")]
        public decimal Fast { get; set; }

        /// <summary>
        /// 慢
        /// </summary>
        [JsonProperty("safeLow")]
        public decimal SafeLow { get; set; }

        /// <summary>
        /// 区块数
        /// </summary>
        [JsonProperty("blockNum")]
        public int BlockNum { get; set; }

    }

    /// <summary>
    /// Gas Price Infomation
    /// </summary>
    public class GasPriceInfo : GasPriceBaseInfo
    {
        /// <summary>
        /// 标准的
        /// </summary>
        [JsonProperty("standard")]
        public virtual decimal Standard { get; set; }
        /// <summary>
        /// 推荐值
        /// </summary>
        public decimal Recommend { get; set; }

    }

    /// <summary>
    /// Gas Price Infomation
    /// </summary>
    public class EthgasstationInfo : GasPriceBaseInfo
    {
        /// <summary>
        /// 平均值（标准的）
        /// </summary>
        [JsonProperty("average")]
        public decimal Standard { get; set; }

    }
}
