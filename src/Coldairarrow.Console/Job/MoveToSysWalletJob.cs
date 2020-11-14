using Autofac;
using CCPP.Cryptocurrency.Common;
using Coldairarrow.Business.Foundation;
using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.Enum;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Scheduler.Core;
using Coldairarrow.Util;
using Exceptionless;
using Microsoft.Extensions.Logging;
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
        #endregion

        public MoveToSysWalletJob()
        {
            this._container = RmesAutoFacModule.GetContainer();
            this._logger = this._container.Resolve<ILogger<ConfirmTransactionSynchronizationJob>>();
            this._cacheDataBusiness = this._container.Resolve<ICacheDataBusiness>();
            this._coinTransactionInBusiness = this._container.Resolve<ICoinTransactionInBusiness>();
            this._cryptocurrencyBusiness = this._container.Resolve<ICryptocurrencyBusiness>();
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
            var coins = (await this._cacheDataBusiness.GetCoinsAsync()).Where(e=>e.IsUseSysAccount && e.IsSupportWallet);

            var datas = _coinTransactionSyncInFeeRepository.Table.Where(f => (f.Status == CoinTransactionSyncInStatus.WaitConfirm || f.Status == CoinTransactionSyncInStatus.WaitSync))
                .OrderBy(f => f.LastUpdateTime).Take(ConstDefine.MaxPageSize).ToList();
            if (!datas.Any())
            {
                return;
            }

            var coinIds = datas.Select(f => f.CoinID).Distinct().ToList();
            var coinList = _coinRepository.GetList(f => f.IsSupportWallet);
            var tokenCoinIds = coinList.Where(f => !string.IsNullOrEmpty(f.TokenCoinID)).Select(f => f.TokenCoinID).Distinct().ToList();
            coinIds.AddRange(tokenCoinIds);
            var addressList = datas.Where(f => coinIds.Contains(f.CoinID)).Select(f => f.FromAddress).ToList();
            var userIDList = datas.Select(f => f.UserID).Distinct().ToList();
            var customerWalletList = _customerWalletRepository.GetList(f => coinIds.Contains(f.CoinID) && userIDList.Contains(f.UserID));
            var sysWalletList = _sysWalletRepository.Table.AsNoTracking().Where(f => f.IsEnable && coinIds.Contains(f.CoinID)).ToList();
            var transactionIDList = datas.Select(f => f.TXID).Distinct().ToList();
            var transactionList = _coinTransactionInRepository.Table.AsNoTracking().Where(f => transactionIDList.Contains(f.TXID)).ToList();
            var groupTransactions = datas.GroupBy(f => f.CoinID);
            var newTransactionFees = new List<CoinTransactionFee>();
            var newSysWalletLogs = new List<SysWalletLog>();
            foreach (var item in groupTransactions)
            {
                var coin = coinList.Find(f => f.ID == item.Key);
                if (coin == null)
                {
                    continue;
                }
                string sysCoinID = item.Key;
                if (!string.IsNullOrEmpty(coin.TokenCoinID))
                {
                    sysCoinID = coin.TokenCoinID;
                }
                var sysWallets = sysWalletList.Where(f => f.CoinID == sysCoinID).OrderBy(f => f.CreateTime).ToList();
                if (!sysWallets.Any())
                {
                    continue;
                }
                var providerType = (ProviderType)coin.ProviderType;
                var coinProvider = VirtualCoinDomain.GetCoinProvider(coin);
                var sysWallet = sysWallets.First();
                foreach (var tx in item)
                {
                    tx.LastUpdateTime = DateTime.Now;
                    try
                    {
                        if (tx.Status == CoinTransactionSyncInStatus.WaitConfirm)
                        {
                            #region 处理待确认的记录信息
                            if (string.IsNullOrEmpty(tx.MoveTXID))
                            {
                                continue;

                            }
                            if (coinProvider.HasTransactionReceipt)
                            {
                                var result = coinProvider.GetTransactionReceipt(tx.MoveTXID);
                                if (result.Result != null)
                                {
                                    if (result.Result.IsSuccess)
                                    {
                                        tx.Status = CoinTransactionSyncInStatus.Confirm;
                                    }
                                }
                            }
                            else
                            {
                                var transactionInfo = coinProvider.GetTransaction(tx.MoveTXID);
                                if (transactionInfo.Result != null)
                                {
                                    if (coinProvider.HasNotBlockCount)
                                    {
                                        if (transactionInfo.Result.Valid)
                                        {
                                            tx.Status = CoinTransactionSyncInStatus.Confirm;
                                        }
                                    }
                                    else
                                    {
                                        if (transactionInfo.Result.Confirmations >= coin.MinConfirms)
                                        {
                                            tx.Status = CoinTransactionSyncInStatus.Confirm;

                                        }
                                    }

                                }
                            }
                            #endregion
                        }
                        else if (tx.Status == CoinTransactionSyncInStatus.WaitSync)
                        {
                            //找到当前用户的钱包信息
                            CustomerWallet customerWallet = null;
                            if (coinProvider.IsAddressOnce)
                            {
                                customerWallet = customerWalletList.FirstOrDefault(f => f.UserID == tx.UserID && f.CoinID == sysCoinID);
                            }
                            else
                            {
                                customerWallet = customerWalletList.FirstOrDefault(f => f.Address == tx.FromAddress && f.UserID == tx.UserID && f.CoinID == sysCoinID);
                            }
                            decimal moveAmount = tx.Amount;
                            if (customerWallet != null)
                            {
                                string securityKey = customerWallet.SecurityKey;
                                if (!string.IsNullOrEmpty(securityKey) && customerWallet.IsEncryption == true)
                                {
                                    securityKey = CustomerWalletEncryptionDomain.Decode(securityKey, customerWallet.UserID);
                                }

                                var estimateGasData = new EstimateGasData
                                {
                                    From = customerWallet.Address,
                                    To = sysWallet.Address,
                                    Price = 0,
                                    TokenAddress = coin.TokenCoinAddress,
                                    WalletSupplyAmount = coin.WalletSupplyAmount,
                                    Amount = tx.Amount
                                };
                                string seed = string.Empty;
                                if (!string.IsNullOrEmpty(customerWallet.Seed))
                                {
                                    seed = CustomerWalletEncryptionDomain.Decode(customerWallet.Seed, customerWallet.UserID);
                                }
                                var balanceData = new BalanceData
                                {
                                    Address = customerWallet.Address,
                                    Seed = seed
                                };
                                if (coinProvider.IsAddressOnce)
                                {
                                    var addressLogs = _customerWalletLogRepository.Table.AsNoTracking().Where(f => f.CoinID == customerWallet.CoinID && f.UserID == customerWallet.UserID).OrderByDescending(f => f.CreateTime).Select(f => f.Address).Distinct().Take(500).ToList();
                                    if (addressLogs != null && addressLogs.Any())
                                    {
                                        balanceData.Address = string.Join(',', addressLogs);
                                    }
                                }
                                var balanceResult = coinProvider.GetBalance(balanceData);
                                if (balanceResult.Result < tx.Amount)
                                {
                                    continue;
                                }
                                var estimateGasResult = coinProvider.EstimateGas(estimateGasData);
                                if (!string.IsNullOrEmpty(estimateGasResult.Error))
                                {
                                    continue;
                                }
                                //花费的费用
                                var estimateGasInfo = estimateGasResult.Result;

                                if (!string.IsNullOrEmpty(coin.RelationCoinID))
                                {
                                    moveAmount = tx.Amount;
                                }
                                else
                                {
                                    moveAmount = tx.Amount - estimateGasInfo.NeedAmount;
                                }

                                if (coinProvider.IsNeedRegisterAccount)
                                {
                                    //注册钱包地址
                                    if (!customerWallet.IsRegisterAddress.HasValue || !customerWallet.IsRegisterAddress.Value)
                                    {
                                        var registerResult = coinProvider.RegisterAccount(customerWallet.Address);
                                        if (registerResult != null && registerResult.Result != null)
                                        {
                                            var registerAccount = registerResult.Result;
                                            var registerAddressFee = new CoinTransactionFee
                                            {
                                                CoinID = coin.ID,
                                                ID = Guid.NewGuid().GuidTo16String(),
                                                MinerCoinID = coin.ID,
                                                CreateTime = DateTime.Now,
                                                TXID = registerAccount.TX,
                                                MinerFee = registerAccount.Fee,
                                                CreatorID = ConstDefine.SupperUserId,
                                                Status = CoinTransactionFeeStatus.SyncFinish,
                                                TransactionType = CoinTransactionType.RegisterAddress,
                                                UserID = tx.UserID,
                                                ExtraTaxFee = coin.ExtraTaxFee ?? 0
                                            };
                                            newTransactionFees.Add(registerAddressFee);
                                            customerWallet.IsRegisterAddress = true;
                                        }
                                    }
                                }
                                string sysAddress = string.Empty;

                                //地址只能使用一次
                                if (coinProvider.IsAddressOnce)
                                {
                                    #region
                                    string sysSeed = string.Empty;
                                    if (!string.IsNullOrEmpty(sysWallet.Seed))
                                    {
                                        sysSeed = CustomerWalletEncryptionDomain.Decode(sysWallet.Seed, sysWallet.Address);
                                    }
                                    var newSysAddress = coinProvider.AddWalletAddress(new AddAddressInfo
                                    {
                                        IsActive = true,
                                        Seed = sysSeed
                                    }).Result;
                                    var sysWalletLog = new SysWalletLog
                                    {
                                        ID = Guid.NewGuid().GuidTo16String(),
                                        Address = newSysAddress.Address,
                                        CoinID = coin.ID,
                                        CreateTime = DateTime.Now,
                                        CreatorID = ConstDefine.SupperUserId,
                                        IsEnable = false
                                    };
                                    if (!string.IsNullOrEmpty(newSysAddress.PrivateKey))
                                    {
                                        sysWalletLog.PrivateKey = CustomerWalletEncryptionDomain.Encode(newSysAddress.PrivateKey, newSysAddress.Address);
                                    }
                                    if (!string.IsNullOrEmpty(newSysAddress.PublicKey))
                                    {
                                        sysWalletLog.PublicKey = CustomerWalletEncryptionDomain.Encode(newSysAddress.PublicKey, newSysAddress.Address);
                                    }
                                    sysAddress = sysWalletLog.Address;
                                    newSysWalletLogs.Add(sysWalletLog);
                                    #endregion
                                }
                                else
                                {
                                    sysAddress = sysWallet.Address;
                                }
                                var sendCoinInfo = new SendCoinInfo
                                {
                                    Coin = Mapper.Map<Coin, CoinDTO>(coin),
                                    Quantity = moveAmount,
                                    ToAddress = sysAddress,
                                    SysWalletEstimateGas = new DTO.C2C.SysWalletEstimateGasInfoDTO
                                    {
                                        EstimateGas = estimateGasInfo.EstimateGas,
                                        GasPrice = estimateGasInfo.GasPrice,
                                        SysWallet = sysWallet
                                    }
                                };
                                var moveTXID = _virtualCoinDomain.SendCoinToSys(sendCoinInfo, customerWallet)?.Data;
                                if (!string.IsNullOrEmpty(moveTXID))
                                {
                                    var txInfo = transactionList.FirstOrDefault(f => f.TXID == tx.TXID && f.CoinID == tx.CoinID && f.UserID == tx.UserID);
                                    if (txInfo != null)
                                    {
                                        string minerCoinID = string.IsNullOrEmpty(coin.RelationCoinID) ? txInfo.CoinID : coin.RelationCoinID;
                                        CoinTransactionFee coinTransactionFee = new CoinTransactionFee
                                        {
                                            Amount = txInfo.Amount,
                                            CoinID = txInfo.CoinID,
                                            UserID = txInfo.UserID,
                                            TXID = txInfo.TXID,
                                            MinerCoinID = minerCoinID,
                                            CreateTime = DateTime.Now,
                                            CreatorID = ConstDefine.SupperUserId,
                                            ID = Guid.NewGuid().GuidTo16String(),
                                            TransactionType = CoinTransactionType.UserToSys,
                                            PlatformFee = txInfo.Fee ?? 0,
                                            MoveTXID = moveTXID,
                                            MoveSystemAddress = sysWallet.Address,
                                            MoveAmount = moveAmount,
                                            Status = CoinTransactionFeeStatus.WaitConfirm,
                                            ExtraTaxFee = coin.ExtraTaxFee ?? 0,
                                            WebSiteID = txInfo.WebSiteID
                                        };
                                        newTransactionFees.Add(coinTransactionFee);
                                    }
                                    tx.MoveTXID = moveTXID;
                                    tx.ToAddress = sysWallet.Address;
                                    tx.Status = CoinTransactionSyncInStatus.WaitConfirm;
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (tx.LastUpdateTime.HasValue && (tx.LastUpdateTime.Value - tx.CreateTime).TotalHours > 24)
                        {
                            tx.Status = CoinTransactionSyncInStatus.Invalid;
                        }
                        Console.WriteLine($"err:{JsonConvert.SerializeObject(ex)}");
                        Task.Factory.StartNew(() =>
                        {
                            ex.ToExceptionless().Submit();
                        });

                    }

                }
            }
            if (newTransactionFees.Any())
            {
                this._coinTransactionFeeRepository.Add(newTransactionFees, false);
            }
            this._coinTransactionSyncInFeeRepository.Update(datas, false);
            this._coinTransactionSyncInFeeRepository.SaveChanges(false);
            if (newSysWalletLogs != null && newSysWalletLogs.Any())
            {
                this._sysWalletLogRepository.Add(newSysWalletLogs, false);
            }
            if (customerWalletList != null && customerWalletList.Any())
            {
                this._customerWalletRepository.Update(customerWalletList, false);
            }
            this._customerWalletRepository.SaveChanges(false);
        }
    }
}
