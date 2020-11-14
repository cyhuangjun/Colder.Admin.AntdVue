using Newtonsoft.Json;

namespace CCPP.Cryptocurrency.Common.ETH
{
    /// <summary>
    /// 获取代币余额
    /// </summary>
    public class TokenBalance
    {
        /// <summary>
        /// 指定合约地址
        /// </summary>
        [JsonProperty("to")]
        public string To { get; set; }
        /// <summary>
        /// 数据信息
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
