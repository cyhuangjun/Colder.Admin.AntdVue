using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Transaction
{
    /// <summary>
    /// 站点资产
    /// </summary>
    [Table("UserAssets")]
    public class UserAssets
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, Column(Order = 1)]
        public String Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String UserID { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public String CoinID { get; set; }

        /// <summary>
        ///  
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal TotalAmount { get; set; }

        /// <summary>
        /// 冻结数量
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal FrozenAmount { get; set; }

        /// <summary>
        /// Balance
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal Balance { get; set; }

        /// <summary>
        /// RowVersion
        /// </summary>
        [Timestamp]
        public DateTime RowVersion { get; set; }

    }
}