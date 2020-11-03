using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Response
{
    public class CurrencyEstimateViewDto
    {
        /// <summary>
        /// Conversion amount
        /// </summary>
        public decimal AmountFrom { set; get; }
        /// <summary>
        /// Original currency
        /// </summary>
        public string CurrencyFrom { set; get; }
        /// <summary>
        /// Target currency
        /// </summary>
        public string CurrencyTo { set; get; }
        /// <summary>
        /// Estimated amount
        /// </summary>
        public decimal EstimatedAmount { set; get; }
    }
}
