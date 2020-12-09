using CCPP.Cryptocurrency.Common;
using Coldairarrow.Entity.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.IBusiness.Core
{
    public interface ICryptocurrencyBusiness
    {
        ICryptocurrencyProvider GetCryptocurrencyProvider(Coin coin);
    }
}
