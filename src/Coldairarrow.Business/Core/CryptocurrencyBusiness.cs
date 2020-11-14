using CCPP.Cryptocurrency.Common;
using CCPP.Cryptocurrency.Common.Provider;
using Coldairarrow.Business.Foundation;
using Coldairarrow.Entity.Enum;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Util;
using Coldairarrow.Util.Helper;
using System;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Core
{
    public class CryptocurrencyBusiness : ICryptocurrencyBusiness, ITransientDependency
    {
        readonly ISysWalletBusiness _sysWalletBusiness;
        readonly ISecurityWalletBusiness _securityWalletBusiness;
        public CryptocurrencyBusiness(ISysWalletBusiness sysWalletBusiness,
                                      ISecurityWalletBusiness securityWalletBusiness)
        {
            this._sysWalletBusiness = sysWalletBusiness;
            this._securityWalletBusiness = securityWalletBusiness;
        }

        public async Task<ICryptocurrencyProvider> GetCryptocurrencyProviderAsync(Coin coin)
        {
            var securityWallet = await this._securityWalletBusiness.GetEntityAsync(e => e.CoinID == coin.Id && e.Enabled && !e.Deleted);
            var sysWallet = await this._sysWalletBusiness.GetEntityAsync(e => e.CoinID == coin.Id && e.Enabled && !e.Deleted); 
            var config = new Configuration()
            {
                Host = string.Empty,
                Port = string.Empty,
                UserName = string.Empty,
                Password = string.Empty,
                Precision = coin.TokenCoinPrecision,
                ThridApiUrl = coin.ThridApiUrl,
                MultiSigAddress = securityWallet != null ? new Tuple<string, string>(securityWallet.Address, securityWallet.Privatekey) : null,
                ApiSecurityKey = coin.ApiSecurityKey,
                ApiUrl = coin.ApiUrl,
                Contract = coin.TokenCoinAddress,
                WalletSecurityKey = EncryptionHelper.Decode(coin.WalletSecurityKey, coin.Id),
            };
            if (sysWallet != null)
            {
                var addressInfo = new AddressInfo()
                {
                    Address = sysWallet.Address,
                };
                if (!string.IsNullOrEmpty(sysWallet.SecurityKey))
                {
                    addressInfo.Password = EncryptionHelper.Decode(sysWallet.SecurityKey, sysWallet.ID);
                }
                if (!string.IsNullOrEmpty(sysWallet.PrivateKey))
                {
                    addressInfo.PrivateKey = EncryptionHelper.Decode(sysWallet.PrivateKey, sysWallet.Address);
                }
                if (!string.IsNullOrEmpty(sysWallet.PublicKey))
                {
                    addressInfo.PublicKey = EncryptionHelper.Decode(sysWallet.PublicKey, sysWallet.Address);
                }
                config.SysAddress = addressInfo;
            }
            switch (coin.ProviderType)
            {
                case ProviderType.BTC:
                    return new BTCClientServiceProvider(config);
                case ProviderType.ETH:
                    return new ETHClientServiceProvider(config);
                case ProviderType.ETHTokenCoin:
                    return new ETHTokenClientServiceProvider(config);
            }
            throw new NotImplementedException();
        }
    }
}
