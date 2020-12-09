using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Enum
{
    public enum PaymentStatus
    {
        Waiting,
        Confirming,
        Confirmed,
        Sending,
        Partially_paid,
        Finished,
        Failed,
        Refunded,
        Expired
    }
}
