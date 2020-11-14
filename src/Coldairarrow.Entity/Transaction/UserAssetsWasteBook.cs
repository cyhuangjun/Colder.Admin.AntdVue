using Coldairarrow.Entity.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Transaction
{
    /// <summary>
    /// 站点资产流水
    /// </summary>
    [Table("UserAssetsWasteBook")]
    public class UserAssetsWasteBook
    {

        /// <summary>
        /// ID
        /// </summary>
        [Key, Column(Order = 1)]
        public String ID { get; set; }

        /// <summary>
        /// 站点
        /// </summary>
        public String UserID { get; set; }

        /// <summary>
        /// 交易品种ID
        /// </summary>
        public String CoinID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 关联单号
        /// </summary>
        public String RelateBusinessID { get; set; }

        /// <summary>
        /// 资产流水类型
        /// </summary>
        public AssetsWasteBookType AssetsWasteBookType { get; set; }

        /// <summary>
        /// 原始可用数量
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal OriginalBalance { get; set; }

        /// <summary>
        /// 变动数量，包含手续费
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal ChangeAmount { get; set; }

        /// <summary>
        /// 最新余额
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal Balance { get; set; }

        /// <summary>
        /// 原始冻结数量
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal OriginalFrozenAmount { get; set; }

        /// <summary>
        /// 变动冻结数量
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal ChangeFrozenAmount { get; set; }

        /// <summary>
        /// 最新冻结数量
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal FrozenAmount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public String Remarks { get; set; }

    }
}