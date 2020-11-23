using Coldairarrow.Entity.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Request
{
    public class CashOutRequest : TransfersViewDto
    {
        public string Mac { set; get; }
    }
}
