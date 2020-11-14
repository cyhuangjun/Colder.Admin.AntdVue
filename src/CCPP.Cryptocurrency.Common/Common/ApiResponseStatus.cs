using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// Api响应状态
    /// </summary>
    public enum ApiResponseStatus
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,
        /// <summary>
        /// 种子无效
        /// </summary>
        SeedInvalid = 1,
        /// <summary>
        /// 时间戳过期
        /// </summary>
        TimestampInvalid = 2,
        /// <summary>
        /// 解析数据失败
        /// </summary>
        DataParseError = 3,
        /// <summary>
        /// Hash无效
        /// </summary>
        HashInvalid = 4,
        /// <summary>
        /// RPC失败
        /// </summary>
        RPCError = 5
    }
}
