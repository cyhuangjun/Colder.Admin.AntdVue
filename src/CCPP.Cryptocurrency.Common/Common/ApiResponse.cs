using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    public class ApiResponse
    {
        public bool Status { get; set; }
        public string Data { get; set; }
        public ApiResponseStatus Code { get; set; }
    }
}
