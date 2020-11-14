using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 地址信息
    /// </summary>
    public class AddressInfo
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey { get; set; }
        /// <summary>
        /// 私钥
        /// </summary>
        public string PrivateKey { get; set; }
    }
}
