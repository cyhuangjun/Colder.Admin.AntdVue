using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Coldairarrow.Entity.Enum
{
    public enum TransactionStatus
    {
        /// <summary>
        /// 等待上级审批
        /// </summary>
        WaitingForApprovalFromTheSuperior = -3,
        /// <summary>
        /// 已受理、申请
        /// </summary>
        [Description("已受理")]
        Apply = 1,
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        PendingApproval = 2,
        /// <summary>
        /// 处理失败,钱包扣除失败
        /// </summary>
        [Description("钱包处理失败")]
        ProcessFail = 4,
        /// <summary>
        /// 审核未通过
        /// </summary>
        [Description("审核未通过")]
        ApprovalReject = 5,
        /// <summary>
        /// 完成，通过
        /// </summary>
        [Description("已完成")]
        Finish = 12,
        /// <summary>
        /// 处理中
        /// </summary>
        [Description("处理中")]
        Processing = 3,
        /// <summary>
        /// 待多签
        /// </summary>
        [Description("待多签")]
        ToBeMultiSignature = -6,
        /// <summary>
        /// 待签名
        /// </summary>
        [Description("待签名")]
        ToBeSigned = 6,
        /// <summary>
        /// 撤销
        /// </summary>
        [Description("撤销")]
        Cancel = 7,
        /// <summary>
        /// 待充值
        /// </summary>
        [Description("待充值")]
        WaitRecharge = 8,
        /// <summary>
        /// 待发送
        /// </summary>
        [Description("待发送")]
        WaitSend = -9,
        /// <summary>
        /// 确认中
        /// </summary>
        [Description("确认中")]
        WaitConfirm = 9,
        /// <summary>
        /// 已到账
        /// </summary>
        [Description("已到账")]
        ArrivedAccount = 10,
        /// <summary>
        /// 已汇出
        /// </summary>
        [Description("已汇出")]
        TransactionOut = 11,
        /// <summary>
        /// 小于系统最小充值记录，无法到账
        /// </summary>
        NotArriveLessInMinAmount = 13,
        /// <summary>
        /// 已冻结
        /// </summary>
        [Description("已冻结")]
        ITArrived = 14,
        /// <summary>
        /// 已解冻
        /// </summary>
        [Description("已解冻")]
        ITFreed = 15,
        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常订单")]
        Exception = 99
    }
}
