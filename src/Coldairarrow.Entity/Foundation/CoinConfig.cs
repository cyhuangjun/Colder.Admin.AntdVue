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
        public String ID { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public String Currency { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
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
        public FeeMode MinerFeeModeType { get; set; }

        /// <summary>
        /// 最小充币
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal CoinInMinAmount { get; set; }

        /// <summary>
        /// 最小提币
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal CoinOutMinAmount { get; set; }
        /// <summary>
        /// 最大提币
        /// </summary>
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
        public FeeMode CoinInHandlingFeeModeType { get; set; }

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
        public FeeMode CoinOutHandlingFeeModeType { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        public Boolean IsDefault { get; set; }

        /// <summary>
        /// FCreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// FCreatorID
        /// </summary>
        public String CreatorID { get; set; }

        /// <summary>
        /// FLastUpdateTime
        /// </summary>
        public DateTime? LastUpdateTime { get; set; }

        /// <summary>
        /// FLastUpdateUserID
        /// </summary>
        public String LastUpdateUserID { get; set; }

    }
}