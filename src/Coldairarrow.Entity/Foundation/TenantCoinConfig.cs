﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Foundation
{
    /// <summary>
    /// UserCoinConfig
    /// </summary>
    [Table("TenantCoinConfig")]
    public class TenantCoinConfig
    {

        /// <summary>
        /// ID
        /// </summary>
        [Key, Column(Order = 1)]
        public String Id { get; set; }

        /// <summary>
        /// TenantId
        /// </summary>
        public String TenantId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String CoinID { get; set; }

        public string CoinConfigID { set; get; }
        /// <summary>
        /// 最小充币
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal? CoinInMinAmount { get; set; }

        /// <summary>
        /// 最小提币
        /// </summary>
        [Column(TypeName = "decimal(28, 16)")]
        public Decimal? CoinOutMinAmount { get; set; }

        [Column(TypeName = "decimal(28, 16)")]
        public Decimal? CoinOutMaxAmount { get; set; } 

        /// <summary>
        /// CreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}