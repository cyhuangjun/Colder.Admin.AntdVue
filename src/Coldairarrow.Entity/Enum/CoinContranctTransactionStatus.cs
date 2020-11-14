using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Coldairarrow.Entity.Enum
{
    /// <summary>
    /// 虚拟币契约相关交易状态
    /// </summary>
    public enum CoinContranctTransactionStatus
    {
        /// <summary>
        /// 待同步
        /// </summary>
        [Description("待同步")]
        WaitSync = 1,
        /// <summary>
        /// 已经同步
        /// </summary>
        [Description("已经同步")]
        Synchronized = 2,
        /// <summary>
        /// 无效
        /// </summary>
        [Description("无效")]
        Invalid = 3
    }
}
