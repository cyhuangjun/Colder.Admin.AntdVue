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
        public Coin Coin { get; set; }
    }
}
