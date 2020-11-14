using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 未花费交易信息
    /// </summary>
    public class UnspentTransaction
    {
        /// <summary>
        /// 交易ID
        /// </summary>
        public string TXID { get; set; }
        /// <summary>
        /// 输出索引
        /// </summary>
        public int VOut { get; set; }
        /// <summary>
        /// 钱包地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 钱包地址标签
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 脚本信息
        /// </summary>
        public string ScriptPubKey { get; set; }
        /// <summary>
        /// 未花费数量
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 确认数
        /// </summary>
        public int Confirmations { get; set; }

        public bool Spendable { get; set; }
    }
}
