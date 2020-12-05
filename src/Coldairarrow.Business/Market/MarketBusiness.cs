using CacheManager.Core;
using CCPP.Cryptocurrency.Common;
using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Business.Foundation;
using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity;
using Coldairarrow.Entity.DTO;
using Coldairarrow.Entity.Enum;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.Entity.Request;
using Coldairarrow.Entity.Response;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.IBusiness.Market;
using Coldairarrow.Util;
using Coldairarrow.Util.Helper;
using Dynamitey;
using EFCore.Sharding;
using System;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Coldairarrow.Business.Market
{
    public class MarketBusiness : IMarketBusiness, ITransientDependency
    {
        #region DI
        IDbAccessor _db;
        ICacheManager<object> _cache;
        ICurrencyBusiness _currencyBusiness;
        ICoinBusiness _coinBusiness;
        ICryptocurrencyBusiness _cryptocurrencyBusiness;
        IUserAssetsBusiness _userAssetsBusiness;
        IBase_UserBusiness _base_UserBusiness;
        ICoinConfigBusiness _coinConfigBusiness;
        ICacheDataBusiness _cacheDataBusiness;
        public MarketBusiness(IDbAccessor db,
                                ICacheManager<object> cache,
                                ICurrencyBusiness currencyBusiness,
                                ICoinBusiness coinBusiness,
                                ICryptocurrencyBusiness cryptocurrencyBusiness,
                                IUserAssetsBusiness userAssetsBusiness,
                                IBase_UserBusiness base_UserBusiness,
                                ICoinConfigBusiness coinConfigBusiness,
                                ICoinTransactionOutBusiness coinTransactionOutBusiness,
                                ICacheDataBusiness cacheDataBusiness)
        {
            _db = db;
            _cache = cache;
            _currencyBusiness = currencyBusiness;
            _coinBusiness = coinBusiness;
            _cryptocurrencyBusiness = cryptocurrencyBusiness;
            _userAssetsBusiness = userAssetsBusiness;
            _base_UserBusiness = base_UserBusiness;
            _coinConfigBusiness = coinConfigBusiness;
            _cacheDataBusiness = cacheDataBusiness;
        }
        #endregion

        public async Task<AjaxResult<CurrencyEstimateViewDto>> EstimateAsync(EstimateRequest request)
        {
            var result = new AjaxResult<CurrencyEstimateViewDto>(); 
            if (string.IsNullOrEmpty(request.CurrencyFrom) || string.IsNullOrEmpty(request.CurrencyTo) || request.Amount <= 0)
            {
                result.Success = false;
                result.ErrorCode = ErrorCodeDefine.ParameterInvalid;
                result.Msg = "parameter invalid!";
                return result;
            } 
            var cacheKey = BuildCacheKey(request.CurrencyFrom, request.CurrencyTo);
            var redis = RedisHelper.Instance;
            if (redis.Exists(cacheKey))
            {
                var estimatePrice = await redis.GetAsync<decimal>(cacheKey);
                result.Success = true;
                result.ErrorCode = ErrorCodeDefine.Success;
                result.Data = new CurrencyEstimateViewDto()
                {
                    AmountFrom = request.Amount,
                    CurrencyFrom = request.CurrencyFrom,
                    CurrencyTo = request.CurrencyTo,
                    EstimatedAmount = (request.Amount * estimatePrice),
                };
                return result;
            }
            else
            {
                result.Success = false;
                result.ErrorCode = ErrorCodeDefine.EstimatePriceNotFound;
                result.Msg = "estimate price not found.";
                return result;
            }
        }

        private string BuildCacheKey(string from, string to)
        {
            var cacheKey = string.Format("_MARKET_{0}_{1}_", from.ToUpper(), to.ToUpper());
            return cacheKey;
        }

        public async Task<AjaxResult<MinAmountViewDto>> MinAmountAsync(string userId, string currency)
        {
            var result = new AjaxResult<MinAmountViewDto>() { Data = new MinAmountViewDto() { Currency = currency } }; 
            if (string.IsNullOrEmpty(currency))
            {
                result.ErrorCode = ErrorCodeDefine.CoinNotRequired;
                result.Msg = "currency cannot be empty!";
                return result;
            }
            var coinConfig = await this._coinConfigBusiness.MinAmountAsync(userId, currency);
            if(coinConfig == null)
            {
                result.Success = false;
                result.ErrorCode = ErrorCodeDefine.CoinConfigNotFound;
            }
            else
            {
                result.Success = true;
                result.ErrorCode = ErrorCodeDefine.Success;
                result.Data.MinPaymentAmount = coinConfig.MinPaymentAmount;
                result.Data.MinTransferAmount = coinConfig.MinTransferAmount;
            }
            return result;
        }

        public async Task<AjaxResult<PaymentViewDto>> PaymentAsync(string userId, PaymentRequest request)
        {
            var result = new AjaxResult<PaymentViewDto>();
            if (string.IsNullOrEmpty(request.UID))
            {
                result.ErrorCode = ErrorCodeDefine.CoinNotRequired;
                result.Msg = "uid cannot be empty!";
                return result;
            }
            if (string.IsNullOrEmpty(request.PayCurrency))
            {
                result.ErrorCode = ErrorCodeDefine.CoinNotRequired;
                result.Msg = "Cryptocurrency cannot be empty!";
                return result;
            }
            var coin = await this._coinBusiness.GetCoinByCodeAsync(request.PayCurrency);
            if (coin == null)
            {
                result.ErrorCode = ErrorCodeDefine.CoinNotExist;
                result.Msg = $"Cryptocurrency {request.PayCurrency} Not Exist!";
                return result;
            }
            if (!coin.IsSupportWallet)
            {
                result.ErrorCode = ErrorCodeDefine.WalletMaintenancing;
                result.Msg = "Wallet maintenancing!";
                return result;
            }
            var user = await this._base_UserBusiness.GetTheDataAsync(userId);
            if (user == null)
            {
                result.ErrorCode = ErrorCodeDefine.UserIDNotExist;
                result.Msg = "userid not exist.";
                return result;
            }
            var wallet = await this._cacheDataBusiness.GetWallet(userId, request.UID, coin.Id);
            if (wallet == null)
            {
                var cryptocurrencyProvider = await _cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(coin);
                var securityKey = string.Empty;
                if (cryptocurrencyProvider.IsNeedSecurityKey)
                {
                    securityKey = Guid.NewGuid().ToString().CreateSign();
                }
                var addressResult = cryptocurrencyProvider.CreateWalletAddress(new CreateWalletAddressInput() { SecurityKey = securityKey });
                if (!string.IsNullOrEmpty(addressResult.Error))
                {
                    result.ErrorCode = ErrorCodeDefine.ServerError;
                    result.Msg = $"server error, {addressResult.Error}!";
                    return result;
                }
                var walletId = Guid.NewGuid().GuidTo16String();
                wallet = new Wallet()
                {
                    Id = walletId,
                    Address = addressResult.Result.Address,
                    PrivateKey = EncryptionHelper.Encode(addressResult.Result.PrivateKey, userId),
                    SecurityKey = !string.IsNullOrEmpty(securityKey) ? EncryptionHelper.Encode(securityKey, userId) : string.Empty,
                    PublicKey = EncryptionHelper.Encode(addressResult.Result.PublicKey, userId),
                    CoinID = coin.Id,
                    UserID = userId,
                    UID = request.UID,
                    CreateTime = DateTime.Now,
                    Hash = EncryptionHelper.Encode(addressResult.Result.Address, walletId)
                };
                await this._db.InsertAsync(wallet);
            }
            var minAmountConfig = await this._coinConfigBusiness.MinAmountAsync(userId, request.PayCurrency);
            var coinConfig = await this._coinConfigBusiness.GetEntityAsync(userId, coin.Code);
            result.Success = true;
            result.ErrorCode = ErrorCodeDefine.Success;
            result.Data = new PaymentViewDto()
            {
                UID = request.UID,
                PayAddress = wallet.Address,
                PayCurrency = request.PayCurrency,
                FeeRate = coinConfig?.CoinInHandlingFeeRate,
                FeeType = coinConfig?.CoinInHandlingFeeModeType,
                FixFee = coinConfig?.CoinInHandlingFee,
                MinFee = coinConfig?.CoinInHandlingMinFee,
                MinPayAmount = minAmountConfig.MinPaymentAmount
            };
            return result;
        }

        [Transactional]
        public async Task<AjaxResult<TransfersViewDto>> TransfersAsync(string userId, TransfersRequest request)
        {
            var result = new AjaxResult<TransfersViewDto>();
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(request.Currency) || string.IsNullOrEmpty(request.AddressTo) || request.Amount <= 0)
            {
                result.ErrorCode = ErrorCodeDefine.ParameterInvalid;
                result.Msg = "Parameter Invalid!";
                return result;
            }
            var coin = await this._coinBusiness.GetCoinByCodeAsync(request.Currency);
            if (coin == null)
            {
                result.ErrorCode = ErrorCodeDefine.CoinNotExist;
                result.Msg = $"Cryptocurrency {request.Currency} Not Exist!";
                return result;
            }
            var user = await this._base_UserBusiness.GetTheDataAsync(userId);
            if (user == null)
            {
                result.ErrorCode = ErrorCodeDefine.UserIDNotExist;
                result.Msg = "userid not exist.";
                return result;
            }
            var balance = await this._userAssetsBusiness.GetBalance(userId, request.Currency);
            var coinConfig = await this._coinConfigBusiness.GetEntityAsync(userId, coin.Code);
            var fee = 0m; 
            if (coinConfig != null)
            {
                // 固定手续费
                if (coinConfig.CoinOutHandlingFeeModeType == FeeMode.Fixed)
                {
                    fee = coinConfig.CoinOutHandlingFee ?? 0;
                }
                else
                {
                    fee = request.Amount * coinConfig.CoinOutHandlingFeeRate ?? 0;
                    fee = Math.Round(fee, GlobalData.QuantityPercision);
                    fee = Math.Max(fee, coinConfig.CoinOutHandlingMinFee ?? 0);
                }
            }
            if (balance < (request.Amount + fee))
            {
                result.ErrorCode = ErrorCodeDefine.CryptocurrencyAssetsNotEnought;
                result.Msg = $"Cryptocurrency {request.Currency} Assets Not Enought!";
                return result;
            }            
            var minAmountConfig = await this._coinConfigBusiness.MinAmountAsync(userId, request.Currency);
            if (minAmountConfig != null)
            {
                if (minAmountConfig.MinTransferAmount > request.Amount)
                    result.ErrorCode = ErrorCodeDefine.TransferLessThanMinQty;
                result.Msg = "transfer less than min qty.";
                return result;
            }
            var businessId = Guid.NewGuid().GuidTo16String();
            var assetsChangeItemDTO = new AssetsChangeItemDTO()
            {
                CoinID = coin.Id,
                AssetsWasteBookType = AssetsWasteBookType.CoinOut,
                ChangeAvailableAmount = -1 * (request.Amount + fee),
                ChangeFrozenAmount = (request.Amount + fee),
                FeeAmount = fee,
                RelateID = businessId,
                UserID = userId
            };
            var assetsResult = await this._userAssetsBusiness.UpdateAssets(assetsChangeItemDTO);
            if (!assetsResult.Success)
            {
                result.ErrorCode = assetsResult.ErrorCode;
                result.Msg = assetsResult.Msg;
                return result;
            }
            var transfers = new Transfers()
            {
                Id = businessId,
                AddressTo = request.AddressTo,
                Amount = request.Amount,
                CreatedAt = DateTime.Now,
                Currency = request.Currency,
                HandlingFee = fee, 
                OrderDescription = request.OrderDescription,
                OrderId = request.OrderId,
                Status = TransfersStatus.Waiting,
                UpdatedAt = DateTime.Now,
                UserID = userId,
            }; 

            var coinTo = await this._coinBusiness.GetCoinByCodeAsync(request.Currency);
            if (coinTo != null)
            {
                var coinTransactionOut = new CoinTransactionOut()
                {
                    Id = Guid.NewGuid().GuidTo16String(),
                    Address = transfers.AddressTo,
                    AddressTag = string.Empty,
                    Amount = transfers.Amount,
                    CoinID = coinTo.Id,
                    CreateTime = DateTime.Now,
                    LastUpdateTime = DateTime.Now,
                    LastUpdateUserID = userId,
                    Status = TransactionStatus.Apply,
                    UserID = userId,
                };
                await _db.InsertAsync(coinTransactionOut);
                transfers.TransactionOutID = coinTransactionOut.Id;
            }
            await _db.InsertAsync(transfers);

            result.Success = true;
            result.ErrorCode = ErrorCodeDefine.Success;
            result.Data = new TransfersViewDto()
            {
                AddressTo = transfers.AddressTo,
                Amount = transfers.Amount,
                Currency = transfers.Currency,
                OrderDescription = transfers.OrderDescription,
                OrderId = transfers.OrderId,
                Status = transfers.Status,
                WithdrawId = transfers.Id,
            };
            return result;
        }

        public async Task<AjaxResult<TransfersViewDto>> TransfersAsync(string userId, string withdrawId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(withdrawId))
                return new AjaxResult<TransfersViewDto>() { ErrorCode = ErrorCodeDefine.ParameterInvalid, Success = false, Msg = "Parameter Invalid!" };
            var transfers = await this._db.GetEntityAsync<Transfers>(withdrawId);
            if (transfers == null)
                return new AjaxResult<TransfersViewDto>() { ErrorCode = ErrorCodeDefine.ParameterInvalid, Success = false, Msg = "Parameter Invalid!" };
            if (transfers.UserID != userId)
                return new AjaxResult<TransfersViewDto>() { ErrorCode = ErrorCodeDefine.IllegalOperation, Success = false, Msg = "Illegal Operation!" };
            return new AjaxResult<TransfersViewDto>()
            {
                Success = true,
                ErrorCode = ErrorCodeDefine.Success,
                Data = new TransfersViewDto()
                {
                    AddressTo = transfers.AddressTo,
                    Amount = transfers.Amount,
                    Currency = transfers.Currency,
                    OrderDescription = transfers.OrderDescription,
                    OrderId = transfers.OrderId,
                    Status = transfers.Status,
                    WithdrawId = transfers.Id,
                }
            };
        }
    }
}
