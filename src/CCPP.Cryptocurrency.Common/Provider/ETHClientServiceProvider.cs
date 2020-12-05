using CCPP.Cryptocurrency.Common.ETH;
using Coldairarrow.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CCPP.Cryptocurrency.Common.Provider
{
    public class ETHClientServiceProvider : BaseClientServiceProvider, ICryptocurrencyProvider
    { 
        /// <summary>
        /// 锲约方法长度
        /// </summary>
        private const int ContractMethodLength = 10;
        private const int ToAddressLength = 64;
        private const int AmountLength = 64;
        /// <summary>
        /// 公式转换精度值
        /// </summary>
        protected static int WeiPrecision = 18;
        /// <summary>
        /// 消耗Gas放大值
        /// </summary>
        protected static decimal GasMultiple = 3M;
        public int Digits
        {
            get
            {
                if (!this._configuration.Precision.HasValue)
                {
                    return _configuration.Precision.Value;
                }
                return 18;
            }
        }
        /// <summary>
        /// 是否有获取凭证信息
        /// </summary>
        public virtual bool HasTransactionReceipt => true;
        /// <summary>
        /// 没有区块高度信息
        /// </summary>
        public virtual bool HasNotBlockCount => false;
        /// <summary>
        /// 是否是代币
        /// </summary>
        public virtual bool IsToken => false;

        /// <summary>
        /// 是否需要钱包地址进行加密
        /// </summary>
        public bool IsNeedSecurityKey => true;
        /// <summary>
        /// 是否通过区块获取交易记录
        /// </summary>
        public bool IsUseBlockGetTransaction => true;
        public virtual bool IsNeedSysAddress => false;
        /// <summary>
        /// 账号提币是否需要注册
        /// </summary>
        public virtual bool IsNeedRegisterAccount => false;

        public virtual bool IsUseAddressIdentity => false; 
        public ETHClientServiceProvider(Configuration configuration) : base(configuration)
        {

        }
        public ResponseData<GenerateAddressResult> CreateWalletAddress(CreateWalletAddressInput createAddressInfo)
        {
            var data = this.InvokeMethod<string>(ETHRPCMethod.AddWalletAddress, DefaultApiName, createAddressInfo.SecurityKey);
            var result = new ResponseData<GenerateAddressResult>
            {
                Error = data.Error,
                Result = new GenerateAddressResult
                {
                    Address = data.Result
                }
            };
            return result;
        }
        
        /// <summary>
        /// 获取区块数量
        /// </summary>
        /// <returns></returns>
        public ResponseData<int> GetBlockCount()
        {
            var requestResult = this.InvokeMethod<string>(ETHRPCMethod.GetBlockCount, DefaultApiName);
            ResponseData<int> result = new ResponseData<int>
            {
                Error = requestResult.Error,
                Result = Convert.ToInt32(requestResult.Result, 16)
            };
            return result;
        }
        /// <summary>
        /// 获取对映区块的Hash值
        /// </summary>
        /// <param name="height">区块高度</param>
        /// <returns></returns>
        public ResponseData<string> GetBlockHash(long height)
        {
            return this.InvokeMethod<string>(ETHRPCMethod.GetBlockHash, DefaultApiName, height);
        }
        /// <summary>
        /// 转币
        /// </summary>
        /// <returns>转账成功返回交易ID</returns>
        public virtual ResponseData<string> SendTransaction(SendCoinData data)
        {
            if (string.IsNullOrEmpty(data.FromAccount))
            {
                data.FromAccount = this._configuration.SysAddress.Address;
                data.FromAccountPassword = this._configuration.SysAddress.Password;
            }
            int precision = this._configuration.Precision ?? WeiPrecision;
            var sendTransaction = new SendTransaction
            {
                From = data.FromAccount,
                To = data.ToCoinAddress,
                Value = data.Amount.BigToUnitHex(precision),
                Data = ""
            };
            sendTransaction.GasPrice = this._configuration.GasPrice.LongDECToHEX();
            var gas = this.EstimateGas(new EstimateMinerfeeData() { Amount = data.Amount, From = data.FromAccount, To = data.ToCoinAddress });
            sendTransaction.Gas = gas.LongDECToHEX();
            return this.InvokeMethod<string>(ETHRPCMethod.SendFrom, DefaultApiName, sendTransaction, data.FromAccountPassword);
        }
               
        /// <summary>
        /// 获取对映区块信息
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public ResponseData<CoinBlock> GetBlockInfo(long height)
        {
            var requestResult = this.InvokeMethod<BlockInfo>(ETHRPCMethod.GetBlockByHeight, DefaultApiName, height.ConvertToHex(), true);
            var blockData = requestResult.Result;
            var blockResult = GetBlockCount();
            ResponseData<CoinBlock> result = new ResponseData<CoinBlock>
            {
                Error = requestResult.Error
            };

            if (blockData != null)
            {
                CoinBlock coinBlock = new CoinBlock
                {
                    Hash = blockData.Hash,
                    Height = blockData.Number.GetInt64FromHex(),
                    Nonce = blockData.Nonce.GetInt64FromHex(),
                    Size = blockData.Size.GetInt64FromHex(),
                    Time = blockData.Timestamp.GetIntFromHex(),
                    PreviousblockHash = blockData.ParentHash
                };
                coinBlock.Confirmations = blockResult.Result - coinBlock.Height + 1;
                if (blockData.Transactions != null && blockData.Transactions.Any())
                {
                    coinBlock.Tx = blockData.Transactions.Select(f => f.Hash).ToList();
                    List<CoinTransaction> transactions = new List<CoinTransaction>();
                    List<ContractTransaction> contractTransactions = new List<ContractTransaction>();
                    foreach (var item in blockData.Transactions)
                    {
                        CoinTransaction coinTransaction = ConvertToCoinTransaction(item);
                        coinTransaction.Confirmations = coinBlock.Confirmations;
                        coinTransaction.BlockTime = coinBlock.Time;
                        coinTransaction.Time = coinBlock.Time;
                        coinTransaction.TimeReceived = coinBlock.Time;
                        transactions.Add(coinTransaction);
                        //锲约交易信息
                        if (coinTransaction.ContractTransaction != null)
                        {
                            contractTransactions.Add(coinTransaction.ContractTransaction);
                        }
                    }
                    coinBlock.Transactions = transactions;
                    coinBlock.ContractTransactions = contractTransactions;
                }
                result.Result = coinBlock;
            }
            return result;
        }
         
        /// <summary>
        /// 获取交易信息
        /// </summary>
        /// <param name="accountTag">账号标签</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="skipCount">跳过多少页,根据区块进行加载</param>
        /// <returns></returns>
        public ResponseData<IList<CoinTransaction>> GetTransaction(string accountTag, int pageSize, int pageIndexOrSkipCount)
        {
            ResponseData<IList<CoinTransaction>> result = new ResponseData<IList<CoinTransaction>>();
            long endIndex = pageSize + pageIndexOrSkipCount;
            List<CoinTransaction> transactions = new List<CoinTransaction>();
            for (long i = pageIndexOrSkipCount; i <= endIndex; i++)
            {
                var blockResult = this.GetBlockInfo(i);
                if (blockResult.Result != null)
                {
                    var blockTransactions = blockResult.Result.Transactions;
                    if (blockTransactions != null)
                    {
                        transactions.AddRange(blockTransactions);
                    }

                }
            }
            result.Result = transactions;
            return result;
        }
        /// <summary>
        /// 获取账号余额
        /// </summary>
        /// <param name="accountTag">标签</param>
        /// <returns></returns>
        public virtual ResponseData<decimal> GetBalance(BalanceData data)
        {
            var balanceRequest = this.InvokeMethod<string>(ETHRPCMethod.GetBalance, DefaultApiName, data.Address, "latest");
            int precision = this._configuration.Precision ?? WeiPrecision;
            ResponseData<decimal> result = new ResponseData<decimal>
            {
                Error = balanceRequest.Error,
                Result = balanceRequest.Result.UnitToBig(precision)
            };
            return result;
        }

        /// <summary>
        /// 获取钱包所有可用余额
        /// </summary>
        /// <param name="confirm">最小确认数</param>
        /// <returns></returns>
        public virtual ResponseData<decimal> GetAllBalance(int confirm)
        {
            return GetBalance(new BalanceData()
            {
                Address = this._configuration.SysAddress.Address
            });
        }

        /// <summary>
        /// 获取单笔交易记录信息
        /// </summary>
        /// <param name="txid">交易ID</param>
        /// <returns></returns>
        public ResponseData<CoinTransaction> GetTransaction(string txid)
        {
            var requestResult = this.InvokeMethod<TransactionInfo>(ETHRPCMethod.GetTransaction, DefaultApiName, txid);
            var transactionData = requestResult.Result;
            if (transactionData == null)
            {
                return null;
            }
            CoinTransaction coinTransaction = ConvertToCoinTransaction(transactionData);
            if (coinTransaction.Blockindex > 0)
            {
                var blockResult = this.GetBlockInfo(coinTransaction.Blockindex);
                var block = blockResult.Result;
                if (block != null)
                {
                    coinTransaction.Confirmations = block.Confirmations;
                    coinTransaction.BlockTime = block.Time;
                    coinTransaction.TimeReceived = block.Time;
                }
            }

            ResponseData<CoinTransaction> result = new ResponseData<CoinTransaction>
            {
                Error = requestResult.Error,
                Result = coinTransaction
            };
            return result;
        }
        /// <summary>
        /// 获取原始交易数据信息
        /// </summary>
        /// <param name="txid">交易编号</param>
        /// <returns></returns>
        public ResponseData<RawTransaction> GetRawTransaction(string txid)
        {
            return this.InvokeMethod<RawTransaction>(ETHRPCMethod.GetRawTransaction, DefaultApiName, txid, 1);
        }
        /// <summary>
        /// 获取未花费的交易
        /// </summary>
        /// <returns></returns>
        public ResponseData<IList<UnspentTransaction>> GetUnspentTransaction(params string[] addressList)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// 校验钱包地址
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <returns></returns>
        public ResponseData<ValidateAddress> ValidateAddress(string address)
        {
            ValidateAddress validateAddress = new Common.ValidateAddress
            {
                IsValid = false
            };
            ResponseData<ValidateAddress> result = new ResponseData<Common.ValidateAddress>
            {
                Result = validateAddress
            };
            if (string.IsNullOrEmpty(address))
            {
                validateAddress.IsValid = false;
                return result;
            }
            if (address.Length != 42)
            {
                validateAddress.IsValid = false;
                return result;
            }
            if (!address.StartsWith("0x"))
            {
                validateAddress.IsValid = false;
                return result;
            }
            var regex = new Regex("^0x[A-Za-z0-9]{34,64}$");
            validateAddress.IsValid = regex.IsMatch(address);
            return result;
        }

        /// <summary>
        /// 解锁账号
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">账号密码</param>
        /// <param name="validTime">有效时间，默认300秒，单位秒</param>
        /// <returns></returns>
        protected virtual ResponseData<bool> UnlockAccount(string account, string password, int validTime = 300)
        {
            return this.InvokeMethod<bool>(ETHRPCMethod.UnlockAccount, DefaultApiName, account, password, validTime);
        }

        /// <summary>
        /// 锁定账号
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns></returns>
        protected virtual ResponseData<bool> LockAccount(string account)
        {
            return this.InvokeMethod<bool>(ETHRPCMethod.LockAccount, DefaultApiName, account);
        }

        protected virtual decimal EstimateGas(EstimateMinerfeeData data)
        {
            Estimate estimate = new Estimate
            {
                From = data.From,
                To = data.To,
                AsPrice = this._configuration.GasPrice.LongDECToHEX()
            };
            if (data.Amount > 0)
                estimate.Value = data.Amount.BigToUnitHex(WeiPrecision);
            else
                estimate.Value = "";
            decimal gasAmount = 300000;
            ResponseData<string> gasResult = null;
            try
            {
                if (string.IsNullOrEmpty(data.MultiSigAddress))
                    gasResult = this.InvokeMethod<string>(ETHRPCMethod.EstimateGas, DefaultApiName, estimate);
                else
                    gasResult = this.InvokeMethod<string>(ETHRPCMethod.EstimateGas, DefaultApiName, new { to = data.MultiSigAddress, data = GetSendData(new SendCoinData() { ETHMultiSigResult = data.ETHMultiSigResult, ToCoinAddress = data.To, Amount = data.Amount }) });
                gasAmount = gasResult.Result.LongHEXToDEC() * GasMultiple;
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(data.MultiSigAddress))
                    Console.WriteLine($"EstimateGas:{JsonConvert.SerializeObject(ex)}, Estimate:{JsonConvert.SerializeObject(estimate)}");
                else
                    Console.WriteLine($"EstimateGas:{JsonConvert.SerializeObject(ex)}, Estimate:{JsonConvert.SerializeObject(new { to = data.MultiSigAddress, data = GetSendData(new SendCoinData() { ETHMultiSigResult = data.ETHMultiSigResult, ToCoinAddress = data.To, Amount = data.Amount }) })}, ETHMultiSigResult:{JsonConvert.SerializeObject(data.ETHMultiSigResult)}, ToCoinAddress:{data.To}, MultiSigAddress:{data.MultiSigAddress}, Amount:{data.Amount}");
                WriteLog?.Invoke($"EstimateGas:{JsonConvert.SerializeObject(ex)}, Estimate:{JsonConvert.SerializeObject(estimate)}");
            }
            return gasAmount;
        }

        /// <summary>
        /// 评估需要消耗的数量
        /// </summary>
        /// <param name="from">转出地址</param>
        /// <param name="to">转到地址</param>
        /// <param name="amount">转出数量</param>
        /// <param name="price">转出单价</param>
        /// <returns></returns>
        public virtual ResponseData<decimal> EstimateMinerfee(EstimateMinerfeeData data)
        { 
            var totalGas = EstimateGas(data) * this._configuration.GasPrice;
            totalGas = totalGas.UnitToBig(WeiPrecision);
            var result = new ResponseData<decimal>
            {
                Result = totalGas
            };
            return result;
        }

        /// <summary>
        /// 获取转出单价信息
        /// </summary>
        /// <returns></returns>
        protected virtual ResponseData<decimal> GetGasPrice()
        {
            var callResult = this.InvokeMethod<string>(ETHRPCMethod.GasPrice, DefaultApiName);
            var gasPrice = callResult.Result.LongHEXToDEC();
            var min = (decimal)Math.Pow(10, 9);
            if (gasPrice < min)
            {
                gasPrice = min;
            }
            ResponseData<decimal> result = new ResponseData<decimal>
            {
                Error = callResult.Error,
                Result = gasPrice
            };
            return result;
        }

        /// <summary>
        /// 获取合约交易信息
        /// </summary>
        /// <param name="txid">交易ID</param>
        /// <returns></returns>
        public ResponseData<CoinTransaction> GetContractTransaction(string txid)
        {
            var precision = this._configuration.Precision ?? WeiPrecision;
            var transactionResult = GetTransaction(txid);
            ResponseData<CoinTransaction> result = new ResponseData<CoinTransaction>
            {
                Error = transactionResult.Error
            };
            var contractTransaction = transactionResult.Result.ContractTransaction;
            CoinTransaction coinTransaction = new CoinTransaction
            {
                Blockhash = contractTransaction.BlockHash,
                Blockindex = contractTransaction.BlockHeight,
                FromAddress = contractTransaction.FromAddress,
                TXId = contractTransaction.TXID,
                Fee = transactionResult.Result.Fee,
                Generated = transactionResult.Result.Generated,
                BlockTime = transactionResult.Result.BlockTime,
                Category = transactionResult.Result.Category,
                Confirmations = transactionResult.Result.Confirmations,
                Time = transactionResult.Result.Time,
                TimeReceived = transactionResult.Result.TimeReceived,
                WalletconFlicts = transactionResult.Result.WalletconFlicts
            };
            var inputData = contractTransaction.Input;
            string toAddress = string.Empty;
            string amountData = string.Empty;
            if (inputData.StartsWith("0x0ae4a584"))
            {
                toAddress = inputData.Substring(ContractMethodLength + (64 * 4), ToAddressLength);
                amountData = inputData.Substring(ContractMethodLength + (64 * 5), AmountLength);
            }
            else
            {
                toAddress = inputData.Substring(ContractMethodLength, ToAddressLength);
                amountData = inputData.Substring(ContractMethodLength + ToAddressLength, AmountLength);
            }
            toAddress = toAddress.TrimStart('0').PadLeft(40, '0');
            coinTransaction.Address = "0x" + toAddress;
            coinTransaction.Account = coinTransaction.Address;
            coinTransaction.Amount = ("0x" + amountData.TrimStart('0')).UnitToBig(precision);
            result.Result = coinTransaction;
            return result;
        }
        /// <summary>
        /// 获取交易凭证信息
        /// </summary>
        /// <param name="txID">交易ID</param>
        /// <returns></returns>
        public ResponseData<TransactionReceipt> GetTransactionReceipt(string txID)
        {
            ResponseData<TransactionReceipt> result = new ResponseData<TransactionReceipt>();
            var invokeResult = this.InvokeMethod<ETHTransactionReceipt>(ETHRPCMethod.GetTransactionReceipt, DefaultApiName, txID);
            result.Error = invokeResult.Error;
            if (invokeResult.Result != null)
            {
                var ethTransactionReceipt = invokeResult.Result;
                TransactionReceipt transactionReceipt = new TransactionReceipt
                {
                    BlockHash = ethTransactionReceipt.BlockHash,
                    BlockHeight = ethTransactionReceipt.BlockNumber.GetInt64FromHex(),
                    ContractAddress = ethTransactionReceipt.ContractAddress,
                    CumulativeGasUsed = ethTransactionReceipt.CumulativeGasUsed.LongHEXToDEC(),
                    From = ethTransactionReceipt.From,
                    GasUsed = ethTransactionReceipt.GasUsed.LongHEXToDEC(),
                    To = ethTransactionReceipt.To,
                    TransactionIndex = ethTransactionReceipt.TransactionIndex.GetInt64FromHex(),
                    TXID = ethTransactionReceipt.TransactionHash,
                    IsSuccess = IsSuccess(ethTransactionReceipt)
                };
                result.Result = transactionReceipt;
            }
            return result;
        }

        /// <summary>
        /// 获取交易手续费
        /// </summary>
        /// <param name="txID">交易ID</param>
        /// <returns></returns>
        public ResponseData<decimal> GetTransactionFee(string txID, int minConfirms)
        {
            var lastBlockNumber = GetBlockCount().Result;
            var precision = _configuration.Precision ?? WeiPrecision;
            var transactionReceipt = GetTransactionReceipt(txID);
            decimal gas = 0;
            decimal gasPrice = 0;
            if (!string.IsNullOrEmpty(transactionReceipt.Error) || transactionReceipt.Result == null || !transactionReceipt.Result.IsSuccess || (lastBlockNumber - transactionReceipt.Result.BlockHeight) < minConfirms)
            {
                ResponseData<decimal> responseData = new ResponseData<decimal>
                {
                    Error = "未获取到凭证信息",
                    Result = -1
                };
                return responseData;

            }
            gas = transactionReceipt.Result.GasUsed;
            var transaction = GetTransaction(txID);
            gasPrice = transaction.Result.GasPrice;
            decimal fee = (gas * gasPrice).UnitToBig(precision);
            ResponseData<decimal> result = new ResponseData<decimal>
            {
                Result = Math.Abs(fee)
            };
            return result;

        }

        /// <summary>
        /// 交易是否成功
        /// </summary>
        /// <param name="receipt"></param>
        /// <returns></returns>
        protected virtual bool IsSuccess(ETHTransactionReceipt receipt)
        {
            return receipt.Status.GetIntFromHex() == 1;
        }

        /// <summary>
        /// ETH交易记录信息转换为通用交易记录
        /// </summary>
        /// <param name="transactionInfo">ETH交易记录信息</param>
        /// <returns></returns>
        private CoinTransaction ConvertToCoinTransaction(TransactionInfo transactionInfo)
        {
            if (transactionInfo == null)
            {
                return null;
            }
            var precision = _configuration.Precision ?? WeiPrecision;
            CoinTransaction coinTransaction = new CoinTransaction
            {
                Blockhash = transactionInfo.BlockHash,
                Blockindex = transactionInfo.BlockNumber.GetIntFromHex(),
                Account = transactionInfo.To,
                Address = transactionInfo.To,
                FromAddress = transactionInfo.From,
                Amount = transactionInfo.Value.UnitToBig(precision),
                TXId = transactionInfo.Hash,
                Category = "Receive",
                Gas = transactionInfo.Gas.BigIntHEXToDEC(),
                GasPrice = transactionInfo.GasPrice.BigIntHEXToDEC(),
                Nonce = transactionInfo.Nonce.BigIntHEXToDEC()
            };
            coinTransaction.Fee = transactionInfo.Gas.BigIntHEXToDEC() * transactionInfo.GasPrice.UnitToBig(precision);
            coinTransaction.ContractTransaction = GetContractTransaction(transactionInfo);
            return coinTransaction;
        }

        /// <summary>
        /// 转换为锲约交易信息
        /// </summary>
        /// <param name="transactionInfo"></param>
        /// <returns></returns>
        private ContractTransaction GetContractTransaction(TransactionInfo transactionInfo)
        {
            //有输入并且不等于0x
            if (!string.IsNullOrEmpty(transactionInfo.Input) && transactionInfo.Input.ToUpper() != "0X")
            {
                if (transactionInfo.Input.Length >= (ContractMethodLength + ToAddressLength + AmountLength))
                {
                    ContractTransaction contractTransaction = new ContractTransaction
                    {
                        BlockHeight = Convert.ToInt32(transactionInfo.BlockNumber.GetInt64FromHex()),
                        FromAddress = transactionInfo.From,
                        TXID = transactionInfo.Hash,
                        BlockHash = transactionInfo.BlockHash,
                        Input = transactionInfo.Input,
                        ContractAddress = transactionInfo.To
                    };
                    if (transactionInfo.Input.Length >= ContractMethodLength)
                    {
                        contractTransaction.ContractMethod = transactionInfo.Input.Substring(0, ContractMethodLength);
                    }
                    var inputData = contractTransaction.Input;
                    string toAddress = string.Empty;
                    if (inputData.StartsWith("0x0ae4a584"))
                        toAddress = inputData.Substring(ContractMethodLength + (64 * 4), ToAddressLength);
                    else
                        toAddress = inputData.Substring(ContractMethodLength, ToAddressLength);
                    toAddress = toAddress.TrimStart('0').PadLeft(40, '0');
                    contractTransaction.ToAddress = "0x" + toAddress;
                    return contractTransaction;
                }
            }
            return null;
        }
          
        /// <summary>
        /// 创建多签交易
        /// </summary>
        /// <param name="sendData"></param>
        /// <returns></returns>
        public virtual ResponseData<string> CreateMultiSigTransaction(SendCoinData sendData)
        {
            try
            {
                int precision = _configuration.Precision ?? WeiPrecision;
                var multiSigAddress = base._configuration.MultiSigAddress?.Item1;
                if (string.IsNullOrEmpty(multiSigAddress))
                    return new ResponseData<string>() { Error = "未初始化ETH多签地址." };
                string fromAddress, toAddress;
                if (multiSigAddress.IndexOf("0x") == 0)
                    fromAddress = multiSigAddress.Substring(2);
                else
                    fromAddress = multiSigAddress;
                if (sendData.ToCoinAddress.IndexOf("0x") == 0)
                    toAddress = sendData.ToCoinAddress.Substring(2);
                else
                    toAddress = sendData.ToCoinAddress;
                string amount = sendData.Amount.BigToUnitHex(precision).Substring(2).PadLeft(64, '0');
                string nonce = GetNonce(multiSigAddress).ConvertToHex().Substring(2).PadLeft(64, '0');

                var paramValue = $"0x1900{fromAddress}{toAddress}{amount}{nonce}";
                var result = this.InvokeMethod<string>(ETHRPCMethod.WEB3SHA3, DefaultApiName, paramValue);
                return result;
            }
            catch (Exception e)
            {
                return new ResponseData<string>() { Error = $"CreateMultiSigTransaction Error. Details:{e.Message}" };
            }
        }

        protected long GetNonce(string address)
        {
            var methodID = "0xaffed0e0";
            var data = new TokenBalance
            {
                To = address,
                Data = methodID
            };
            var result = this.InvokeMethod<string>(ETHRPCMethod.EthCall, DefaultApiName, data, "latest");
            if (string.IsNullOrEmpty(result.Error) && result.Result == "0x")
                throw new Exception("未能获取Nonce值.");
            if (string.IsNullOrEmpty(result.Error))
                return result.Result.GetInt64FromHex();
            throw new Exception(result.Error);
        }

        /// <summary>
        /// 转币
        /// </summary>
        /// <param name="fromAccountTag">自己的钱包标签</param>
        /// <param name="toCoinAddress">收币方标签</param>
        /// <param name="amount">数量，单位为 ETH</param>
        /// <returns>转账成功返回交易ID</returns>
        public ResponseData<string> SentMultiSigTransaction(SendCoinData data)
        {
            var multiSigAddress = base._configuration.MultiSigAddress?.Item1;
            int precision = _configuration.Precision ?? WeiPrecision;
            //代币不需要转ETH相关的币
            var sendTransaction = new SendTransaction
            {
                From = data.FromAccount,
                To = multiSigAddress,
                Value = ""
            };
            if (this._configuration.GasPrice > 0)
            {
                sendTransaction.GasPrice = this._configuration.GasPrice.LongDECToHEX();
            }
            var gas = this.EstimateGas(new EstimateMinerfeeData() { Amount = data.Amount, From = data.FromAccount, To = data.ToCoinAddress });
            sendTransaction.Gas = gas.LongDECToHEX();
            sendTransaction.Data = GetSendData(data);
            Console.WriteLine($"SentMultiSigTransaction {JsonConvert.SerializeObject(sendTransaction)}");
            return this.InvokeMethod<string>(ETHRPCMethod.SendFrom, DefaultApiName, sendTransaction, data.FromAccountPassword);
        }

        protected virtual string GetSendData(SendCoinData data)
        {
            var signResults = data.ETHMultiSigResult.OrderBy(e => e.Item1).ToList();
            int precision = _configuration.Precision ?? WeiPrecision;
            var methodID = "0x0ae4a584";
            var sigVOffset = "0e0".PadLeft(64, '0');
            var sigROffset = "160".PadLeft(64, '0');
            var sigSOffset = "1e0".PadLeft(64, '0');
            var bGethOffset = "260".PadLeft(64, '0');
            var toAddress = data.ToCoinAddress.Substring(2).PadLeft(64, '0');
            var sendAmount = data.Amount.BigToUnitHex(precision).Substring(2).PadLeft(64, '0');
            var dataOffset = "2e0".PadLeft(64, '0');
            var sigVSize = "003".PadLeft(64, '0');
            var sigV0 = signResults[0].Item2.Substring(signResults[0].Item2.Length - 2, 2).PadLeft(64, '0');
            var sigV1 = signResults[1].Item2.Substring(signResults[1].Item2.Length - 2, 2).PadLeft(64, '0');
            var sigV2 = signResults[2].Item2.Substring(signResults[2].Item2.Length - 2, 2).PadLeft(64, '0');
            var sigRSize = "003".PadLeft(64, '0');
            var sigR0 = signResults[0].Item2.Substring(2, 64);
            var sigR1 = signResults[1].Item2.Substring(2, 64);
            var sigR2 = signResults[2].Item2.Substring(2, 64);
            var sigSSize = "003".PadLeft(64, '0');
            var sigS0 = signResults[0].Item2.Substring(66, 64);
            var sigS1 = signResults[1].Item2.Substring(66, 64);
            var sigS2 = signResults[2].Item2.Substring(66, 64);
            var bGethSize = "003".PadLeft(64, '0');
            var bGeth = "0".PadLeft(64 * 3, '0');
            var sendType = "0".PadLeft(64, '0');
            var content = $"{methodID}{sigVOffset}{sigROffset}{sigSOffset}{bGethOffset}{toAddress}{sendAmount}{dataOffset}{sigVSize}{sigV0}{sigV1}{sigV2}{sigRSize}{sigR0}{sigR1}{sigR2}{sigSSize}{sigS0}{sigS1}{sigS2}{bGethSize}{bGeth}{sendType}";
            return content;
        }
        public bool IsWalletInsideAddress(AddressInfo addressInfo)
        {
            try
            {
                var result = UnlockAccount(addressInfo.Address, addressInfo.Password);
                if (result.Result)
                {
                    LockAccount(addressInfo.Address);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}