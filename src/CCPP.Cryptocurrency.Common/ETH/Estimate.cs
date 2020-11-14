using Newtonsoft.Json;

namespace CCPP.Cryptocurrency.Common.ETH
{
    /// <summary>
    /// 评估矿工费相关信息
    /// </summary>
    public class Estimate
    {
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("to")]
        public string To { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("asPrice")]
        public string AsPrice { get; set; }
    }
}
