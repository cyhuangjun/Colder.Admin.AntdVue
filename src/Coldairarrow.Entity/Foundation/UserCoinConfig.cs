﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Foundation
{
    /// <summary>
    /// UserCoinConfig
    /// </summary>
    [Table("UserCoinConfig")]
    public class UserCoinConfig
    {

        /// <summary>
        /// ID
        /// </summary>
        [Key, Column(Order = 1)]
        public String ID { get; set; }

        /// <summary>
        /// UserID
        /// </summary>
        public String UserID { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public String Currency { get; set; }

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
        /// CreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}