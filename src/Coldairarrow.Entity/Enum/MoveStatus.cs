using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Enum
{
    public enum MoveStatus
    {
        WaitMoveToSys,
        MoveSysWaitConfirm,
        WaitMoveToUser,
        MoveUserWaitConfirm, 
        Finish,
        Invalid
    }
}
