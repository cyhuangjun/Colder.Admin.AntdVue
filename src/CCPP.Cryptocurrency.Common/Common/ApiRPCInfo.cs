using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    public class ApiRPCInfo
    {
        public string method { get; set; }
        public object @params { get; set; }
        public int id { get; set; }
    }
}
