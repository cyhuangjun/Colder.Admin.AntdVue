using Newtonsoft.Json;
using System.Collections.Generic;

namespace CCPP.Cryptocurrency.Common.ETH
{
    /// <summary>
    /// ETH交易凭证信息
    /// </summary>
    public class ETHTransactionReceipt
    {
        /// <summary>
        /// 区块Hash值
        /// </summary>
        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }
        /// <summary>
        /// 区块高度
        /// </summary>
        [JsonProperty("blockNumber")]
        public string BlockNumber { get; set; }
        /// <summary>
        /// 合约地址
        /// </summary>
        [JsonProperty("contractAddress")]
        public string ContractAddress { get; set; }
        /// <summary>
        /// 计算用的Gas值
        /// </summary>
        [JsonProperty("cumulativeGasUsed")]
        public string CumulativeGasUsed { get; set; }
        /// <summary>
        /// 发送地址
        /// </summary>
        [JsonProperty("from")]
        public string From { get; set; }
        /// <summary>
        /// 使用的Gas
        /// </summary>
        [JsonProperty("gasUsed")]
        public string GasUsed { get; set; }
        /// <summary>
        /// 目标地址
        /// </summary>
        [JsonProperty("to")]
        public string To { get; set; }
        /// <summary>
        /// 交易ID
        /// </summary>
        [JsonProperty("transactionHash")]
        public string TransactionHash { get; set; }
        /// <summary>
        /// 交易ID索引
        /// </summary>
        [JsonProperty("transactionIndex")]
        public string TransactionIndex { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("logs")]
        public List<TransactionReceiptLog> Logs { get; set; }


    }
}
