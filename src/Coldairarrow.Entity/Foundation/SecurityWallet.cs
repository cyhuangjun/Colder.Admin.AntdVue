using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Foundation
{
    /// <summary>
    /// SecurityWallet
    /// </summary>
    [Table("SecurityWallet")]
    public class SecurityWallet
    {

        /// <summary>
        /// ID
        /// </summary>
        [Key, Column(Order = 1)]
        public String ID { get; set; }

        /// <summary>
        /// 虚拟币
        /// </summary>
        public String CoinID { get; set; }

        /// <summary>
        /// 目标地址
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// 安全密钥
        /// </summary>
        public String SecurityKey { get; set; }

        /// <summary>
        /// Privatekey
        /// </summary>
        public String Privatekey { get; set; }

        /// <summary>
        /// Hash
        /// </summary>
        public String Hash { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public Boolean Enabled { get; set; }

        /// <summary>
        /// Deleted
        /// </summary>
        public Boolean Deleted { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public String CreatorID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后更新人
        /// </summary>
        public String LastUpdateUserID { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateTime { get; set; } 
    }
}