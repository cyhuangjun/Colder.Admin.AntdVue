using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Coldairarrow.Entity.Enum
{
    /// <summary>
    /// 手续费类型
    /// </summary>
    public enum FeeMode
    {
        /// <summary>
        /// 固定
        /// </summary>
        [Description("固定")]
        Fixed = 1,
        /// <summary>
        /// 比例
        /// </summary>
        [Description("比例")]
        Proportion = 2
    }
}
