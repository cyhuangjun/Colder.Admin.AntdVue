using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common.ETH
{
    /// <summary>
    /// 发送交易数据信息
    /// </summary>
    public class SendTransaction
    {
        /// <summary>
        /// 发送账号
        /// </summary>
        [JsonProperty("from")]
        public string From { get; set; }
        /// <summary>
        /// 发送账号
        /// </summary>
        [JsonProperty("to")]
        public string To { get; set; }
        /// <summary>
        /// 发送金额,单位wei
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
        /// <summary>
        /// Gas数量
        /// </summary>
        [JsonProperty("gas")]
        public string Gas { get; set; }
        /// <summary>
        /// Gas 价格
        /// </summary>
        [JsonProperty("gasPrice")]
        public string GasPrice { get; set; }
        /// <summary>
        /// 其它附带信息
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }
        /// <summary>
        /// Nonce
        /// </summary>
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
    }
}
