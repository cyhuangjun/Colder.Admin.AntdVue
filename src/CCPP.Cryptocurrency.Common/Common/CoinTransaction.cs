using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 交易信息
    /// </summary>
    public class CoinTransaction
    {
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 地址标签
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 确认数
        /// </summary>
        public Int64 Confirmations { get; set; }
        /// <summary>
        /// 是否是挖币所得
        /// </summary>
        public bool Generated { get; set; }
        /// <summary>
        /// 区块Hash值
        /// </summary>
        public string Blockhash { get; set; }
        /// <summary>
        /// 区块索引
        /// </summary>
        public Int64 Blockindex { get; set; }
        /// <summary>
        /// 产生区块时间戳
        /// </summary>
        public Int64 BlockTime { get; set; }
        /// <summary>
        /// 交易ID
        /// </summary>
        public string TXId { get; set; }
        /// <summary>
        /// 冲突信息
        /// </summary>
        public List<string> WalletconFlicts { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public Int64 Time { get; set; }
        /// <summary>
        /// 收到时间
        /// </summary>
        public Int64 TimeReceived { get; set; }
        /// <summary>
        /// 来源于哪个地址
        /// </summary>
        public string FromAddress { get; set; }
        /// <summary>
        /// 交易手续费
        /// </summary>
        public decimal Fee { get; set; }
        /// <summary>
        /// 契约交易信息
        /// </summary>
        public ContractTransaction ContractTransaction { get; set; }
        /// <summary>
        /// Gas价格
        /// </summary>
        public decimal GasPrice { get; set; }
        /// <summary>
        /// 使用的Gas
        /// </summary>
        public decimal Gas { get; set; }
        /// <summary>
        /// 随机Nonce值
        /// </summary>
        public decimal Nonce { get; set; }
        /// <summary>
        /// 代币交易信息
        /// </summary>
        public List<TokenTransaction> TokenTransactions { get; set; }
        /// <summary>
        /// 是否启用代币信息,如果为true,只使用代币记录进行操作,否则启用主交易信息
        /// </summary>
        public bool IsUseTokenTransaction { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Valid { get; set; }

    }
}
