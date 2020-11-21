using Coldairarrow.Entity.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Request
{
    public class SendCoinInfo
    {
        public string ToAddress { get; set; }
        public string ToAddressTag { get; set; }
        public decimal Quantity { get; set; }
        /// <summary>
        /// 矿工费比例
        /// </summary>
        public decimal MinerFeeRate { get; set; }
        public string OldTXID { get; set; }
        public Coin Coin { get; set; }
        public Coin ParentCoin { get; set; }
        public decimal GasPrice { get; set; }
        public decimal EstimateGas { set; get; } 
    }
}
