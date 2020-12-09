using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    public interface ICryptocurrencyProvider
    {
        GenerateAddressResult CreateWalletAddress(CreateWalletAddressInput createAddressInfo);
    }
}
