using Newtonsoft.Json;

namespace CCPP.Cryptocurrency.Common.ETH
{
    /// <summary>
    /// ETH交易信息
    /// </summary>
    public class TransactionInfo
    {
        /// <summary>
        /// 区块Hash值
        /// </summary>
        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("blockNumber")]
        public string BlockNumber { get; set; }
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("gas")]
        public string Gas { get; set; }
        [JsonProperty("gasPrice")]
        public string GasPrice { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }
        [JsonProperty("input")]
        public string Input { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        [JsonProperty("to")]
        public string To { get; set; }
        [JsonProperty("transactionIndex")]
        public string TransactionIndex { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("v")]
        public string V { get; set; }
        [JsonProperty("r")]
        public string R { get; set; }
        [JsonProperty("s")]
        public string S { get; set; }

    }
}