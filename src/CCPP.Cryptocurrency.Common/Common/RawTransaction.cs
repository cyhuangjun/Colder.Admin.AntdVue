using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 原始交易信息
    /// </summary>
    public class RawTransaction
    {
        /// <summary>
        /// 交易ID
        /// </summary>
        public string TXId { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }
        public long Size { get; set; }
        public long VSize { get; set; }
        /// <summary>
        /// 锁定时间
        /// </summary>
        public int LockTime { get; set; }
        /// <summary>
        /// 区块Hash值
        /// </summary>
        public string BlockHash { get; set; }
        /// <summary>
        /// 确认数
        /// </summary>
        public int Confirmations { get; set; }
        /// <summary>
        /// 收到时间
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// 区块产生时间
        /// </summary>
        public int Blocktime { get; set; }
        /// <summary>
        /// 输入交易详细信息
        /// </summary>
        public List<RawTransactionDetail> VOut { get; set; }
        /// <summary>
        /// 输出交易详细信息
        /// </summary>
        public List<RawTransactionDetail> VIn { get; set; }
    }
}
