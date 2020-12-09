using CCPP.Cryptocurrency.Common;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Business.Core
{
    public class CryptocurrencyBusiness : ICryptocurrencyBusiness, ITransientDependency
    {
        public ICryptocurrencyProvider GetCryptocurrencyProvider(Coin coin)
        {
            throw new NotImplementedException();
        }
    }
}
