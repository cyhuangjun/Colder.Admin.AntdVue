using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Report
{
    /// <summary>
    /// StatisticalDayReport
    /// </summary>
    [Table("StatisticalDayReport")]
    public class StatisticalDayReport
    {

        /// <summary>
        /// Id
        /// </summary>
        [Key, Column(Order = 1)]
        public String Id { get; set; }

        /// <summary>
        /// 来源站点ID
        /// </summary>
        public String TenantId { get; set; }

        /// <summary>
        /// 交易品种ID
        /// </summary>
        public String CoinID { get; set; }

        /// <summary>
        /// CreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// TotalRechargeQty
        /// </summary>
        public Int32 TotalRechargeQty { get; set; }

        /// <summary>
        /// TotalWithdrawalQty
        /// </summary>
        public Int32 TotalWithdrawalQty { get; set; }

        /// <summary>
        /// TotalRechargeAmount
        /// </summary>
        public Decimal TotalRechargeAmount { get; set; }

        /// <summary>
        /// TotalWithdrawalAmount
        /// </summary>
        public Decimal TotalWithdrawalAmount { get; set; }

    }
}