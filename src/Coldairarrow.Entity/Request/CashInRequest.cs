using Coldairarrow.Entity.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Request
{
    public class CashInRequest : PaymentResultViewDto
    {
        public string Mac { set; get; }
    }
}
