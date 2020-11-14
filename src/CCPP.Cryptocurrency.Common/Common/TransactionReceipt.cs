using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 交易凭证信息
    /// </summary>
    public class TransactionReceipt
    {
        /// <summary>
        /// 区块Hash值
        /// </summary>
        public string BlockHash { get; set; }
        /// <summary>
        /// 区块高度
        /// </summary>
        public long BlockHeight { get; set; }
        /// <summary>
        /// 合约地址
        /// </summary>
        public string ContractAddress { get; set; }
        /// <summary>
        /// 计算用的Gas值
        /// </summary>
        public decimal CumulativeGasUsed { get; set; }
        /// <summary>
        /// 发送地址
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 使用的Gas
        /// </summary>
        public decimal GasUsed { get; set; }
        /// <summary>
        /// 目标地址
        /// </summary>
        public string To { get; set; }
        /// <summary>
        /// 交易ID
        /// </summary>
        public string TXID { get; set; }
        /// <summary>
        /// 交易ID索引
        /// </summary>
        public long TransactionIndex { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

    }
}
