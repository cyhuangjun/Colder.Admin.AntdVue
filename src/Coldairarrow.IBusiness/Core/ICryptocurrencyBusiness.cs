using CCPP.Cryptocurrency.Common;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.Entity.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.IBusiness.Core
{
    public interface ICryptocurrencyBusiness
    { 
        Task<ICryptocurrencyProvider> GetCryptocurrencyProviderAsync(Coin coin);

        Task<ResponseData<string>> SendCoinToSys(SendCoinInfo sendCoinInfo, Wallet wallet);
        Task<ResponseData<string>> SendCoinToCustomer(SendCoinInfo sendCoinInfo, SysWallet sysWallet);
    }
}
