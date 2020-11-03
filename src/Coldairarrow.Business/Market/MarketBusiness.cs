using CacheManager.Core;
using CCPP.Cryptocurrency.Common;
using Coldairarrow.Business.Foundation;
using Coldairarrow.Entity;
using Coldairarrow.Entity.DTO;
using Coldairarrow.Entity.Enum;
using Coldairarrow.Entity.Request;
using Coldairarrow.Entity.Response;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.IBusiness.Market;
using Coldairarrow.Util;
using EFCore.Sharding;
using System;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Market
{
    public class MarketBusiness : IMarketBusiness, ITransientDependency
    {
        IDbAccessor _db;
        ICacheManager<object> _cache;
        ICurrencyBusiness _currencyBusiness;
        ICoinBusiness _coinBusiness;
        ICryptocurrencyBusiness _cryptocurrencyBusiness;
        IUserAssetsBusiness _userAssetsBusiness;
        public MarketBusiness(IDbAccessor db,
                                ICacheManager<object> cache,
                                ICurrencyBusiness currencyBusiness,
                                ICoinBusiness coinBusiness,
                                ICryptocurrencyBusiness cryptocurrencyBusiness,
                                IUserAssetsBusiness userAssetsBusiness)
        {
            _db = db;
            _cache = cache;
            _currencyBusiness = currencyBusiness;
            _coinBusiness = coinBusiness;
            _cryptocurrencyBusiness = cryptocurrencyBusiness;
            _userAssetsBusiness = userAssetsBusiness;
        }

        public async Task<AjaxResult<CurrencyEstimateViewDto>> EstimateAsync(EstimateRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<AjaxResult<MinAmountViewDto>> MinAmountAsync(string userId, string currency)
        {
            throw new NotImplementedException();
        }

        public async Task<AjaxResult<PaymentViewDto>> PaymentAsync(string userId, PaymentRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.PriceCurrency))
                    return new AjaxResult<PaymentViewDto>() { ErrorCode = ErrorCodeDefine.CurrencyRequired, Success = false, Msg = "Fiatcurrency cannot be empty!" };
                if (string.IsNullOrEmpty(request.PayCurrency))
                    return new AjaxResult<PaymentViewDto>() { ErrorCode = ErrorCodeDefine.CoinNotRequired, Success = false, Msg = "Cryptocurrency cannot be empty!" };

                var coin = await this._coinBusiness.GetCoinByCodeAsync(request.PayCurrency);
                var currency = await this._currencyBusiness.GetCurrencyByCodeAsync(request.PriceCurrency);
                if (coin == null)
                    return new AjaxResult<PaymentViewDto>() { ErrorCode = ErrorCodeDefine.CoinNotExist, Success = false, Msg = $"Cryptocurrency {request.PayCurrency} Not Exist!" };
                if (currency == null)
                    return new AjaxResult<PaymentViewDto>() { ErrorCode = ErrorCodeDefine.CurrencyNoExist, Success = false, Msg = $"Fiatcurrency {request.PriceCurrency}  Not Exist!" };
                if (request.PayAmount <= 0 || request.PriceAmount <= 0 || request.PayAmount >= 100000000 || request.PriceAmount >= 100000000)
                    return new AjaxResult<PaymentViewDto>() { ErrorCode = ErrorCodeDefine.ParameterInvalid, Success = false, Msg = "Parameter Invalid!" };
                if (!coin.IsSupportWallet)
                    return new AjaxResult<PaymentViewDto>() { ErrorCode = ErrorCodeDefine.WalletMaintenancing, Success = false, Msg = "Wallet maintenancing!" };

                var securityKey = string.Empty;
                var addressResult = _cryptocurrencyBusiness.GetCryptocurrencyProvider(coin).CreateWalletAddress(new CreateWalletAddressInput() { SecurityKey = securityKey });
                if (string.IsNullOrEmpty(addressResult.Address))
                    return new AjaxResult<PaymentViewDto>() { Success = false, Msg = "server error!", ErrorCode = ErrorCodeDefine.ServerError };
                var payAmount = request.PayAmount ?? 0;
                var payment = new Payment()
                {
                    Id = Guid.NewGuid().GuidTo16String(),
                    CreatedAt = DateTime.Now,
                    OrderDescription = request.OrderDescription,
                    OrderId = request.OrderId,
                    PayAddress = addressResult.Address,
                    PrivateKey = addressResult.PrivateKey,
                    SecurityKey = securityKey,
                    PublicKey = addressResult.PublicKey,
                    PayAmount = payAmount,
                    PayCurrency = request.PayCurrency,
                    PriceAmount = request.PriceAmount,
                    PriceCurrency = request.PriceCurrency,
                    PurchaseId = request.PurchaseId,
                    Status = PaymentStatus.Waiting,
                    UserID = userId,
                    CallbackUrl = request.CallbackUrl,
                    UpdatedAt = DateTime.Now
                };
                await _db.InsertAsync(payment);
                return new AjaxResult<PaymentViewDto>()
                {
                    Success = true,
                    ErrorCode = ErrorCodeDefine.Success,
                    Data = new PaymentViewDto()
                    {
                        CallbackUrl = payment.CallbackUrl,
                        CreatedAt = payment.CreatedAt,
                        OrderDescription = payment.OrderDescription,
                        OrderId = payment.OrderId,
                        PayAddress = payment.PayAddress,
                        PayAmount = payment.PayAmount,
                        PayCurrency = payment.PayCurrency,
                        PaymentId = payment.Id,
                        PriceAmount = payment.PriceAmount,
                        PriceCurrency = payment.PriceCurrency,
                        PurchaseId = payment.PurchaseId,
                        UpdatedAt = payment.UpdatedAt,
                        Status = payment.Status
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return new AjaxResult<PaymentViewDto>() { Success = false, Msg = "server error!", ErrorCode = ErrorCodeDefine.ServerError };
            }
        }

        public async Task<AjaxResult<PaymentResultViewDto>> PaymentAsync(string userId, string paymentId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(paymentId))
                    return new AjaxResult<PaymentResultViewDto>() { ErrorCode = ErrorCodeDefine.ParameterInvalid, Success = false, Msg = "Parameter Invalid!" };
                var payment = await this._db.GetEntityAsync<Payment>(paymentId);
                if (payment == null)
                    return new AjaxResult<PaymentResultViewDto>() { ErrorCode = ErrorCodeDefine.ParameterInvalid, Success = false, Msg = "Parameter Invalid!" };
                if (payment.UserID != userId)
                    return new AjaxResult<PaymentResultViewDto>() { ErrorCode = ErrorCodeDefine.IllegalOperation, Success = false, Msg = "Illegal Operation!" };
                return new AjaxResult<PaymentResultViewDto>()
                {
                    Success = true,
                    ErrorCode = ErrorCodeDefine.Success,
                    Data = new PaymentResultViewDto()
                    {
                        CreatedAt = payment.CreatedAt,
                        OrderDescription = payment.OrderDescription,
                        OrderId = payment.OrderId,
                        PayAddress = payment.PayAddress,
                        PayAmount = payment.PayAmount,
                        PayCurrency = payment.PayCurrency,
                        PaymentId = payment.Id,
                        PriceAmount = payment.PriceAmount,
                        PriceCurrency = payment.PriceCurrency,
                        PurchaseId = payment.PurchaseId,
                        UpdatedAt = payment.UpdatedAt,
                        Status = payment.Status,
                        ActuallyPaid = payment.ActuallyPaid,
                        ActualAmount = payment.ActualAmount,
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return new AjaxResult<PaymentResultViewDto>() { Success = false, Msg = "server error!", ErrorCode = ErrorCodeDefine.ServerError };
            }
        }

        public async Task<AjaxResult<TransfersViewDto>> TransfersAsync(string userId, TransfersRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(request.CurrencyFrom) || string.IsNullOrEmpty(request.CurrencyTo) || string.IsNullOrEmpty(request.AddressTo) || request.AmountFrom <= 0)
                    return new AjaxResult<TransfersViewDto>() { ErrorCode = ErrorCodeDefine.ParameterInvalid, Success = false, Msg = "Parameter Invalid!" };
                var coin = await this._coinBusiness.GetCoinByCodeAsync(request.CurrencyFrom);
                if (coin == null)
                    return new AjaxResult<TransfersViewDto>() { ErrorCode = ErrorCodeDefine.CoinNotExist, Success = false, Msg = $"Cryptocurrency {request.CurrencyFrom} Not Exist!" };
                var balance = this._userAssetsBusiness.GetBalance(userId, request.CurrencyFrom);
                var feeRate = await this._coinBusiness.GetFeeRate(userId, coin.Id);
                var fee = request.AmountFrom * feeRate;
                if (balance < (request.AmountFrom + fee))
                    return new AjaxResult<TransfersViewDto>() { ErrorCode = ErrorCodeDefine.CryptocurrencyAssetsNotEnought, Success = false, Msg = $"Cryptocurrency {request.CurrencyFrom} Assets Not Enought!" };
                var estimateResult = await this.EstimateAsync(new EstimateRequest()
                {
                    Amount = request.AmountFrom,
                    CurrencyFrom = request.CurrencyFrom,
                    CurrencyTo = request.CurrencyTo
                });
                if (!estimateResult.Success)
                    return new AjaxResult<TransfersViewDto>() { Success = false, Msg = estimateResult.Msg, ErrorCode = estimateResult.ErrorCode };
                var businessId = Guid.NewGuid().GuidTo16String();
                var assetsChangeItemDTO = new AssetsChangeItemDTO()
                {
                    AssetsID = coin.Id,
                    AssetsWasteBookType = AssetsWasteBookType.CoinOut,
                    ChangeAvailableAmount = -1 * (request.AmountFrom + fee),
                    ChangeFreeAmount = (request.AmountFrom + fee),
                    FeeAmount = fee,
                    RelateID = businessId,
                    UserID = userId
                };
                this._userAssetsBusiness.UpdateAssets(assetsChangeItemDTO);
                var transfers = new Transfers()
                {
                    Id = businessId,
                    AddressTo = request.AddressTo,
                    AmountFrom = request.AmountFrom,
                    AmountTo = estimateResult.Data.EstimatedAmount,
                    CallbackUrl = request.CallbackUrl,
                    CreatedAt = DateTime.Now,
                    CurrencyFrom = request.CurrencyFrom,
                    CurrencyTo = request.CurrencyTo,
                    Fee = fee,
                    FeeRate = feeRate,
                    OrderDescription = request.OrderDescription,
                    OrderId = request.OrderId,
                    Status = TransfersStatus.Waiting,
                    UpdatedAt = DateTime.Now,
                    UserID = userId,
                };
                await _db.InsertAsync(transfers);
                return new AjaxResult<TransfersViewDto>()
                {
                    Success = true,
                    ErrorCode = ErrorCodeDefine.Success,
                    Data = new TransfersViewDto()
                    {
                        AddressTo = transfers.AddressTo,
                        AmountFrom = transfers.AmountFrom,
                        AmountTo = transfers.AmountTo,
                        CallbackUrl = transfers.CallbackUrl,
                        CurrencyFrom = transfers.CurrencyFrom,
                        CurrencyTo = transfers.CurrencyTo,
                        OrderDescription = transfers.OrderDescription,
                        OrderId = transfers.OrderId,
                        Status = transfers.Status,
                        WithdrawId = transfers.Id,
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return new AjaxResult<TransfersViewDto>() { Success = false, Msg = "server error!", ErrorCode = ErrorCodeDefine.ServerError };
            }
        }

        public async Task<AjaxResult<TransfersViewDto>> TransfersAsync(string userId, string withdrawId)
        {
            try
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
                        AmountFrom = transfers.AmountFrom,
                        AmountTo = transfers.AmountTo,
                        CallbackUrl = transfers.CallbackUrl,
                        CurrencyFrom = transfers.CurrencyFrom,
                        CurrencyTo = transfers.CurrencyTo,
                        OrderDescription = transfers.OrderDescription,
                        OrderId = transfers.OrderId,
                        Status = transfers.Status,
                        WithdrawId = transfers.Id,
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return new AjaxResult<TransfersViewDto>() { Success = false, Msg = "server error!", ErrorCode = ErrorCodeDefine.ServerError };
            }
        }
    }
}
