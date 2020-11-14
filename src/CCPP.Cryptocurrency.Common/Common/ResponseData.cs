using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    public class ResponseData<T>
    {
        public T Result { get; set; }
        public string Error { get; set; }
    }
}
