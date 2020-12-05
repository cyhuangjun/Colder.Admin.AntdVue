using CCPP.Cryptocurrency.Common;
using CCPP.Cryptocurrency.Common.Provider;
using Coldairarrow.Business.Foundation;
using Coldairarrow.Entity.Enum;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.Entity.Request;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Util;
using Coldairarrow.Util.Helper;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Core
{
    public class CryptocurrencyBusiness : ICryptocurrencyBusiness, ITransientDependency
    {
        readonly ISysWalletBusiness _sysWalletBusiness;
        readonly ISecurityWalletBusiness _securityWalletBusiness;
        readonly IEvaluateMinefeeRateBusiness _evaluateMinefeeRateBusiness;
        public CryptocurrencyBusiness(ISysWalletBusiness sysWalletBusiness,
                                      ISecurityWalletBusiness securityWalletBusiness,
                                      IEvaluateMinefeeRateBusiness evaluateMinefeeRateBusiness)
        {
            this._sysWalletBusiness = sysWalletBusiness;
            this._securityWalletBusiness = securityWalletBusiness;
            this._evaluateMinefeeRateBusiness = evaluateMinefeeRateBusiness;
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
                    config.MinerFeeRate = _evaluateMinefeeRateBusiness.GetBTCFeeRate();
                    return new BTCClientServiceProvider(config);
                case ProviderType.ETH:
                    config.GasPrice = _evaluateMinefeeRateBusiness.GetETHGasPrice().Recommend;
                    return new ETHClientServiceProvider(config);
                case ProviderType.ETHTokenCoin:
                    config.GasPrice = _evaluateMinefeeRateBusiness.GetETHGasPrice().Recommend;
                    return new ETHTokenClientServiceProvider(config);
            }
            throw new NotImplementedException();
        }

        public async Task<ResponseData<string>> SendCoinToSys(SendCoinInfo sendCoinInfo, Wallet wallet)
        {
            var coin = sendCoinInfo.Coin;
            var sendCoinData = new SendCoinData
            {
                FromAccount = wallet.Address,
                ToCoinAddress = sendCoinInfo.ToAddress,
                ToCoinAddressTag = sendCoinInfo.ToAddressTag,
                Amount = sendCoinInfo.Quantity,
            };
            if (!string.IsNullOrEmpty(wallet.SecurityKey))
            {
                sendCoinData.FromAccountPassword = EncryptionHelper.Decode(wallet.SecurityKey, wallet.UserID);
            }
            var provider = await GetCryptocurrencyProviderAsync(coin);
            var sendResult = provider.SendTransaction(sendCoinData);
            return sendResult;
        }

        public async Task<ResponseData<string>> SendCoinToCustomer(SendCoinInfo sendCoinInfo, SysWallet sysWallet)
        {
            var coin = sendCoinInfo.Coin;
            var coinProvider = await this.GetCryptocurrencyProviderAsync(coin);
            var sendCoinData = new SendCoinData()
            {
                ToCoinAddress = sendCoinInfo.ToAddress,
                ToCoinAddressTag = sendCoinInfo.ToAddressTag,
                Amount = Math.Abs(sendCoinInfo.Quantity),
            };
            if (sysWallet != null)
            {
                sendCoinData.FromAccount = sysWallet.Address;
                var sKey = EncryptionHelper.GetAvailableStr(sysWallet.ID);
                if (!string.IsNullOrEmpty(sysWallet.SecurityKey))
                {
                    sendCoinData.FromAccountPassword = Encoding.UTF8.GetString(EncryptionHelper.AESDecrypt(Convert.FromBase64String(sysWallet.SecurityKey), sKey));
                }
            }
            ResponseData<string> sendResult = coinProvider.SendTransaction(sendCoinData);
            return sendResult;
        } 
    }
}
