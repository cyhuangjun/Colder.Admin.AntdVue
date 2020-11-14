using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 推荐费用请求参数
    /// </summary>
    public class RecommendedParam
    {
        /// <summary>
        /// 货币数
        /// </summary>
        public decimal Amount { get; set; } = 0;

        /// <summary>
        /// 当前货币
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 发送地址，如果有，就找当前用户的地址，如果没有就去一个临时的地址
        /// </summary>
        public string SendToAddress { get; set; } = "";

    }
}
