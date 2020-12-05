using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Coldairarrow.Entity.Enum
{
    public enum TransactionStatus
    {
        /// <summary>
        /// 申请
        /// </summary>
        [Description("申请")]
        Apply,
        /// <summary>
        /// 处理失败,钱包扣除失败
        /// </summary>
        [Description("钱包处理失败")]
        ProcessFail,
        /// <summary>
        /// 确认中
        /// </summary>
        [Description("确认中")]
        WaitConfirm,  
        /// <summary>
        /// 完成，通过
        /// </summary>
        [Description("已完成")]
        Finished,
        /// <summary>
        /// 审核未通过
        /// </summary>
        [Description("审核未通过")]
        ApprovalReject,
        /// <summary>
        /// 撤销
        /// </summary>
        [Description("撤销")]
        Cancel,    
        /// <summary>
        /// 小于系统最小充值记录，无法到账
        /// </summary>
        NotArriveLessInMinAmount,
        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常订单")]
        Exception = 99
    }
}
