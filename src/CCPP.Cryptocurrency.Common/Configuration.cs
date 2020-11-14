using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    public class Configuration
    {
        /// <summary>
        /// 主机
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// RPC 用户
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// RPC 密码
        /// </summary>
        public string Password { get; set; } 
        /// <summary>
        /// 钱包精度
        /// </summary>
        public int? Precision { get; set; }
        /// <summary>
        /// 第三方访问接口
        /// </summary>
        public string ThridApiUrl { get; set; }
        /// <summary>
        /// 合约信息
        /// </summary>
        public string Contract { get; set; }
        /// <summary>
        /// 系统地址信息
        /// </summary>
        public AddressInfo SysAddress { get; set; } 

        public string ApiUrl { get; set; }

        public string ApiSecurityKey { get; set; }
        public string WalletSecurityKey { get; set; }
        /// <summary>
        /// 多签地址
        /// </summary>
        public Tuple<string, string> MultiSigAddress { get; set; }
    }
}
