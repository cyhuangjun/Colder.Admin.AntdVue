using Autofac;
using CCPP.Cryptocurrency.Common;
using Coldairarrow.Business.Foundation;
using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.Enum;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Scheduler.Core;
using Coldairarrow.Scheduler.Dto;
using Exceptionless;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coldairarrow.Util;

namespace Coldairarrow.Scheduler.Job
{
    public class DepositSynchronizationJob : IJob
    {
        #region VARIABLE
        private const int MAXRETRYCOUNT = 4; 
        private const int COINPAGESIZE = 2000; 
        private static bool _isRun = false;
        /// <summary>
        /// 运行状态
        /// </summary>
        private static int _runStatus = 0;
        /// <summary>
        /// 虚拟币同步记录信息
        /// </summary>
        private static Dictionary<string, SyncCoinRecordDTO> _syncCoinRecords = new Dictionary<string, SyncCoinRecordDTO>();
        #endregion

        #region DI
        private readonly ILifetimeScope _container;
        private readonly ILogger<DepositSynchronizationJob> _logger;
        private readonly ICacheDataBusiness _cacheDataBusiness;
        private readonly ICoinSynchronizationBlockBusiness _coinSynchronizationBlockBusiness;
        private readonly ICryptocurrencyBusiness _cryptocurrencyBusiness;
        private readonly ISysWalletBusiness _sysWalletBusiness;
        private readonly IWalletBusiness _walletBusiness;
        private readonly ICoinTransactionInBusiness _coinTransactionInBusiness;
        private readonly ICoinConfigBusiness _coinConfigBusiness;
        #endregion

