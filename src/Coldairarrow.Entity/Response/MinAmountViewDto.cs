using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Response
{
    public class MinAmountViewDto
    {
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { set; get; }
        /// <summary>
        /// 最小充币
        /// </summary>
        public decimal MinPaymentAmount { set; get; }
        /// <summary>
        /// 最小提币
        /// </summary>
        public decimal MinTransferAmount { set; get; } 
        /// <summary>
        /// 最大提币
        /// </summary>
        public decimal MaxTransferAmount { set; get; }
    }
}
