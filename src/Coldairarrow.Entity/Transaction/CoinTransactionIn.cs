using Coldairarrow.Entity.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Transaction
{
    /// <summary>
    /// CoinTransactionIn
    /// </summary>
    [Table("CoinTransactionIn")]
    public class CoinTransactionIn
    {

        /// <summary>
        /// ID
        /// </summary>
        [Key, Column(Order = 1)]
        public String ID { get; set; }

        /// <summary>
        /// Account
        /// </summary>
        public String Account { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal Amount { get; set; }

        /// <summary>
        /// Confirmations
        /// </summary>
        public Int32 Confirmations { get; set; }

        /// <summary>
        /// TXID
        /// </summary>
        public String TXID { get; set; }

        /// <summary>
        /// BlockIndex
        /// </summary>
        public Int32 BlockIndex { get; set; }

        /// <summary>
        /// BlockTime
        /// </summary>
        public DateTime BlockTime { get; set; }

        /// <summary>
        /// Timereceived
        /// </summary>
        public DateTime Timereceived { get; set; }

        /// <summary>
        /// UserID
        /// </summary>
        public String UserID { get; set; }

        /// <summary>
        /// CoinID
        /// </summary>
        public String CoinID { get; set; }

        /// <summary>
        /// CreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// LastUpdateTime
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public TransactionStatus Status { get; set; }

        /// <summary>
        /// FromAddress
        /// </summary>
        public String FromAddress { get; set; }

        public string SysToUserTXID { set; get; }

        public string UserToSysTXID { set; get; }

        [Column(TypeName = "decimal(28, 16)")]
        public string MoveToAddress { set; get; }

        public decimal MoveAmount { set; get; }

        public DateTime? MoveTime { set; get; }

        public string MinefeeCoinID { set; get; }

        /// <summary>
        /// 消耗矿工费
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public decimal MoveUserMinefee { set; get; }
        /// <summary>
        /// 消耗矿工费
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public decimal MoveSysMinefee { set; get; }
        /// <summary>
        /// 预留矿工费
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public decimal ReserveMinerfees { set; get; } 

        public MoveStatus MoveStatus { set; get; }
    }
}