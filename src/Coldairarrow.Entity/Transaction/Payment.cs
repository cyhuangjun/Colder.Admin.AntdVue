using Coldairarrow.Entity.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Transaction
{
    /// <summary>
    /// 转入平台虚拟币
    /// </summary>
    [Table("Payment")]
    public class Payment
    {

        /// <summary>
        /// ID
        /// </summary>
        [Key, Column(Order = 1)]
        public String Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public String UserID { get; set; }

        /// <summary>
        /// UID
        /// </summary>
        public String UID { get; set; }

        public string OrderId { set; get; }
        /// <summary>
        /// OrderDescription
        /// </summary>
        public String OrderDescription { get; set; }

        /// <summary>
        /// 手续费率
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal? FeeRate { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal? Fee { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public PaymentStatus Status { get; set; }

        /// <summary>
        /// PriceCurrency
        /// </summary>
        public String PriceCurrency { get; set; }

        /// <summary>
        /// PriceAmount
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal PriceAmount { get; set; }

        /// <summary>
        /// PayCurrency
        /// </summary>
        public String PayCurrency { get; set; }

        /// <summary>
        /// PayAmount
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal PayAmount { get; set; }

        /// <summary>
        /// PayAddress
        /// </summary>
        public String PayAddress { get; set; }
        /// <summary>
        /// 实际支付
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal ActuallyPaid { get; set; }

        /// <summary>
        /// 实际入账金额
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal ActualAmount { get; set; }

        /// <summary>
        /// 确认数
        /// </summary>
        public Int32? Confirmations { get; set; }

        /// <summary>
        /// 交易ID
        /// </summary>
        public String TXID { get; set; }

        /// <summary>
        /// PurchaseId
        /// </summary>
        public String PurchaseId { get; set; }

        /// <summary>
        /// CallbackUrl
        /// </summary>
        public String CallbackUrl { get; set; }

        /// <summary>
        /// CallBackStatus
        /// </summary>
        public APICallBackStatus? CallBackStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// ArrivalAt
        /// </summary>
        public DateTime? ArrivalAt { get; set; }

    }
}