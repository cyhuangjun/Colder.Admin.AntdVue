using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Foundation
{
    /// <summary>
    /// Wallet
    /// </summary>
    [Table("Wallet")]
    public class Wallet
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
        /// UID
        /// </summary>
        public String UID { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// CoinID
        /// </summary>
        public String CoinID { get; set; }

        /// <summary>
        /// SecurityKey
        /// </summary>
        public String SecurityKey { get; set; }

        /// <summary>
        /// PrivateKey
        /// </summary>
        public String PrivateKey { get; set; }

        /// <summary>
        /// PublicKey
        /// </summary>
        public String PublicKey { get; set; }

        /// <summary>
        /// Hash
        /// </summary>
        public String Hash { get; set; }

        /// <summary>
        /// CreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}