        public DepositSynchronizationJob()
        {
            this._container = RmesAutoFacModule.GetContainer();
            this._logger = this._container.Resolve<ILogger<DepositSynchronizationJob>>();
            this._cacheDataBusiness = this._container.Resolve<ICacheDataBusiness>();
            this._coinSynchronizationBlockBusiness = this._container.Resolve<ICoinSynchronizationBlockBusiness>();
            this._cryptocurrencyBusiness = this._container.Resolve<ICryptocurrencyBusiness>();           
            this._sysWalletBusiness = this._container.Resolve<ISysWalletBusiness>();
            this._walletBusiness = this._container.Resolve<IWalletBusiness>();
            this._coinTransactionInBusiness = this._container.Resolve<ICoinTransactionInBusiness>();
            this._coinConfigBusiness = this._container.Resolve<ICoinConfigBusiness>();
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
                    await SyncTransactionData();
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

        private async Task SyncTransactionData()
        {
            var coins = await this._cacheDataBusiness.GetCoinsAsync();
            if (coins == null || !coins.Any()) return;
            var syncCoins = coins.Where(f => string.IsNullOrEmpty(f.TokenCoinID));
            var syncBlocks = await this._coinSynchronizationBlockBusiness.GetDataListAsync(syncCoins.Select(e => e.Id));
            foreach (var syncCoin in syncCoins)
            {
                if (_syncCoinRecords[syncCoin.Id]?.IsRunning ?? false)
                    continue;
                _syncCoinRecords[syncCoin.Id] = new SyncCoinRecordDTO()
                {
                    CoinID = syncCoin.Id,
                    IsRunning = false,
                    Coin = syncCoin,
                    Record = syncBlocks.First(e => e.Id == syncCoin.Id)
                };
            }
            //最小间隔同步个数
            int minSpan = 5;
            foreach (var record in _syncCoinRecords)
            {
                if (record.Value.IsRunning) continue;
                var coin = record.Value.Coin;
                if (!string.IsNullOrEmpty(coin.TokenCoinID) || !coin.IsSupportWallet || !syncCoins.Any(e => e.Id == coin.Id)) continue;
                record.Value.IsRunning = true;
                Thread.MemoryBarrier();
                await Task.Factory.StartNew(async () =>
                {
                    int startBlock = 0;
                    var currentSyncBlock = record.Value.Record.LastBlockHeight;
                    try
                    {
                        var currentCoinSyncRecord = record.Value.Record;
                        #region
                        ResponseData<IList<CoinTransaction>> result = null;
                        var cryptocurrencyProvider = await this._cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(coin);                       
                        if (!cryptocurrencyProvider.IsUseBlockGetTransaction)  //通过交易记录获取交易信息
                        {
                            result = await GetTransaction(coin, 0, COINPAGESIZE, 4);
                            if (result == null || result.Result == null || !result.Result.Any())
                            {
                                if ((DateTime.Now - currentCoinSyncRecord.LastUpdateTime).TotalMinutes >= 5)
                                {
                                    currentCoinSyncRecord.LastUpdateTime = DateTime.Now;
                                   await SaveData(coin, null, currentCoinSyncRecord);
                                }
                                return;
                            }
                            currentCoinSyncRecord.LastUpdateTime = DateTime.Now;
                            await SaveTransaction(result, coin, currentCoinSyncRecord);
                        }
                        else //通过区块获取区块交易信息
                        {
                            if (currentCoinSyncRecord == null) return;                            
                            var blockResult = cryptocurrencyProvider.GetBlockCount();
                            if (!string.IsNullOrEmpty(blockResult.Error))
                            {
                                var blockCountException = new Exception($"获取区块高度出错:blockResult.Error");
                                blockCountException.ToExceptionless().Submit();
                            }
                            var currentBlockCount = blockResult.Result;
                            if (currentCoinSyncRecord.LastBlockHeight > startBlock)
                            {
                                startBlock = currentCoinSyncRecord.LastBlockHeight;
                            }
                            if (startBlock >= currentBlockCount) return;
                            
                            startBlock = startBlock - minSpan;
                            if (startBlock < 0)
                            {
                                startBlock = 0;
                            }
                            currentSyncBlock = startBlock;
                            List<CoinTransaction> newTXs = new List<CoinTransaction>();
                            for (int i = startBlock; i <= currentBlockCount; i++)
                            {
                                result = await GetTransaction(coin, i - 1, 1, 4);
                                if (result != null && result.Result != null && result.Result.Any())
                                {
                                    var txDatas = result.Result;
                                    foreach (var item in txDatas)
                                    {
                                        bool isAdd = false;
                                        if (item.IsUseTokenTransaction)
                                        {
                                            isAdd = newTXs.Any(f => f.Blockhash == item.Blockhash && f.Blockindex == item.Blockindex && f.TXId == item.TXId);
                                        }
                                        else
                                        {
                                            if (item.ContractTransaction != null)
                                            {
                                                isAdd = newTXs.Where(f => f.ContractTransaction != null).Any(f => f.Blockindex == item.Blockindex && f.TXId == item.TXId && f.Address == item.Address
                                                    && f.ContractTransaction.ToAddress == item.ContractTransaction.ToAddress
                                                    && f.ContractTransaction.ContractMethod == item.ContractTransaction.ContractMethod
                                                    && f.ContractTransaction.FromAddress == item.ContractTransaction.FromAddress
                                                    && f.ContractTransaction.TXID == item.ContractTransaction.TXID);
                                            }
                                            else
                                            {
                                                isAdd = newTXs.Any(f => f.Blockindex == item.Blockindex && f.TXId == item.TXId && f.Address == item.Address && f.Account == item.Account && f.Amount == item.Amount && f.FromAddress == item.FromAddress);
                                            }

                                        }

                                        if (isAdd)
                                        {
                                            continue;
                                        }
                                        newTXs.Add(item);
                                    }
                                }

                                if (newTXs.Count >= 6000 || i == currentBlockCount || (i - currentSyncBlock) >= 200)
                                {
                                    currentCoinSyncRecord.LastBlockHeight = i;
                                    currentCoinSyncRecord.LastUpdateTime = DateTime.Now;
                                    if (newTXs.Any())
                                    {
                                        var newResult = new ResponseData<IList<CoinTransaction>>
                                        {
                                            Result = newTXs
                                        };
                                        await SaveTransaction(newResult, coin, currentCoinSyncRecord);
                                        newTXs.Clear();
                                    }
                                    else
                                    {
                                        await SaveData(coin, null, currentCoinSyncRecord);
                                    }
                                    currentSyncBlock = i;
                                }
                            }
                            currentCoinSyncRecord.LastBlockHeight = currentBlockCount;
                            currentCoinSyncRecord.LastUpdateTime = DateTime.Now;
                            await SaveData(coin, null, currentCoinSyncRecord);
                            currentSyncBlock = currentBlockCount;
                        } 
                        #endregion
                    }
                    catch (Exception e)
                    {
                        var syncRecord = record.Value;
                        if (syncRecord == null || (DateTime.Now - syncRecord.NotificateErrorTime).TotalMinutes > 2)
                        {
                            _logger.LogError(e, e.Message);
                            ExceptionlessClient.Default.CreateException(new Exception($"同步区块异常币种:{coin.Code},异常信息:{JsonConvert.SerializeObject(e)}")).Submit();
                            syncRecord.NotificateErrorTime = DateTime.Now;
                        }
                    }
                    finally
                    {
                        record.Value.IsRunning = false;
                        if (record.Value.Record != null)
                        {
                            record.Value.Record.LastBlockHeight = currentSyncBlock;
                            record.Value.Record.LastUpdateTime = DateTime.Now;
                        }
                        Thread.MemoryBarrier();
                    }
                });
            }
        }

        private async Task<ResponseData<IList<CoinTransaction>>> GetTransaction(Coin coin, int skip, int pageSize, int maxRetryCount = 3)
        {
            ResponseData<IList<CoinTransaction>> result = null;
            int retryCount = 0;
            do
            {
                try
                {
                    var cryptocurrencyProvider = await this._cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(coin);
                    result = cryptocurrencyProvider.GetTransaction("*", pageSize, skip);
                    break;
                }
                catch (Exception e)
                {
                    retryCount++;
                    if (retryCount >= maxRetryCount)
                    {
                        this._logger.LogError(e, $"{e.Message} 虚拟币:{coin.Code},异常高度块：{skip},ex:{JsonConvert.SerializeObject(e)}");
                        ExceptionlessClient.Default.CreateException(new Exception($"虚拟币:{coin.Code},异常高度块：{skip}")).Submit();
                        e.ToExceptionless().Submit();
                        throw e;
                    }
                }
            } while (true);
            return result;
        }
           
        private CoinContranctTransactionIn GetCoinContranctTransactionIn(ContractTransaction rawContractTransaction, string coinID, DateTime dateTime)
        {
            var contractTransaction = new CoinContranctTransactionIn
            {
                BlockHash = rawContractTransaction.BlockHash,
                BlockHeight = rawContractTransaction.BlockHeight,
                ContractMethod = rawContractTransaction.ContractMethod,
                FromAddress = rawContractTransaction.FromAddress,
                Input = rawContractTransaction.Input,
                CoinID = coinID,
                Status = CoinContranctTransactionStatus.WaitSync,
                ID = Guid.NewGuid().GuidTo16String(),
                ToAddress = rawContractTransaction.ToAddress,
                CreateTime = dateTime,
                LastUpdateTime = dateTime,
                TXID = rawContractTransaction.TXID,
                ContractAddress = rawContractTransaction.ContractAddress
            };
            return contractTransaction;
        }

        private async Task SaveTransaction(ResponseData<IList<CoinTransaction>> transactionResult, Coin coin, CoinSynchronizationBlock currentCoinSyncRecord)
        {
            if (transactionResult?.Result?.Any() ?? false) return;            
            int retryCount = 0;
            List<Coin> coins = await this._cacheDataBusiness.GetCoinsAsync();
            List<SysWallet> sysWallets = await this._sysWalletBusiness.GetListAsync(null);
            var transactionList = transactionResult.Result;
            do
            {
                #region 用户充币
                try
                {
                    var walletTagList = transactionList.Where(f => f != null && !string.IsNullOrEmpty(f.Address)).Select(f => f.Address).Distinct().ToList();
                    var contractAddressList = transactionList.Where(f => f != null && f.ContractTransaction != null && !string.IsNullOrEmpty(f.ContractTransaction.ToAddress)).Select(f => f.ContractTransaction.ToAddress).Distinct().ToList();
                    if (contractAddressList != null && contractAddressList.Any())
                    {
                        walletTagList.AddRange(contractAddressList);
                        walletTagList = walletTagList.Distinct().ToList();
                    }
                    var tokenAddresList = transactionList.Where(f => f != null && f.IsUseTokenTransaction && f.TokenTransactions != null)
                        .SelectMany(f => f.TokenTransactions).ToList().Where(a => !string.IsNullOrEmpty(a.Address)).Select(a => a.Address).Distinct().ToList();
                    if (tokenAddresList.Any())
                    {
                        walletTagList.AddRange(tokenAddresList);
                    }
                    var coinID = string.IsNullOrEmpty(coin.TokenCoinID) ? coin.Id : coin.TokenCoinID;
                    var userWalletList = await this._walletBusiness.GetListAsync(e => e.CoinID == coinID && walletTagList.Contains(e.Address));    
                    var transactionIDList = transactionList.Select(f => f.TXId).Distinct().ToList();
                    var addTransactionList = await this._coinTransactionInBusiness.GetListAsync(e => transactionIDList.Contains(e.TXID) && walletTagList.Contains(e.Address));
                         
                    var coinTransactionList = new List<CoinTransactionIn>();
                    //契约相关的交易信息
                    var contractTransactionList = new List<CoinContranctTransactionIn>();
                    var now = DateTime.Now;
                    foreach (var item in transactionList)
                    {
                        #region
                        if (item.Generated || string.IsNullOrEmpty(item.Category)) continue;
                        TransactionCategory categoryType = (TransactionCategory)Enum.Parse(typeof(TransactionCategory), item.Category, true);
                        if (categoryType != TransactionCategory.Receive) continue;
                        if (sysWallets.Any(e => e.Address.Equals(item.FromAddress, StringComparison.InvariantCultureIgnoreCase))) continue;
                        //使用主交易信息
                        if (!item.IsUseTokenTransaction)
                        {
                            string userID = string.Empty;
                            string uid = string.Empty;
                            string address = string.Empty;
                            var userWallet = userWalletList.Where(f => f.Address.Equals(item.Address, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (userWallet != null)
                            {
                                address = userWallet.Address;
                                userID = userWallet.UserID;
                                uid = userWallet.UID;
                            }
                            if (string.IsNullOrEmpty(address) && item.ContractTransaction != null)
                            {
                                bool isAddressExists = userWalletList.Any(f => f.Address.Equals(item.ContractTransaction.ToAddress, StringComparison.InvariantCultureIgnoreCase));
                                if (!isAddressExists) continue;
                                var contractTransaction = GetCoinContranctTransactionIn(item.ContractTransaction, coin.Id, now);
                                contractTransactionList.Add(contractTransaction);
                                continue;
                            }
                            var hasAdd = addTransactionList.Any(f => f.TXID == item.TXId && f.Address.Equals(item.Address, StringComparison.InvariantCultureIgnoreCase));
                            if (hasAdd) continue;
                            //创建的地址是虚拟地址，需要使用系统钱包进行判断
                            if (coin.IsVirtualAddress)
                            {
                                if (sysWallets == null || !sysWallets.Any())
                                {
                                    (new Exception($"找不到系统钱包地址相关信息: coin code:{coin.Code},TXID:{item.TXId}")).ToExceptionless().Submit();
                                    continue;
                                }
                                var hasExistAddress = sysWallets.Any(f => f.Address.Equals(item.Account, StringComparison.InvariantCultureIgnoreCase) && f.CoinID == coin.Id);
                                if (!hasExistAddress)
                                    continue;
                            }
                            var transaction = new CoinTransactionIn
                            {
                                ID = Guid.NewGuid().GuidTo16String(),
                                Account = item.Account,
                                Address = item.Address,
                                Amount = item.Amount,
                                FromAddress = item.FromAddress,
                                Confirmations = (int)item.Confirmations,
                                BlockIndex = (int)item.Blockindex,
                                BlockTime = item.BlockTime.ToDateTime_From_JsGetTime(),
                                CoinID = coin.Id,
                                CreateTime = now,
                                UserID = userID,
                                TXID = item.TXId,
                                Timereceived = item.TimeReceived.ToDateTime_From_JsGetTime(),
                                Status = TransactionStatus.WaitConfirm,
                                LastUpdateTime = now
                            };
                            coinTransactionList.Add(transaction);
                        }
                        else  //使用代币交易记录
                        {
                            if (item.TokenTransactions != null && item.TokenTransactions.Any())
                            {
                                foreach (var tokenRecord in item.TokenTransactions)
                                {
                                    string toAddress = string.IsNullOrEmpty(tokenRecord.Address) ? item.Address : tokenRecord.Address;
                                    var userWallet = userWalletList.FirstOrDefault(f => f.Address == toAddress);
                                    if (userWallet == null) continue;
                                    string userID = userWallet.UserID;
                                    var hasAdd = addTransactionList.Any(f => f.TXID == item.TXId && f.Address.Equals(toAddress, StringComparison.InvariantCultureIgnoreCase));
                                    if (hasAdd) continue;
                                    Coin tokenCoin = null;
                                    if (tokenRecord.IsMainCoin)
                                    {
                                        tokenCoin = coin;
                                    }
                                    else
                                    {
                                        tokenCoin = coins.FirstOrDefault(f => f.TokenCoinAddress == tokenRecord.CoinNamespace && f.TokenCoinKey == tokenRecord.CoinKey && (f.TokenCoinID == coin.Id || (string.IsNullOrEmpty(f.TokenCoinID) && f.Id == coin.Id)));
                                    }
                                    if (tokenCoin == null)
                                    {
                                        (new Exception($"虚拟币信息不存在:记录信息：CoinNamespace:{tokenRecord.CoinNamespace},CoinKey:{tokenRecord.CoinKey},TXID:{item.TXId}")).ToExceptionless().Submit();
                                        continue;
                                    }
                                    //创建的地址是虚拟地址，需要使用系统钱包进行判断
                                    if (tokenCoin.IsVirtualAddress)
                                    {
                                        if (sysWallets == null || !sysWallets.Any())
                                        {
                                            (new Exception($"找不到系统钱包地址相关信息:记录信息：CoinNamespace:{tokenRecord.CoinNamespace},CoinKey:{tokenRecord.CoinKey},TXID:{item.TXId}")).ToExceptionless().Submit();
                                            continue;
                                        }
                                        var hasExistAddress = sysWallets.Any(f => f.Address.Equals(tokenRecord.Account, StringComparison.InvariantCultureIgnoreCase) && (f.CoinID == tokenCoin.Id || (f.CoinID == tokenCoin.TokenCoinID && !string.IsNullOrEmpty(tokenCoin.TokenCoinID))));
                                        if (!hasExistAddress) continue;
                                    }
                                    var tokenTransactionIn = new CoinTransactionIn
                                    {
                                        ID = Guid.NewGuid().GuidTo16String(),
                                        CreateTime = now,
                                        UserID = userID,
                                        TXID = item.TXId,
                                        Timereceived = item.TimeReceived.ToDateTime_From_JsGetTime(),
                                        Status = TransactionStatus.WaitConfirm,
                                        LastUpdateTime = now,
                                        Account = string.IsNullOrEmpty(tokenRecord.Account) ? item.Account : tokenRecord.Account,
                                        Address = string.IsNullOrEmpty(tokenRecord.Address) ? item.Address : tokenRecord.Address,
                                        FromAddress = string.IsNullOrEmpty(tokenRecord.FromAddress) ? item.FromAddress : tokenRecord.FromAddress,
                                        Amount = tokenRecord.Quantity,
                                        Confirmations = (int)item.Confirmations,
                                        BlockIndex = (int)item.Blockindex,
                                        BlockTime = item.BlockTime.ToDateTime_From_JsGetTime(),
                                        CoinID = tokenCoin.Id
                                    };
                                    coinTransactionList.Add(tokenTransactionIn);
                                }
                            }
                        }
                        if (item.ContractTransaction != null)
                        {
                            var rawContractTransaction = item.ContractTransaction;
                            var contractTransaction = GetCoinContranctTransactionIn(rawContractTransaction, coin.Id, now);
                            contractTransactionList.Add(contractTransaction);
                        }
                        #endregion
                    }
                    bool flag = coinTransactionList.Any();
                    if (flag)
                    {
                        var coinIDList = coinTransactionList.Select(f => f.CoinID).Distinct().ToList();
                        var userIDList = coinTransactionList.Select(f => f.UserID).Distinct().ToList();
                        foreach (var transaction in coinTransactionList)
                        {
                            var tranCoin = coins.First(e=>e.Id == transaction.CoinID);
                            var rechargeConfig = await this._coinConfigBusiness.MinAmountAsync(transaction.UserID, tranCoin.Code); 
                            if (rechargeConfig != null)
                            {
                                if (transaction.Amount < rechargeConfig.MinPaymentAmount)
                                {
                                    transaction.Status = TransactionStatus.NotArriveLessInMinAmount;
                                }
                            }
                        }
                    } 
                    await SaveData(coin, coinTransactionList, currentCoinSyncRecord, contractTransactionList); 
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    retryCount++;
                    if (retryCount >= MAXRETRYCOUNT)
                    { 
                        e.ToExceptionless().Submit();
                        throw e;
                    }
                }
                #endregion
            } while (true);
        }

        private async Task SaveData(Coin coin, List<CoinTransactionIn> coinTransactionList, CoinSynchronizationBlock coinSynchronizationBlock, List<CoinContranctTransactionIn> coinContranctTransactionIns = null)
        { 
            var result = await this._coinTransactionInBusiness.SyncCoinTransactionIn(coinTransactionList, coinContranctTransactionIns);
            if (!result.Success)
            {
                throw new Exception("保存交易记录失败");
            }
           await this._coinSynchronizationBlockBusiness.UpdateDataAsync(coinSynchronizationBlock);
        }
    }
}
