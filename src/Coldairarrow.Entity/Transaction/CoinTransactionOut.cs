using Coldairarrow.Entity.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Transaction
{
    /// <summary>
    /// CoinTransactionOut
    /// </summary>
    [Table("CoinTransactionOut")]
    public class CoinTransactionOut
    {

        /// <summary>
        /// ID
        /// </summary>
        [Key, Column(Order = 1)]
        public String Id { get; set; }

        /// <summary>
        /// UserID
        /// </summary>
        public String UserID { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// CoinID
        /// </summary>
        public String CoinID { get; set; }

        /// <summary>
        /// Quantity
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal Amount { get; set; }

        /// <summary>
        /// TXID
        /// </summary>
        public String TXID { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public TransactionStatus Status { get; set; }

        /// <summary>
        /// ApproveComments
        /// </summary>
        public String ApproveComments { get; set; }

        /// <summary>
        /// BlockTime
        /// </summary>
        public DateTime? BlockTime { get; set; }

        /// <summary>
        /// ApproverID
        /// </summary>
        public String ApproverID { get; set; }  
        /// <summary>
        /// AddressTag
        /// </summary>
        public String AddressTag { get; set; }

        /// <summary>
        /// Hexstring
        /// </summary>
        public String Hexstring { get; set; }

        public string MinefeeCoinID { set; get; }

        public decimal Minefee { set; get; }

        /// <summary>
        /// SignedResult1
        /// </summary>
        public String SignedResult1 { get; set; }

        /// <summary>
        /// SignedResult2
        /// </summary>
        public String SignedResult2 { get; set; }

        /// <summary>
        /// SignedResult3
        /// </summary>
        public String SignedResult3 { get; set; }

        /// <summary>
        /// MultiSigUserName1
        /// </summary>
        public String MultiSigUserName1 { get; set; }

        /// <summary>
        /// MultiSigUserName2
        /// </summary>
        public String MultiSigUserName2 { get; set; }

        /// <summary>
        /// MultiSigUserName3
        /// </summary>
        public String MultiSigUserName3 { get; set; }

        /// <summary>
        /// MultiSigTime1
        /// </summary>
        public DateTime? MultiSigTime1 { get; set; }

        /// <summary>
        /// MultiSigTime2
        /// </summary>
        public DateTime? MultiSigTime2 { get; set; }

        /// <summary>
        /// MultiSigTime3
        /// </summary>
        public DateTime? MultiSigTime3 { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public int? Version { get; set; }

        /// <summary>
        /// BatchApprovalID
        /// </summary>
        public String BatchApprovalID { get; set; }

        /// <summary>
        /// CreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }

        public DateTime? ApproveTime { set; get; }

        /// <summary>
        /// LastUpdateUserID
        /// </summary>
        public String LastUpdateUserID { get; set; }

        /// <summary>
        /// LastUpdateTime
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}