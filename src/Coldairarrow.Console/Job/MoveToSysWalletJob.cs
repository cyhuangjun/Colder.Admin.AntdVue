using Autofac;
using CCPP.Cryptocurrency.Common;
using Coldairarrow.Business.Foundation;
using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.Enum;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.Entity.Request;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Scheduler.Core;
using Coldairarrow.Util;
using Coldairarrow.Util.Helper;
using Exceptionless;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coldairarrow.Scheduler.Job
{
    public class MoveToSysWalletJob : IJob
    {
        #region VARIABLE 
        private static bool _isRun = false;
        /// <summary>
        /// 运行状态
        /// </summary>
        private static int _runStatus = 0;
        #endregion

        #region DI
        private readonly ILifetimeScope _container;
        private readonly ILogger<ConfirmTransactionSynchronizationJob> _logger;
        private readonly ICacheDataBusiness _cacheDataBusiness;
        private readonly ICoinTransactionInBusiness _coinTransactionInBusiness;
        private readonly ICryptocurrencyBusiness _cryptocurrencyBusiness;
        private readonly ISysWalletBusiness _sysWalletBusiness;
        private readonly IWalletBusiness _walletBusiness;
        private readonly IEvaluateMinefeeRateBusiness _evaluateMinefeeRateBusiness;
        #endregion

        public MoveToSysWalletJob()
        {
            this._container = RmesAutoFacModule.GetContainer();
            this._logger = this._container.Resolve<ILogger<ConfirmTransactionSynchronizationJob>>();
            this._cacheDataBusiness = this._container.Resolve<ICacheDataBusiness>();
            this._coinTransactionInBusiness = this._container.Resolve<ICoinTransactionInBusiness>();
            this._cryptocurrencyBusiness = this._container.Resolve<ICryptocurrencyBusiness>();
            this._sysWalletBusiness = this._container.Resolve<ISysWalletBusiness>();
            this._walletBusiness = this._container.Resolve<IWalletBusiness>();
            this._evaluateMinefeeRateBusiness = this._container.Resolve<IEvaluateMinefeeRateBusiness>();
        }

        public Task Execute(IJobExecutionContext context)
        {
            if (Interlocked.CompareExchange(ref _runStatus, 1, 0) != 0)
            {
                return Task.FromResult<object>(null);
            }
            Thread.MemoryBarrier();
            if (_isRun)
            {
                return Task.FromResult<object>(null);
            }
            _isRun = true;
            Thread.MemoryBarrier();
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    await MoveToSysWallet();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    ex.ToExceptionless().Submit();
                }
                finally
                {
                    Interlocked.CompareExchange(ref _runStatus, 0, 1);
                    _isRun = false;
                    Thread.MemoryBarrier();
                }
            });
        }

        private async Task MoveToSysWallet()
        {
            var coins = (await this._cacheDataBusiness.GetCoinsAsync()).Where(e => e.IsUseSysAccount && e.IsSupportWallet);
            var transactions = await this._coinTransactionInBusiness.GetListAsync(e => e.Status == TransactionStatus.Finished && e.MoveStatus != MoveStatus.Finish && e.MoveStatus != MoveStatus.Invalid);
            var transactionsGroupCoins = transactions.GroupBy(e => e.CoinID);
            foreach (var transactionsGroupCoin in transactionsGroupCoins)
            {
                var coin = await this._cacheDataBusiness.GetCoinAsync(transactionsGroupCoin.Key);
                if (coin == null)
                {
                    foreach (var item in transactionsGroupCoin)
                    {
                        item.MoveStatus = MoveStatus.Invalid;
                    }
                    await this._coinTransactionInBusiness.UpdateDataAsync(transactionsGroupCoin.ToList());
                    continue;
                }
                if (!coin.IsUseSysAccount)
                {
                    foreach (var item in transactionsGroupCoin)
                    {
                        item.MoveStatus = MoveStatus.Finish;
                    }
                    await this._coinTransactionInBusiness.UpdateDataAsync(transactionsGroupCoin.ToList());
                    continue;
                }
                if (!coin.IsSupportWallet)
                    continue;
                string sysCoinID = transactionsGroupCoin.Key;
                if (!string.IsNullOrEmpty(coin.TokenCoinID))
                {
                    sysCoinID = coin.TokenCoinID;
                }
                var sysWallet = await this._sysWalletBusiness.GetEntityAsync(e => e.CoinID == sysCoinID && !e.Deleted && e.Enabled);
                if (sysWallet == null)
                {
                    continue;
                }
                var cryptocurrencyProvider = await this._cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(coin);
                var transactionsGroupAddress = transactionsGroupCoin.GroupBy(e => e.Address);
                foreach (var txAddressG in transactionsGroupAddress)
                {
                    try
                    {
                        switch (txAddressG.First().MoveStatus)
                        {
                            case MoveStatus.WaitMoveToSys:
                                await HandleWaitMoveToSys(txAddressG, cryptocurrencyProvider, sysWallet, coin);
                                break;
                            case MoveStatus.MoveSysWaitConfirm:
                                await HandleMoveSysWaitConfirm(txAddressG, cryptocurrencyProvider, coin);
                                break;
                            case MoveStatus.WaitMoveToUser:
                                await HandleWaitMoveToUser(txAddressG, cryptocurrencyProvider, sysWallet, coin);
                                break;
                            case MoveStatus.MoveUserWaitConfirm:
                                await HandleMoveUserWaitConfirm(txAddressG, cryptocurrencyProvider, coin);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        this._logger.LogError(e, $"移动充币到系统地址错误,TransactionInID:{string.Join(",", txAddressG.Select(e => e.Id))}");
                        e.ToExceptionless().Submit();
                    }
                }
            }
        }

        private async Task HandleMoveUserWaitConfirm(IGrouping<string, CoinTransactionIn> txAddressG, ICryptocurrencyProvider cryptocurrencyProvider, Coin coin)
        {
            foreach (var tx in txAddressG)
                await HandleMoveUserWaitConfirm(tx, cryptocurrencyProvider, coin);
        }

        private async Task HandleMoveUserWaitConfirm(CoinTransactionIn tx, ICryptocurrencyProvider cryptocurrencyProvider, Coin coin)
        {
            if (string.IsNullOrEmpty(tx.SysToUserTXID))
            {
                tx.LastUpdateTime = DateTime.Now;
                tx.MoveStatus = MoveStatus.Invalid;
                await this._coinTransactionInBusiness.UpdateDataAsync(tx);
                return;
            }
            if (cryptocurrencyProvider.HasTransactionReceipt)
            {
                var result = cryptocurrencyProvider.GetTransactionReceipt(tx.SysToUserTXID);
                if (result?.Result?.IsSuccess ?? false)
                {
                    tx.LastUpdateTime = DateTime.Now;
                    tx.MoveUserMinefee = GetMinefee(cryptocurrencyProvider, tx.SysToUserTXID);
                    tx.MoveStatus = MoveStatus.WaitMoveToSys;
                    await this._coinTransactionInBusiness.UpdateDataAsync(tx);
                }
            }
            else
            {
                var transactionInfo = cryptocurrencyProvider.GetTransaction(tx.SysToUserTXID);
                if (transactionInfo.Result != null)
                {
                    if (cryptocurrencyProvider.HasNotBlockCount)
                    {
                        if (transactionInfo.Result.Valid)
                        {
                            tx.LastUpdateTime = DateTime.Now;
                            tx.MoveUserMinefee = GetMinefee(cryptocurrencyProvider, tx.SysToUserTXID);
                            tx.MoveStatus = MoveStatus.WaitMoveToSys;
                            await this._coinTransactionInBusiness.UpdateDataAsync(tx);
                        }
                    }
                    else
                    {
                        if (transactionInfo.Result.Confirmations >= coin.MinConfirms)
                        {
                            tx.LastUpdateTime = DateTime.Now;
                            tx.MoveUserMinefee = GetMinefee(cryptocurrencyProvider, tx.SysToUserTXID);
                            tx.MoveStatus = MoveStatus.WaitMoveToSys;
                            await this._coinTransactionInBusiness.UpdateDataAsync(tx);
                        }
                    }
                }
            }
        }

        private async Task HandleWaitMoveToUser(IGrouping<string, CoinTransactionIn> txAddressG, ICryptocurrencyProvider cryptocurrencyProvider, SysWallet sysWallet, Coin tokenCoin)
        {
            var userAddress = txAddressG.Key;
            var coin = await this._cacheDataBusiness.GetCoinAsync(tokenCoin.TokenCoinID);
            var orgCoinProvider = await this._cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(coin); 
            var balanceData = new BalanceData
            {
                Address = userAddress
            };
            //先判断用户钱包有没有矿工费,如果有的话就不需要从系统钱包调
            var balanceResult = orgCoinProvider.GetBalance(balanceData); 
            var estimateMinerfeeResult = (await EstimateMinerfee(tokenCoin, userAddress, sysWallet.Address, txAddressG.Sum(e => e.Amount)));
            if (!string.IsNullOrEmpty(estimateMinerfeeResult.Error))
            {
                var exception = new Exception($"评估矿工费错误,{estimateMinerfeeResult.Error}");
                this._logger.LogError(exception, exception.Message);
                exception.ToExceptionless().Submit();
                return;
            }

            if (balanceResult.Result >= estimateMinerfeeResult.Result)
            {
                foreach (var tx in txAddressG)
                {
                    tx.MoveStatus = MoveStatus.WaitMoveToSys;
                    tx.LastUpdateTime = DateTime.Now;
                }
                await this._coinTransactionInBusiness.UpdateDataAsync(txAddressG.ToList());
                return;
            }
            var totalAmount = estimateMinerfeeResult.Result * 2; 
            var sendCoinInfo = new SendCoinInfo
            {
                Coin = coin,
                Quantity = totalAmount,
                ToAddress = userAddress,    
            };
            var result = await this._cryptocurrencyBusiness.SendCoinToCustomer(sendCoinInfo, sysWallet);
            if (string.IsNullOrEmpty(result.Error))
            { 
                foreach (var tx in txAddressG)
                {
                    tx.MinefeeCoinID = tokenCoin.TokenCoinID;
                    tx.SysToUserTXID = result.Result;
                    tx.ReserveMinerfees = totalAmount;
                    tx.MoveStatus = MoveStatus.MoveUserWaitConfirm;
                    tx.LastUpdateTime = DateTime.Now;
                }
                await this._coinTransactionInBusiness.UpdateDataAsync(txAddressG.ToList());
            }
        }

        private async Task HandleWaitMoveToSys(IGrouping<string, CoinTransactionIn> txAddressGroup, ICryptocurrencyProvider cryptocurrencyProvider, SysWallet sysWallet, Coin coin)
        {
            if (!string.IsNullOrEmpty(coin.TokenCoinID))
            {
                await HandleWaitMoveTokenToSys(txAddressGroup, sysWallet, coin);
                return;
            }
            var userAddress = txAddressGroup.Key;
            var customerWallet = await this._walletBusiness.GetEntityAsync(e => e.Address == userAddress && e.CoinID == sysWallet.CoinID);
            decimal moveAmount = txAddressGroup.Sum(e => e.Amount);
            if (customerWallet != null)
            {
                string securityKey = customerWallet.SecurityKey;
                if (!string.IsNullOrEmpty(securityKey))
                    securityKey = EncryptionHelper.Decode(securityKey, customerWallet.UserID);
                var estimateGasData = new EstimateMinerfeeData
                {
                    From = customerWallet.Address,
                    To = sysWallet.Address,
                    TokenAddress = coin.TokenCoinAddress,
                    Amount = moveAmount
                };
                var balanceData = new BalanceData
                {
                    Address = customerWallet.Address
                };
                var balanceResult = cryptocurrencyProvider.GetBalance(balanceData);
                if (balanceResult.Result < moveAmount)
                {
                    foreach (var tx in txAddressGroup)
                    {
                        tx.MoveStatus = MoveStatus.Invalid;
                        tx.LastUpdateTime = DateTime.Now;
                    }
                    await this._coinTransactionInBusiness.UpdateDataAsync(txAddressGroup.ToList());
                    return;
                }
                var estimateMinerfeeResult = cryptocurrencyProvider.EstimateMinerfee(estimateGasData);
                if (!string.IsNullOrEmpty(estimateMinerfeeResult.Error))
                {
                    var exception = new Exception($"评估矿工费错误,{estimateMinerfeeResult.Error}");
                    this._logger.LogError(exception, exception.Message);
                    exception.ToExceptionless().Submit();
                    return;
                }
                moveAmount = moveAmount - estimateMinerfeeResult.Result;
                string sysAddress = sysWallet.Address;
                var sendCoinInfo = new SendCoinInfo
                {
                    Coin = coin,
                    Quantity = moveAmount,
                    ToAddress = sysAddress,
                };
                var moveTXID = (await this._cryptocurrencyBusiness.SendCoinToSys(sendCoinInfo, customerWallet)).Result;
                if (!string.IsNullOrEmpty(moveTXID))
                {
                    foreach (var tx in txAddressGroup)
                    {
                        tx.LastUpdateTime = DateTime.Now;
                        tx.UserToSysTXID = moveTXID;
                        tx.MoveToAddress = sysWallet.Address;
                        tx.MoveStatus = MoveStatus.MoveSysWaitConfirm;
                        tx.MoveTime = DateTime.Now;
                        tx.MinefeeCoinID = coin.Id;
                        tx.MoveToAddress = sysAddress;
                    }
                    txAddressGroup.First().MoveAmount = moveAmount;
                }
                await this._coinTransactionInBusiness.UpdateDataAsync(txAddressGroup.ToList());
            }
        }

        private async Task HandleWaitMoveTokenToSys(IGrouping<string, CoinTransactionIn> txAddressGroup, SysWallet sysWallet, Coin tokenCoin)
        {
            var coin = await this._cacheDataBusiness.GetCoinAsync(tokenCoin.TokenCoinID);
            var provider = await this._cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(coin);
            var tokenProvider = await this._cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(tokenCoin);
            var totalTokenAmount = txAddressGroup.Sum(e => e.Amount);
            var userAddress = txAddressGroup.Key;
            var estimateMinerfeeResult = await EstimateMinerfee(tokenCoin, userAddress, sysWallet.Address, totalTokenAmount);
            if (!string.IsNullOrEmpty(estimateMinerfeeResult.Error))
            {
                var exception = new Exception($"评估矿工费错误,{estimateMinerfeeResult.Error}");
                this._logger.LogError(exception, exception.Message);
                exception.ToExceptionless().Submit();
                return;
            }
            var balanceData = new BalanceData
            {
                Address = userAddress
            };
            var balance = provider.GetBalance(balanceData);
            var needAmount = estimateMinerfeeResult.Result;
            if (balance.Result < (needAmount))
            {
                foreach (var tx in txAddressGroup)
                {
                    tx.MoveStatus = MoveStatus.WaitMoveToUser;
                    tx.LastUpdateTime = DateTime.Now;
                }
                await this._coinTransactionInBusiness.UpdateDataAsync(txAddressGroup.ToList());
                return;
            }
            var ethTokenProvider = await this._cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(tokenCoin);
            var balanceResult = ethTokenProvider.GetBalance(new BalanceData
            {
                Address = userAddress,
                TokenContractAddress = tokenCoin.TokenCoinAddress
            });
            if (balanceResult.Result < totalTokenAmount)
            {
                var exception = new Exception($"未能到账用户地址{userAddress},Coin Code:{tokenCoin.Code},地址余额:{balanceResult.Result},转币量:{totalTokenAmount}");
                this._logger.LogError(exception, exception.Message);
                ExceptionlessClient.Default.CreateException(exception).Submit();
            }
            totalTokenAmount = balanceResult.Result;
            var sendCoinInfo = new SendCoinInfo
            {
                Coin = tokenCoin,
                Quantity = totalTokenAmount,
                ToAddress = sysWallet.Address,
            };
            var customerWallet = await this._walletBusiness.GetEntityAsync(e => e.Address == userAddress);
            if (customerWallet == null) return;
            var sendResult = await this._cryptocurrencyBusiness.SendCoinToSys(sendCoinInfo, customerWallet);
            if (string.IsNullOrEmpty(sendResult.Error))
            {
                var moveTXID = sendResult.Result;
                foreach (var tx in txAddressGroup)
                {
                    tx.LastUpdateTime = DateTime.Now;
                    tx.UserToSysTXID = moveTXID;
                    tx.MoveToAddress = sysWallet.Address;
                    tx.MoveStatus = MoveStatus.MoveSysWaitConfirm; 
                    tx.MoveTime = DateTime.Now;
                    tx.MinefeeCoinID = coin.Id;
                }
                txAddressGroup.First().MoveAmount = totalTokenAmount;
                await this._coinTransactionInBusiness.UpdateDataAsync(txAddressGroup.ToList());
            }
            else
            {
                var exception = new Exception($"代币发送结果信息Address:{userAddress},发送结果:{JsonConvert.SerializeObject(sendResult)}");
                this._logger.LogError(exception, exception.Message);
                Exceptionless.ExceptionlessClient.Default.CreateException(exception).Submit();
            }
        }

        private async Task HandleMoveSysWaitConfirm(IGrouping<string, CoinTransactionIn> txAddressGroup, ICryptocurrencyProvider cryptocurrencyProvider, Coin coin)
        {
            foreach (var tx in txAddressGroup)
                await HandleMoveSysWaitConfirm(tx, cryptocurrencyProvider, coin);
        }

        private async Task HandleMoveSysWaitConfirm(CoinTransactionIn tx, ICryptocurrencyProvider cryptocurrencyProvider, Coin coin)
        {
            if (string.IsNullOrEmpty(tx.UserToSysTXID))
            {
                tx.LastUpdateTime = DateTime.Now;
                tx.MoveStatus = MoveStatus.Invalid;
                await this._coinTransactionInBusiness.UpdateDataAsync(tx);
                return;
            }
            if (cryptocurrencyProvider.HasTransactionReceipt)
            {
                var result = cryptocurrencyProvider.GetTransactionReceipt(tx.UserToSysTXID);
                if (result?.Result?.IsSuccess ?? false)
                {
                    tx.LastUpdateTime = DateTime.Now;
                    tx.MoveSysMinefee = GetMinefee(cryptocurrencyProvider, tx.UserToSysTXID);
                    tx.MoveStatus = MoveStatus.Finish;
                    await this._coinTransactionInBusiness.UpdateDataAsync(tx);
                }
            }
            else
            {
                var transactionInfo = cryptocurrencyProvider.GetTransaction(tx.UserToSysTXID);
                if (transactionInfo.Result != null)
                {
                    if (cryptocurrencyProvider.HasNotBlockCount)
                    {
                        if (transactionInfo.Result.Valid)
                        {
                            tx.LastUpdateTime = DateTime.Now;
                            tx.MoveSysMinefee = GetMinefee(cryptocurrencyProvider, tx.UserToSysTXID);
                            tx.MoveStatus = MoveStatus.Finish;
                            await this._coinTransactionInBusiness.UpdateDataAsync(tx);
                        }
                    }
                    else
                    {
                        if (transactionInfo.Result.Confirmations >= coin.MinConfirms)
                        {
                            tx.LastUpdateTime = DateTime.Now;
                            tx.MoveSysMinefee = GetMinefee(cryptocurrencyProvider, tx.UserToSysTXID);
                            tx.MoveStatus = MoveStatus.Finish;
                            await this._coinTransactionInBusiness.UpdateDataAsync(tx);
                        }
                    }
                }
            }
        }

        private decimal GetMinefee(ICryptocurrencyProvider cryptocurrencyProvider, string txId)
        {
            var transactionResult = cryptocurrencyProvider.GetTransactionFee(txId, 1);
            if (string.IsNullOrEmpty(transactionResult.Error) && transactionResult.Result != -1)
            {
                decimal fee = Math.Abs(transactionResult.Result);
                return fee;
            }
            return 0;
        }

        /// <summary>
        /// 评估矿工费
        /// </summary>
        /// <param name="coin">虚拟币</param>
        /// <param name="from">发送地址</param>
        /// <param name="to">接收地址</param>
        /// <param name="amount">发送金额</param>
        /// <returns></returns>
        private async Task<ResponseData<decimal>> EstimateMinerfee(Coin coin, string from, string to, decimal amount)
        {
            var estimateGasData = new EstimateMinerfeeData
            {
                From = from,
                To = to,
                Amount = amount, 
                TokenAddress = coin.TokenCoinAddress 
            };
            var provider = await this._cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(coin);
            var estimateGasResult = provider.EstimateMinerfee(estimateGasData);
            return estimateGasResult;
        }
    }
}
