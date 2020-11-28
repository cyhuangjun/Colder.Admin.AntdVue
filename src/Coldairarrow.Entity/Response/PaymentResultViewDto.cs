using Coldairarrow.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Response
{
    public class PaymentResultViewDto
    { 
        public PaymentStatus Status { set; get; }

        public string PayAddress { set; get; }
        /// <summary>
        /// Actual amount
        /// </summary>
        public Decimal Amount { get; set; }
        /// <summary>
        /// cryptocurrency
        /// </summary>
        public string PayCurrency { set; get; }
        public string PaymentId { set; get; } 
        public string ClientUid { set; get; }
        public decimal Fee { set; get; }
        public string TXID { set; get; }
        public string FromAddress { set; get; }
    }
}
