using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coldairarrow.Entity.Request
{
    public class PaymentRequest
    {
        /// <summary>
        /// (required) - cryptocurrency. 
        /// </summary>
        [Required]
        public string PayCurrency { set; get; }
        /// <summary>
        ///  (required) - clientId
        /// </summary>
        public string UID { set; get; }
    }
}
