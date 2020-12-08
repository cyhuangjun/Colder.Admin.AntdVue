using Coldairarrow.Entity.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Foundation
{
    /// <summary>
    /// CoinConfig
    /// </summary>
    [Table("CoinConfig")]
    public class CoinConfig
    {

        /// <summary>
        /// ID
        /// </summary>
        [Key, Column(Order = 1)]
        public String Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public String CoinID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [Display(Name = "名称")]
        public String Caption { get; set; }

        /// <summary>
        /// MinerFee
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal? MinerFee { get; set; }

        /// <summary>
        /// MinerFeeRate
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal? MinerFeeRate { get; set; }

        /// <summary>
        /// MinerFeeModeType
        /// </summary>
        [Required]
        public FeeMode MinerFeeModeType { get; set; }

        /// <summary>
        /// 最小充币
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal CoinInMinAmount { get; set; }

        /// <summary>
        /// 最小提币
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal CoinOutMinAmount { get; set; }

        /// <summary>
        /// 最大提币
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal CoinOutMaxAmount { get; set; }

        /// <summary>
        /// 充币手续费
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal? CoinInHandlingFee { get; set; }

        /// <summary>
        /// 充币手续费率
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal? CoinInHandlingFeeRate { get; set; }

        /// <summary>
        /// 充币费用类型，1固定，2比例
        /// </summary>
        [Required]
        public FeeMode CoinInHandlingFeeModeType { get; set; }

        /// <summary>
        /// CoinInHandlingMinFee
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal? CoinInHandlingMinFee { get; set; }

        /// <summary>
        /// 提币手续费
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal? CoinOutHandlingFee { get; set; }

        /// <summary>
        /// 提币手续费率
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal? CoinOutHandlingFeeRate { get; set; }

        /// <summary>
        /// CoinOutHandlingFeeModeType
        /// </summary>
        [Required]
        public FeeMode CoinOutHandlingFeeModeType { get; set; }

        /// <summary>
        /// CoinOutHandlingMinFee
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal? CoinOutHandlingMinFee { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        [Required]
        [Display(Name = "默认")]
        public Boolean IsDefault { get; set; }

        /// <summary>
        /// CreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// CreatorID
        /// </summary>
        public String CreatorId { get; set; }

        /// <summary>
        /// LastUpdateTime
        /// </summary>
        public DateTime? LastUpdateTime { get; set; }

        /// <summary>
        /// LastUpdateUserID
        /// </summary>
        public String LastUpdateUserId { get; set; }

    }
}