using Coldairarrow.Entity.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Transaction
{
    /// <summary>
    /// CoinContranctTransactionIn
    /// </summary>
    [Table("CoinContranctTransactionIn")]
    public class CoinContranctTransactionIn
    {

        /// <summary>
        /// ID
        /// </summary>
        [Key, Column(Order = 1)]
        public String ID { get; set; }

        /// <summary>
        /// 虚拟币ID
        /// </summary>
        public String CoinID { get; set; }

        /// <summary>
        /// 区块高度
        /// </summary>
        public Int32 BlockHeight { get; set; }

        /// <summary>
        /// BlockHash
        /// </summary>
        public String BlockHash { get; set; }

        /// <summary>
        /// 交易ID
        /// </summary>
        public String TXID { get; set; }

        /// <summary>
        /// 目标地址
        /// </summary>
        public String ToAddress { get; set; }

        /// <summary>
        /// 来源地址
        /// </summary>
        public String FromAddress { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CoinContranctTransactionStatus Status { get; set; }

        /// <summary>
        /// ContractMethod
        /// </summary>
        public String ContractMethod { get; set; }

        /// <summary>
        /// ContractAddress
        /// </summary>
        public String ContractAddress { get; set; }

        /// <summary>
        /// Input
        /// </summary>
        public String Input { get; set; }

        /// <summary>
        /// CreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// LastUpdateTime
        /// </summary>
        public DateTime? LastUpdateTime { get; set; }

    }
}