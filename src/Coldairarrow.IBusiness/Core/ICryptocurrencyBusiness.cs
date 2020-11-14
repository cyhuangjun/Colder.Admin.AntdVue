using CCPP.Cryptocurrency.Common;
using Coldairarrow.Entity.Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.IBusiness.Core
{
    public interface ICryptocurrencyBusiness
    { 
        Task<ICryptocurrencyProvider> GetCryptocurrencyProviderAsync(Coin coin);
    }
}
