using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    public class CreateWalletAddressInput
    {
        public string Tag { get; set; }
        public string SecurityKey { get; set; }
        public bool IsActive { get; set; }
        /// <summary>
        /// 种子信息
        /// </summary>
        public string Seed { get; set; }
    }
}
