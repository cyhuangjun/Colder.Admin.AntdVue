using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    public class ApiRequestData
    {
        public string Api { get; set; }
        public object Body { get; set; }
        public string Path { get; set; }
        public string HttpMethod { get; set; }
    }
}
