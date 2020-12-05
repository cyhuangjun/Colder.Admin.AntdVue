using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_Manage
{
    /// <summary>
    /// 部门表
    /// </summary>
    [Table("Base_Department")]
    public class Base_Department
    {

        /// <summary>
        /// 主键
        /// </summary>
        [Key, Column(Order = 1)]
        public String Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        public String CreatorId { get; set; }

        /// <summary>
        /// 否已删除
        /// </summary>
        public Boolean Deleted { get; set; }

        public bool? IsFrozen { set; get; }
        /// <summary>
        /// 部门名
        /// </summary>
        public String Name { get; set; }

        [Required]
        public string Code { set; get; }
        /// <summary>
        /// 上级部门Id
        /// </summary>
        public String ParentId { get; set; }

        public string ApiKey { set; get; }

        public string SecretKey { set; get; }

        public string CoinConfigID { set; get; }

        public string PaymentCallbackUrl { set; get; }

        public string TransfersCallbackUrl { set; get; }
    }
}