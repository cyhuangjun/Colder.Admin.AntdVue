using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 发送虚拟币参数接口
    /// </summary>
    public class SendCoinData
    {
        /// <summary>
        /// 地址或标签
        /// </summary>
        public string FromAccount { get; set; }
        /// <summary>
        /// 目标地址
        /// </summary>
        public string ToCoinAddress { get; set; }
        /// <summary>
        /// 目标地址标签
        /// </summary>
        public string ToCoinAddressTag { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 发送账号密码
        /// </summary>
        public string FromAccountPassword { get; set; }
          
        /// <summary>
        /// 原始交易的 16 进制字符串(BTC)
        /// </summary>
        public string HexString { get; set; }

        public IList<Tuple<string, string>> ETHMultiSigResult { set; get; }
    }
}
