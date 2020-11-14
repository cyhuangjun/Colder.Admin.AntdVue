using Coldairarrow.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Response
{
    public class PaymentViewDto
    {
        public string PaymentId { set; get; }

        public PaymentStatus Status { set; get; }

        public string PayAddress { set; get; }
        /// <summary>
        /// price in Fiat currency
        /// </summary>
        public decimal PriceAmount { set; get; }
        /// <summary>
        /// - Fiat currency 
        /// </summary>
        public string PriceCurrency { set; get; }
        /// <summary>
        /// price in cryptocurrency
        /// </summary>
        public decimal PayAmount { set; get; }
        /// <summary>
        /// cryptocurrency
        /// </summary>
        public string PayCurrency { set; get; }
        /// <summary>
        /// inner store order ID, e.g. "RGDBP-21314"
        /// </summary>
        public string OrderId { set; get; }
        /// <summary>
        /// inner store order ID, e.g. "RGDBP-21314"
        /// </summary>
        public string UID { set; get; }
        /// <summary>
        /// inner store order description, e.g. "Apple Macbook Pro 2019 x 1"
        /// </summary>
        public string OrderDescription { set; get; }
        /// <summary>
        /// url to receive callbacks, should contain "http" or "https"
        /// </summary>
        public string CallbackUrl { set; get; }

        public DateTime CreatedAt{set;get;}

        public DateTime UpdatedAt { set; get; }
        /// <summary>
        /// id of purchase for which you want to create aother payment, only used for several payments for one order
        /// </summary>
        public string PurchaseId { set; get; }
    }
}
