using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 区块信息
    /// </summary>
    public class CoinBlock
    {
        /// <summary>
        /// 区块hash值
        /// </summary>
        public string Hash { get; set; }
        /// <summary>
        /// 确认数
        /// </summary>
        public Int64 Confirmations { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public Int64 Size { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public Int64 Height { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }
        public string MerkleRoot { get; set; }
        /// <summary>
        /// 包含的交易ID
        /// </summary>
        public List<string> Tx { get; set; }
        /// <summary>
        /// 产生时间
        /// </summary>
        public Int64 Time { get; set; }
        /// <summary>
        /// Nonce值
        /// </summary>
        public Int64 Nonce { get; set; }
        /// <summary>
        /// 难度随机值
        /// </summary>
        public string Bits { get; set; }
        /// <summary>
        /// 难度值
        /// </summary>
        public double Difficulty { get; set; }
        public string Chainwork { get; set; }
        /// <summary>
        /// 上一个区块的Hash值
        /// </summary>
        public string PreviousblockHash { get; set; }
        /// <summary>
        /// 下一个区块的Hash值
        /// </summary>
        public string NextblockHash { get; set; }
        /// <summary>
        /// 交易记录信息
        /// </summary>
        public List<CoinTransaction> Transactions { get; set; }
        /// <summary>
        /// 锲约相关的交易
        /// </summary>
        public List<ContractTransaction> ContractTransactions { get; set; }

    }
}
