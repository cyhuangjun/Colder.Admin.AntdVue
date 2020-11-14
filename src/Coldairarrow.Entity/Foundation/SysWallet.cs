using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Foundation
{
    /// <summary>
    /// SysWallet
    /// </summary>
    [Table("SysWallet")]
    public class SysWallet
    {

        /// <summary>
        /// ID
        /// </summary>
        [Key, Column(Order = 1)]
        public String ID { get; set; }

        /// <summary>
        /// CoinID
        /// </summary>
        public String CoinID { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public String Address { get; set; }

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
        /// Enabled
        /// </summary>
        public Boolean Enabled { get; set; }

        /// <summary>
        /// Deleted
        /// </summary>
        public Boolean Deleted { get; set; }

        /// <summary>
        /// CreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// CreatorID
        /// </summary>
        public String CreatorID { get; set; }

        /// <summary>
        /// LastUpdateTime
        /// </summary>
        public DateTime? LastUpdateTime { get; set; }

        /// <summary>
        /// LastUpdateUserID
        /// </summary>
        public String LastUpdateUserID { get; set; }

    }
}