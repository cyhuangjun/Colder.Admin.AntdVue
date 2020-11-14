using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common.ETH
{
    /// <summary>
    /// 交易凭证信息
    /// </summary>
    public class TransactionReceiptLog
    {
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }
        [JsonProperty("blockNumber")]
        public string BlockNumber { get; set; }
        [JsonProperty("data")]
        public string Data { get; set; }
        [JsonProperty("logIndex")]
        public string LogIndex { get; set; }
        [JsonProperty("removed")]
        public bool Removed { get; set; }
        [JsonProperty("topics")]
        public List<string> Topics { get; set; }
        [JsonProperty("transactionHash")]
        public string TransactionHash { get; set; }
        [JsonProperty("transactionIndex")]
        public string TransactionIndex { get; set; }
    }
}
