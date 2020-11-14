using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Foundation
{
    /// <summary>
    /// CoinSynchronizationBlock
    /// </summary>
    [Table("CoinSynchronizationBlock")]
    public class CoinSynchronizationBlock
    {

        /// <summary>
        /// ID
        /// </summary>
        [Key, Column(Order = 1)]
        public String Id { get; set; }

        /// <summary>
        /// ConfirmCount
        /// </summary>
        public int ConfirmCount { get; set; }

        /// <summary>
        /// LastBlockHeight
        /// </summary>
        public int LastBlockHeight { get; set; }

        /// <summary>
        /// LastUpdateTime
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

    }
}