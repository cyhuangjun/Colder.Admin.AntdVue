using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Coldairarrow.Entity.Enum
{
    public enum TransfersStatus
    {
        [Description("等待审核")]
        Waiting,
        [Description("确认中")]
        Confirming,
        [Description("已完成")]
        Finished,
        [Description("失败")]
        Failed,
        [Description("取消")]
        Cancel,
        [Description("拒绝")]
        Disallowance
    }
}
