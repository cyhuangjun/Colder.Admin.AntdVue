using Coldairarrow.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Response
{
    public class PaymentViewDto
    {  
        public string PayAddress { set; get; } 
        /// <summary>
        /// min Pay Amount
        /// </summary>
        public decimal MinPayAmount { set; get; }
        /// <summary>
        /// cryptocurrency
        /// </summary>
        public string PayCurrency { set; get; }
        /// <summary>
        /// UID
        /// </summary>
        public string UID { set; get; }

        /// <summary>
        /// Fixed fee for deposit
        /// </summary>
        public Decimal? FixFee { get; set; }

        /// <summary>
        /// Deposit fee rate
        /// </summary>
        public Decimal? FeeRate { get; set; }

        /// <summary>
        /// Deposit fee type
        /// </summary>
        public FeeMode? FeeType { get; set; }

        /// <summary>
        /// Minimum deposit fee
        /// </summary>
        public decimal? MinFee { get; set; }
    }
}
