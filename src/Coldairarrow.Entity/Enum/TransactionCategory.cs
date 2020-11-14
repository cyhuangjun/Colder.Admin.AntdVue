using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Enum
{
    public enum TransactionCategory
    {
        /// <summary>
        /// 挖矿所得
        /// </summary>
        Generate = 1,
        /// <summary>
        /// 发送
        /// </summary>
        Send = 2,
        /// <summary>
        /// 收到
        /// </summary>
        Receive = 3,
        /// <summary>
        /// 未成熟
        /// </summary>
        Immature = 5,
        /// <summary>
        /// 未知类型
        /// </summary>
        UnKnow =6,

        Orphan = 7
    }
}
