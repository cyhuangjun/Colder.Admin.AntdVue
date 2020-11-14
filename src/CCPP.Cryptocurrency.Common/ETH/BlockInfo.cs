using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common.ETH
{
    /// <summary>
    /// 区块相关信息
    /// </summary>
    public class BlockInfo
    {
        /// <summary>
        /// 难度
        /// </summary>
        [JsonProperty("difficulty")]
        public string Difficulty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("extraData")]
        public string ExtraData { get; set; }
        /// <summary>
        /// 本块所能消耗的gas，该值决定了能打包的交易数量
        /// </summary>
        [JsonProperty("gasLimit")]
        public string GasLimit { get; set; }
        /// <summary>
        /// 所有交易gas总和，必须小于等于gasLimit
        /// </summary>
        [JsonProperty("gasUsed")]
        public string GasUsed { get; set; }
        /// <summary>
        /// 块Hash值
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; set; }
        /// <summary>
        /// 日志过滤器，pending状态时为null
        /// </summary>
        [JsonProperty("logsBloom")]
        public string LogsBloom { get; set; }
        /// <summary>
        /// 挖到该块的矿工帐号地址
        /// </summary>
        [JsonProperty("miner")]
        public string Miner { get; set; }
        [JsonProperty("mixHash")]
        public string MixHash { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        /// <summary>
        /// 块号
        /// </summary>
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("parentHash")]
        public string ParentHash { get; set; }
        [JsonProperty("receiptsRoot")]
        public string ReceiptsRoot { get; set; }
        [JsonProperty("sha3Uncles")]
        public string Sha3Uncles { get; set; }
        /// <summary>
        /// 块所在空间，单位：byte
        /// </summary>
        [JsonProperty("size")]
        public string Size { get; set; }
        [JsonProperty("stateRoot")]
        public string StateRoot { get; set; }

        /// <summary>
        /// 块生成时的时间戳
        /// </summary>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        /// <summary>
        /// 本块和以前所有块难度之和
        /// </summary>
        [JsonProperty("totalDifficulty")]
        public string TotalDifficulty { get; set; }

        /// <summary>
        /// 所有交易的Hash数组
        /// </summary>
        [JsonProperty("transactions")]
        public List<TransactionInfo> Transactions { get; set; }
        [JsonProperty("transactionsRoot")]
        public string TransactionsRoot { get; set; }
        /// <summary>
        /// 所有叔块的Hash数组
        /// </summary>
        [JsonProperty("uncles")]
        public List<string> Uncles { get; set; }
    }
}
