using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Request
{
    public class TransfersRequest
    {
        public string CurrencyFrom { set; get; }

        public decimal AmountFrom { set; get; }

        public string CurrencyTo { set; get; }

        public string AddressTo { set; get; }
        /// <summary>
        ///  (optional) - inner store order ID, e.g. "RGDBP-21314"
        /// </summary>
        public string OrderId { set; get; }
        /// <summary>
        ///  (optional) - inner store order description, e.g. "Apple Macbook Pro 2019 x 1"
        /// </summary>
        public string OrderDescription { set; get; }
    }
}
