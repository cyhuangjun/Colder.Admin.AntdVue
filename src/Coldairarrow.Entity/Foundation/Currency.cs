using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Foundation
{
    /// <summary>
    /// Currency
    /// </summary>
    [Table("Currency")]
    public class Currency
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
        /// Symbol
        /// </summary>
        public String Symbol { get; set; }

        /// <summary>
        /// Sign
        /// </summary>
        public String Sign { get; set; }

        /// <summary>
        /// IsAvailable
        /// </summary>
        public Boolean IsAvailable { get; set; }

        /// <summary>
        /// DecimalDigit
        /// </summary>
        public Int32? DecimalDigit { get; set; }

        /// <summary>
        /// ImageUrl
        /// </summary>
        public String ImageUrl { get; set; }

        /// <summary>
        /// SortNumber
        /// </summary>
        public Int32 SortNumber { get; set; }

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