using Coldairarrow.Entity.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Transaction
{
    /// <summary>
    /// （转出虚拟币）提币
    /// </summary>
    [Table("Transfers")]
    public class Transfers
    {

        /// <summary>
        /// 主键
        /// </summary>
        [Key, Column(Order = 1)]
        public String Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public String UserID { get; set; }

        /// <summary>
        /// OrderId
        /// </summary>
        public String OrderId { get; set; }

        /// <summary>
        /// OrderDescription
        /// </summary>
        public String OrderDescription { get; set; }

        /// <summary>
        /// 状态（1申请,2审批成功,3处理,4完成，5审批失败，6处理失败）
        /// </summary>
        public TransfersStatus Status { get; set; }

        /// <summary>
        /// CurrencyFrom
        /// </summary>
        public String CurrencyFrom { get; set; }

        /// <summary>
        /// AmountFrom
        /// </summary>
        public Decimal AmountFrom { get; set; }

        /// <summary>
        /// CurrencyTo
        /// </summary>
        public String CurrencyTo { get; set; }

        /// <summary>
        /// AmountTo
        /// </summary>
        public Decimal AmountTo { get; set; }

        /// <summary>
        /// AddressTo
        /// </summary>
        public String AddressTo { get; set; }

        /// <summary>
        /// 货币手续费率
        /// </summary>
        public Decimal? FeeRate { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        public Decimal? Fee { get; set; }

        /// <summary>
        /// 钱包交易ID
        /// </summary>
        public String TXID { get; set; }

        /// <summary>
        /// ApproverID
        /// </summary>
        public String ApproverID { get; set; }

        /// <summary>
        /// ApproveTime
        /// </summary>
        public DateTime? ApproveTime { get; set; }

        /// <summary>
        /// CallbackUrl
        /// </summary>
        public String CallbackUrl { get; set; }

        /// <summary>
        /// CallBackStatus
        /// </summary>
        public Int32? CallBackStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 最后更新用户ID
        /// </summary>
        public String LastUpdateUserID { get; set; }

    }
}