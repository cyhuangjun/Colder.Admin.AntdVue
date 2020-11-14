using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 锲约相关交易信息
    /// </summary>
    public class ContractTransaction
    {
        /// <summary>
        /// 区块高度
        /// </summary>
        public int BlockHeight { get; set; }
        /// <summary>
        /// 区块Hash值
        /// </summary>
        public string BlockHash { get; set; }
        /// <summary>
        /// 交易ID
        /// </summary>
        public string TXID { get; set; }
        /// <summary>
        /// 收币地址
        /// </summary>
        public string ToAddress { get; set; }
        /// <summary>
        /// 发币地址
        /// </summary>
        public string FromAddress { get; set; }
        /// <summary>
        /// 锲约方法名称
        /// </summary>
        public string ContractMethod { get; set; }
        /// <summary>
        /// 合约地址
        /// </summary>
        public string ContractAddress { get; set; }
        /// <summary>
        /// 输入的数据信息
        /// </summary>
        public string Input { get; set; }

    }
}
