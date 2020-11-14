using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    public class BalanceData
    {
        public string Address { get; set; }
        public string TokenContractAddress { get; set; }
        public bool IsRelationCoin { get; set; }
        /// <summary>
        /// 符号
        /// </summary>
        public string Symbol { get; set; }
        public string Seed { get; set; }
        public int Index { get; set; }
        public bool IsMultiSigAddress { set; get; }
    }
}
