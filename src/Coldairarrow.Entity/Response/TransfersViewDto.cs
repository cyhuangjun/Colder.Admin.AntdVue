using Coldairarrow.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Response
{
    public class TransfersViewDto
    {
        public string WithdrawId { set; get; }
        public string CurrencyFrom { set; get; }

        public decimal AmountFrom { set; get; }

        public string CurrencyTo { set; get; }

        public decimal AmountTo { set; get; }

        public string AddressTo { set; get; }
        /// <summary>
        /// (optional) - url to receive callbacks, should contain "http" or "https", eg. "https://xxx.xx"
        /// </summary>
        public string CallbackUrl { set; get; }
        /// <summary>
        ///  (optional) - inner store order ID, e.g. "RGDBP-21314"
        /// </summary>
        public string OrderId { set; get; }
        /// <summary>
        ///  (optional) - inner store order description, e.g. "Apple Macbook Pro 2019 x 1"
        /// </summary>
        public string OrderDescription { set; get; }

        public TransfersStatus Status { set; get; }
    }
}
