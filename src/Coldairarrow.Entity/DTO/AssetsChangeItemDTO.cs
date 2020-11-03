using Coldairarrow.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.DTO
{
    /// <summary>
    /// 资产变更信息
    /// </summary>
    [Serializable]
    public class AssetsChangeItemDTO
    { 
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 资产ID
        /// </summary>
        public string AssetsID { get; set; }
        /// <summary>
        /// 冻结资产变动数，+表示增加，-表示扣除
        /// </summary>
        public decimal ChangeFreeAmount { get; set; }
        /// <summary>
        /// 可用资产变动数，+表示增加，-表示扣除
        /// </summary>
        public decimal ChangeAvailableAmount { get; set; }        
        /// <summary>
        /// 手续费值
        /// </summary>
        public decimal FeeAmount { get; set; }
        /// <summary>
        /// 关联单号ID
        /// </summary>
        public string RelateID { get; set; }
        /// <summary>
        /// 资产流水类型
        /// </summary>
        public AssetsWasteBookType AssetsWasteBookType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
