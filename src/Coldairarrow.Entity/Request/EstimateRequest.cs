using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coldairarrow.Entity.Request
{
    public class EstimateRequest
    {
        /// <summary>
        /// Conversion amount
        /// </summary>
        [Required]
        public decimal Amount { set; get; }
        /// <summary>
        /// Original currency
        /// </summary>
        [Required]
        public string CurrencyFrom { set; get; }
        /// <summary>
        /// Target currency
        /// </summary>
        [Required]
        public string CurrencyTo { set; get; }
    }
}
