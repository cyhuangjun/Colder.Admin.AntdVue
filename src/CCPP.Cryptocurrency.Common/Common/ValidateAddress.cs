using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 校验钱包地址信息
    /// </summary>
    public class ValidateAddress
    {
        /// <summary>
        /// 是否合法
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 钱包地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 是否是自己的
        /// </summary>
        public bool IsMine { get; set; }
        /// <summary>
        /// 是否是脚本
        /// </summary>
        public bool IsScript { get; set; }
        /// <summary>
        /// 公钥
        /// </summary>
        public string Pubkey { get; set; }
        /// <summary>
        /// 是否压缩
        /// </summary>
        public bool IsCompressed { get; set; }
        /// <summary>
        /// 账户
        /// </summary>
        public string Account { get; set; }
    }
}
