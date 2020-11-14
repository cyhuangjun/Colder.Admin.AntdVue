using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coldairarrow.Entity.Request
{
    public class PaymentRequest
    {
        /// <summary>
        /// (required) - price in Fiat currency.
        /// </summary>
        [Required]
        public decimal PriceAmount { set; get; }
        /// <summary>
        /// (required) - Fiat currency.
        /// </summary>
        [Required]
        public string  PriceCurrency { set; get; }
        /// <summary>
        /// (optional) - price in cryptocurrency.
        /// </summary>
        public decimal? PayAmount { set; get; }
        /// <summary>
        /// (required) - cryptocurrency. 
        /// </summary>
        [Required]
        public string PayCurrency { set; get; }
        /// <summary>
        /// (optional) - url to receive callbacks, should contain "http" or "https", eg. "https://xxx.xx"
        /// </summary>
        public string CallbackUrl { set; get; }
        /// <summary>
        ///  (required) - clientId
        /// </summary>
        public string UID { set; get; }
        /// <summary>
        ///  (optional) - inner store order ID, e.g. "RGDBP-21314"
        /// </summary>
        public string OrderId { set; get; }
        /// <summary>
        ///  (optional) - inner store order description, e.g. "Apple Macbook Pro 2019 x 1"
        /// </summary>
        public string OrderDescription { set; get; }
        /// <summary>
        /// (optional) - id of purchase for which you want to create aother payment, only used for several payments for one order
        /// </summary>
        public string PurchaseId { set; get; }
        ///// <summary>
        ///// (optional) - in case you want to receive funds on an external address, you can specify it in this parameter
        ///// </summary>
        //public string PayoutAddress { set; get; }
        ///// <summary>
        /////  (optional) - currency of your external payout_address, required when payout_adress is specified.
        ///// </summary>
        //public string PayoutCurrency { set; get; }
        ///// <summary>
        ///// (optional) - extra id or memo or tag for external payout_address.
        ///// </summary>
        //public string PayoutExtraId { set; get; }
    }
}
