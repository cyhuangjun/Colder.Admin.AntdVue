using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    public class GenerateAddressResult
    {
        public string Address { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public string DisplayAddress { get; set; }
        /// <summary>
        /// 种子
        /// </summary>
        public string Seed { get; set; }
        public int Index { get; set; }
    }
}
