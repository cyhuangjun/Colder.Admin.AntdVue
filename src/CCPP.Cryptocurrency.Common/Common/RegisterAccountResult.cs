using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 激活账号
    /// </summary>
    public class RegisterAccountResult
    {
        /// <summary>
        /// 交易ID
        /// </summary>
        public string TX { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Fee { get; set; }
    }
}
