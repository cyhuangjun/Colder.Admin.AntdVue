using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Coldairarrow.Entity.Enum
{
    /// <summary>
    /// API回调状态
    /// </summary>
    public enum APICallBackStatus
    {
        /// <summary>
        /// 未回调
        /// </summary>
        [Description("未回调")]
        None,
        /// <summary>
        /// 回调成功
        /// </summary>
        [Description("回调成功")]
        Succeeded,
        /// <summary>
        /// 回调失败
        /// </summary>
        [Description("回调失败")]
        Failed, 
        /// <summary>
        /// 未配置回调
        /// </summary>
        [Description("未配置回调")]
        NotCallBackConfigured, 
    }
}
