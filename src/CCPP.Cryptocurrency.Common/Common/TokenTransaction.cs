using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 代币交易信息
    /// </summary>
    public class TokenTransaction
    {
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 虚拟币空间
        /// </summary>
        public string CoinNamespace { get; set; }
        /// <summary>
        /// 虚拟币标识信息
        /// </summary>
        public string CoinKey { get; set; }
        /// <summary>
        /// 是否是主币种
        /// </summary>
        public bool IsMainCoin { get; set; }
        /// <summary>
        /// 发送地址
        /// </summary>
        public string FromAddress { get; set; }
        /// <summary>
        /// 接受地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 地址账号
        /// </summary>
        public string Account { get; set; }
        public bool IsSuccess { get; set; }
    }
}