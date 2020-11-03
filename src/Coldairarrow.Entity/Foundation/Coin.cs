using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Foundation
{
    /// <summary>
    /// Coin
    /// </summary>
    [Table("Coin")]
    public class Coin
    {

        /// <summary>
        /// ID
        /// </summary>
        [Key, Column(Order = 1)]
        public String Id { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public String Code { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// ProviderType
        /// </summary>
        public Int32 ProviderType { get; set; }

        /// <summary>
        /// ImageUrl
        /// </summary>
        public String ImageUrl { get; set; }

        /// <summary>
        /// IsAvailable
        /// </summary>
        public Boolean IsAvailable { get; set; }

        /// <summary>
        /// IsSupportWallet
        /// </summary>
        public Boolean IsSupportWallet { get; set; }

        /// <summary>
        /// IsUseSysAccount
        /// </summary>
        public Boolean IsUseSysAccount { get; set; }

        /// <summary>
        /// ApiUrl
        /// </summary>
        public String ApiUrl { get; set; }

        /// <summary>
        /// ApiSecurityKey
        /// </summary>
        public String ApiSecurityKey { get; set; }

        /// <summary>
        /// 单位精度，支持几位小数点
        /// </summary>
        public Int32? UnitPrecision { get; set; }

        /// <summary>
        /// MinConfirms
        /// </summary>
        public Int32 MinConfirms { get; set; }

        /// <summary>
        /// 最小可提币确认数
        /// </summary>
        public Int32? MinTransactionOutConfirms { get; set; }

        /// <summary>
        /// TokenCoinID
        /// </summary>
        public String TokenCoinID { get; set; }

        /// <summary>
        /// TokenCoinAddress
        /// </summary>
        public String TokenCoinAddress { get; set; }

        /// <summary>
        /// StartSyncBlockHeight
        /// </summary>
        public Int32? StartSyncBlockHeight { get; set; }

        /// <summary>
        /// TokenCoinPrecision
        /// </summary>
        public Int32? TokenCoinPrecision { get; set; }

        /// <summary>
        /// SortNumber
        /// </summary>
        public Int32? SortNumber { get; set; }

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
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// LastUpdateUserID
        /// </summary>
        public String LastUpdateUserID { get; set; }

    }
}