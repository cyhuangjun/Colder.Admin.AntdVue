using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    public class ScriptPubKey
    {
        public string Asm { get; set; }
        public string Hex { get; set; }
        public string ReqSigs { get; set; }
        public string Type { get; set; }
        public List<string> Addresses { get; set; }
    }
    /// <summary>
    /// 交易详细信息
    /// </summary>
    public class RawTransactionDetail
    {
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Value { get; set; }
        public int N { get; set; }
        /// <summary>
        /// 脚本信息
        /// </summary>
        public ScriptPubKey ScriptPubKey { get; set; }
        /// <summary>
        /// 交易ID
        /// </summary>
        public string TXID { get; set; }
        /// <summary>
        /// 输出索引
        /// </summary>
        public int VOut { get; set; }
    }
}
