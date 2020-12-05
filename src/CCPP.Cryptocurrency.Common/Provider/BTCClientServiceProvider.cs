using CCPP.Cryptocurrency.Common.BTC;
using CCPP.Cryptocurrency.Common.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCPP.Cryptocurrency.Common.Provider
{
    public class BTCClientServiceProvider : BaseClientServiceProvider, ICryptocurrencyProvider
    {
        /// <summary>
        /// 最大矿工费
        /// </summary>
        protected virtual decimal MaxFee => 0.1M;
        /// <summary>
        /// 是否通过区块获取交易记录
        /// </summary>
        public virtual bool IsUseBlockGetTransaction => false;
        /// <summary>
        /// 账号提币是否需要注册
        /// </summary>
        public virtual bool IsNeedRegisterAccount => false;
        /// <summary>
        /// 是否需要进行对钱包地址进行加密
        /// </summary>
        public virtual bool IsNeedSecurityKey => false;

        public virtual int Digits => 8;
        /// <summary>
        /// 是否有获取凭证信息
        /// </summary>
        public virtual bool HasTransactionReceipt => false;
        /// <summary>
        /// 没有区块高度信息
        /// </summary>
        public virtual bool HasNotBlockCount => false;
        /// <summary>
        /// 是否是代币
        /// </summary>
        public virtual bool IsToken => false;
        public virtual bool IsNeedSysAddress => false;
        public virtual bool IsUseAddressIdentity => false; 

        public BTCClientServiceProvider(Configuration configuration) : base(configuration)
        {

        }
        /// <summary>
        /// 获取区块数量
        /// </summary>
        /// <returns></returns>
        public virtual ResponseData<int> GetBlockCount()
        {
            return this.InvokeMethod<int>(RPCMethod.GetBlockCount, DefaultApiName);
        }
     
        /// <summary>
        /// 获取对映区块的Hash值
        /// </summary>
        /// <param name="height">区块高度</param>
        /// <returns></returns>
        public virtual ResponseData<string> GetBlockHash(long height)
        {
            return this.InvokeMethod<string>(RPCMethod.GetBlockHash, DefaultApiName, height);
        }
        /// <summary>
        /// 向钱包新增一个地址
        /// </summary> 
        /// <returns></returns>
        public ResponseData<GenerateAddressResult> CreateWalletAddress(CreateWalletAddressInput createAddressInfo)
        {
            var data = this.InvokeMethod<string>(RPCMethod.AddWalletAddress, DefaultApiName);
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
        /// 发送一笔虚拟币
        /// </summary>
        /// <param name="fromAccount">账号</param>
        /// <param name="toAddress">接受虚拟币地址</param>
        /// <param name="amount">金额</param>
        /// <param name="fee">费用</param>
        /// <param name="minconf">最新确认数</param> 
        /// <param name="unlimited">无限制的费用，默认是有限制的(false)</param>
        /// <param name="currency">默认"" 如果传就单独处理</param>
        /// <returns></returns>
        public virtual ResponseData<string> SendTransaction(SendCoinData sendData)
        {
            sendData.Amount = sendData.Amount.DataTruncated(Digits);
            int precision = this._configuration.Precision ?? Digits;
            sendData.Amount = sendData.Amount.DataTruncated(precision);
            ResponseData<string> result = new ResponseData<string>();
            TransactionFeeInfo data = new TransactionFeeInfo();
            var recommandPara = new RecommendedParam
            {
                Amount = sendData.Amount,
                SendToAddress = sendData.ToCoinAddress
            };
            var minconf = 2;
            var isUnlockSuccess = UnLockWallet();
            if (!isUnlockSuccess)
            {
                result.Error = "解锁钱包失败";
                return result;
            }
            var address = base._configuration.MultiSigAddress.Item1;
            var feeRateResult = SetRecommendedFee(recommandPara, this._configuration.MinerFeeRate, minconf, null, new List<string>() { address });
            if (!string.IsNullOrEmpty(feeRateResult.Error))
            {
                result.Error = feeRateResult.Error;
                return result;
            }
            var fee = feeRateResult.Result;
            decimal minOutputAmount = 0.00006m; //默认最小交易输出金额限制

            //step 1:获取交易记录，构建金额来源
            //金额的账户


            var unspentResult = ListUnspent(minconf, 99999999, null, new List<string>() { address });
            var unspents = unspentResult.Result;
            if (unspents == null || !unspents.Any())
            {
                result.Error = "钱包余额不足";
                return result;
            }
            //step 3: 构建 CreateRawTransactionInput , 将钱发送到对方的地址和自己的地址

            List<CreateRawTransactionInput> inputs = new List<CreateRawTransactionInput>();
            Dictionary<string, decimal> outputs = new Dictionary<string, decimal>();
            Dictionary<string, decimal> retOutputs = new Dictionary<string, decimal>();

            List<string> fromAddress = new List<string>();
            decimal sumAmount = 0;

            var sumUnspents = unspents.Where(s => s.Spendable && s.Confirmations >= minconf);
            // 如果可用的金额不够，则 跳出
            if (sumUnspents == null || sumUnspents.Sum(s => s.Amount) < sendData.Amount + fee)
            {
                result.Error = "钱包余额不足";
                return result;
            }
            /*
            var needAmount = amount + fee;
            //优先找到只需要一笔就能满足数额的
            var bestUnSpentSort = unspents.Where(f => f.Amount == needAmount).ToList();

            if (!bestUnSpentSort.Any())
            {
                bestUnSpentSort = unspents.OrderByDescending(o => o.Confirmations).ToList();
            }
            */
            var unspentsSort = unspents.OrderByDescending(o => o.Confirmations);

            foreach (var item in unspentsSort)
            {
                if (!item.Spendable)
                    continue;

                //先判断确认数
                if (item.Confirmations < minconf)
                    continue;

                sumAmount += item.Amount;

                if (sumAmount <= sendData.Amount + fee)
                {
                    if (item.Amount <= 0)
                    {
                        continue;
                    }

                    var input = new CreateRawTransactionInput()
                    {
                        Output = item.Vout,
                        TransactionId = item.TxId
                    };
                    //from
                    inputs.Add(input);
                    fromAddress.Add(item.Address);
                    if (sumAmount == sendData.Amount + fee)
                    {
                        break;
                    }
                }
                else
                {
                    //这里一部分用于发送，一部分转入到自己的另一个地址
                    var input = new CreateRawTransactionInput()
                    {
                        Output = item.Vout,
                        TransactionId = item.TxId
                    };
                    //from
                    inputs.Add(input);
                    fromAddress.Add(item.Address);

                    /*--------转入我的另一个地址--------*/
                    //判断 可发送金额-（发送金额+费用) 是否大于或等于 最小输出金额
                    if ((sumAmount - (fee + sendData.Amount)) >= minOutputAmount)
                    {
                        //step 2： 获取发送人账户的新地址，用来存取剩下的金额 
                        // var myNewAddress = await GetNewAddressAsync(fromAccount);
                        //这个地址的余额
                        decimal addressBalance = sumAmount - (sendData.Amount + fee);

                        if (addressBalance < minOutputAmount)
                            continue;
                        var address2 = CreateWalletAddress(null).Result;
                        outputs.Add(address2.Address, addressBalance);
                    }
                    break;
                }
            }

            //发送到指定的地址的金额
            outputs.Add(sendData.ToCoinAddress, sendData.Amount);
            retOutputs.Add(sendData.ToCoinAddress, sendData.Amount);
            /*
             * input :from
             * if(Inputs.amounts == Outputs.amounts + fee)
             */
            decimal outputsAmount = outputs.Sum(s => s.Value);

            //必须等于（再确认）
            if (sumAmount != (outputsAmount + fee))
            {
                if ((sumAmount - (outputsAmount + fee)) < 0 || (sumAmount - (outputsAmount + fee)) > minOutputAmount)
                {
                    result.Error = "钱包余额不足";
                    return result;
                }
            }

            var rawTransactionHex = CreateRawTransaction(new CreateRawTransaction()
            {
                Inputs = inputs,
                Outputs = outputs
            });
            var signRawTransaction = SignRawTransaction(new SignRawTransaction(rawTransactionHex.Result));
            var rawTransactionResult = this.InvokeMethod<string>("sendrawtransaction", DefaultApiName, signRawTransaction.Result.Hex);
            LockWallet();
            return rawTransactionResult;
        } 
              
        /// <summary>
        /// 获取对映区块信息
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public virtual ResponseData<CoinBlock> GetBlockInfo(long height)
        {
            var blockHashInfo = this.GetBlockHash(height);
            if (!string.IsNullOrEmpty(blockHashInfo.Error))
            {
                return null;
            }
            return this.InvokeMethod<CoinBlock>(RPCMethod.GetBlock, DefaultApiName, blockHashInfo.Result);
        }

        /// <summary>
        /// 获取交易信息
        /// </summary>
        /// <param name="accountTag">账号标签</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public virtual ResponseData<IList<CoinTransaction>> GetTransaction(string accountTag, int pageSize, int pageIndexOrSkipCount)
        {
            int skipCount = pageSize * pageIndexOrSkipCount;
            return this.InvokeMethod<IList<CoinTransaction>>(RPCMethod.GetTransactionList, DefaultApiName, accountTag, pageSize, skipCount);
        } 
        /// <summary>
        /// 获取账号余额
        /// </summary>
        /// <param name="accountTag">标签</param>
        /// <returns></returns>
        public virtual ResponseData<decimal> GetBalance(BalanceData data)
        {
            if (!data.IsMultiSigAddress && data.Address == "*")
                return this.InvokeMethod<decimal>(RPCMethod.GetBalance, DefaultApiName, data.Address);
            else
            {
                var transactions = ListUnspent(1, 9999999, new List<string>() { data.Address });
                var amount = transactions.Result.Sum(e => e.Amount);
                return new ResponseData<decimal>() { Result = amount };
            }
        }
        /// <summary>
        /// 获取钱包所有可用余额
        /// </summary>
        /// <param name="confirm">最小确认数</param>
        /// <returns></returns>
        public virtual ResponseData<decimal> GetAllBalance(int confirm)
        {
            return this.InvokeMethod<decimal>(RPCMethod.GetBalance, DefaultApiName, "*", confirm);
        }
        /// <summary>
        /// 获取单笔交易记录信息
        /// </summary>
        /// <param name="txid">交易ID</param>
        /// <returns></returns>
        public virtual ResponseData<CoinTransaction> GetTransaction(string txid)
        {
            return this.InvokeMethod<CoinTransaction>(RPCMethod.GetTransaction, DefaultApiName, txid);
        }
        /// <summary>
        /// 获取原始交易数据信息
        /// </summary>
        /// <param name="txid">交易编号</param>
        /// <returns></returns>
        public virtual ResponseData<RawTransaction> GetRawTransaction(string txid)
        {
            return this.InvokeMethod<RawTransaction>(RPCMethod.GetRawTransaction, DefaultApiName, txid, 1);
        }
        /// <summary>
        /// 获取未花费的交易
        /// </summary>
        /// <returns></returns>
        public virtual ResponseData<IList<UnspentTransaction>> GetUnspentTransaction(params string[] addressList)
        {
            return this.InvokeMethod<IList<UnspentTransaction>>(RPCMethod.GetUnspent, DefaultApiName);
        }
        /// <summary>
        /// 校验钱包地址
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <returns></returns>
        public virtual ResponseData<ValidateAddress> ValidateAddress(string address)
        {
            return this.InvokeMethod<ValidateAddress>(RPCMethod.ValidateAddress, DefaultApiName, address);
        }

        /// <summary>
        /// 获取指定地址集合的Unspent
        /// </summary>
        /// <param name="minconf"></param>
        /// <param name="maxconf"></param>
        /// <param name="addressesList"></param>
        /// <returns></returns>
        public virtual ResponseData<List<TransactionUnspentInfo>> ListUnspent(int minconf = 1, int maxconf = 9999999, IEnumerable<string> address = null, IEnumerable<string> excludeAddress = null)
        {
            ResponseData<List<TransactionUnspentInfo>> result = null;
            if (address != null)
            {
                //string query = "[";
                //foreach (string address in addressArray)
                //    query += $"\"{address}\",";
                //query = query.Remove(query.Length - 1);
                //query += "]";
                result = this.InvokeMethod<List<TransactionUnspentInfo>>("listunspent", DefaultApiName, minconf, maxconf, address.ToList());
            }
            else
                result = this.InvokeMethod<List<TransactionUnspentInfo>>("listunspent", DefaultApiName, minconf, maxconf);
            if (excludeAddress != null)
                for (int i = result.Result.Count; i > 0; i--)
                {
                    if (excludeAddress?.Contains(result.Result[i - 1].Address) ?? false)
                        result.Result.RemoveAt(i - 1);
                }
            return result;
        }

        public virtual ResponseData<SignedRawTransaction> SignRawTransaction(SignRawTransaction rawTransaction)
        {
            var hex = rawTransaction.RawTransactionHex;
            var inputs = rawTransaction.Inputs.Any() ? rawTransaction.Inputs : null;
            var privateKeys = rawTransaction.PrivateKeys.Any() ? rawTransaction.PrivateKeys : null;
            var lockResult = UnLockWallet();
            if (!lockResult)
            {
                throw new Exception("解锁钱包失败");
            }
            var res = this.InvokeMethod<SignedRawTransaction>("signrawtransaction", DefaultApiName, hex, inputs, privateKeys);
            return res;
        }
        /// <inheritdoc />
        public ResponseData<string> SentMultiSigTransaction(SendCoinData data)
        {
            ResponseData<string> result = new ResponseData<string>();
            var isUnlockSuccess = UnLockWallet();
            if (!isUnlockSuccess)
            {
                result.Error = "解锁钱包失败";
                return result;
            }
            result = this.InvokeMethod<string>("sendrawtransaction", DefaultApiName, data.HexString);
            LockWallet();
            return result;
        }
               
        protected virtual void LockWallet()
        {
            if (!string.IsNullOrEmpty(this._configuration.WalletSecurityKey))
            {
                try
                {
                    this.InvokeMethod<object>(RPCMethod.LockWallet, DefaultApiName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    WriteLog?.Invoke($"{ex.Message} {ex.StackTrace}");
                } 
            }
        }

        /// <summary>
        /// 解锁钱包
        /// </summary>
        /// <returns></returns>
        protected virtual bool UnLockWallet()
        {
            if (!string.IsNullOrEmpty(this._configuration.WalletSecurityKey))
            {
                var result = this.InvokeMethod<object>(RPCMethod.UnlockWallet, DefaultApiName, this._configuration.WalletSecurityKey, 300);
                if (string.IsNullOrEmpty(result.Error))
                {
                    return true;
                }
                return false;
            }
            return true;
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
            return new ResponseData<decimal>() { Result = 0m };
        }
       
        /// <summary>
        /// 获取合约交易信息
        /// </summary>
        /// <param name="txid">交易ID</param>
        /// <returns></returns>
        public virtual ResponseData<CoinTransaction> GetContractTransaction(string txid)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 获取交易凭证信息
        /// </summary>
        /// <param name="txID">交易ID</param>
        /// <returns></returns>
        public virtual ResponseData<TransactionReceipt> GetTransactionReceipt(string txID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取交易手续费
        /// </summary>
        /// <param name="txID">交易ID</param>
        /// <param name="minConfirms">最小确认数</param>
        /// <returns></returns>
        public virtual ResponseData<decimal> GetTransactionFee(string txID, int minConfirms)
        {
            var transaction = GetTransaction(txID);
            ResponseData<decimal> result = new ResponseData<decimal>
            {
                Error = transaction.Error,
                Result = Math.Abs(transaction.Result.Fee)
            };
            return result;
        }

        protected ResponseData<string> CreateRawTransaction(CreateRawTransaction rawTransaction)
        {
            return this.InvokeMethod<string>("createrawtransaction", DefaultApiName, rawTransaction.Inputs, rawTransaction.Outputs);
        }

        /// <summary>
        /// 设置推荐费用（类比特币）
        /// </summary>
        /// <param name="account"></param>
        /// <param name="param"></param>
        /// <param name="blockConfirmation"></param>
        /// <param name="btcFeeRate"></param>
        /// <returns></returns>
        public ResponseData<decimal> SetRecommendedFee(RecommendedParam param, decimal feeRate, int minconf = 1, IEnumerable<string> address = null, IEnumerable<string> excludeAddress = null)
        {
            param.Amount = Convert.ToDecimal(param.Amount.ToString("0.00000000"));

            ResponseData<decimal> ret = new ResponseData<decimal>();
            decimal minFee = 0.00005m;
            decimal maxFee = MaxFee;
            try
            {
                List<string> fromAddress = new List<string>();
                var unspentsResult = ListUnspent(minconf, 9999999, address, excludeAddress);
                var unspents = unspentsResult.Result;

                List<CreateRawTransactionInput> inputs = new List<CreateRawTransactionInput>();
                Dictionary<string, decimal> outputs = new Dictionary<string, decimal>();

                decimal sumAmount = 0;

                //int counter = 1;

                if (unspents != null)
                {
                    foreach (var item in unspents)
                    {
                        if (!item.Spendable && address == null)
                            continue;
                        sumAmount += item.Amount;
                        if (sumAmount < param.Amount)
                        {
                            if (item.Amount <= 0)
                            {
                                continue;
                            }
                            var input = new CreateRawTransactionInput()
                            {
                                Output = item.Vout,
                                TransactionId = item.TxId
                            };
                            inputs.Add(input);
                            fromAddress.Add(item.Address);
                        }
                        else
                        {
                            //这里一部分用于发送，一部分转入到自己的另一个地址 
                            var input = new CreateRawTransactionInput()
                            {
                                Output = item.Vout,
                                TransactionId = item.TxId
                            };
                            inputs.Add(input);
                            fromAddress.Add(item.Address);

                            if (sumAmount > param.Amount)
                            {
                                var myNewAddress = CreateWalletAddress(null).Result;
                                decimal addressBalance = sumAmount - param.Amount;
                                outputs.Add(myNewAddress.Address, addressBalance);
                            }
                            break;

                        }
                    }
                }

                //发送到指定的地址的金额
                outputs.Add(param.SendToAddress, param.Amount);

                var rawTransactionHex = CreateRawTransaction(new CreateRawTransaction()
                {
                    Inputs = inputs,
                    Outputs = outputs
                }).Result;

                var signRawTransaction = SignRawTransaction(new SignRawTransaction(rawTransactionHex));


                var hexSize = signRawTransaction.Result.Hex.Length / 2;

                var estimateFee = ((hexSize * feeRate) / (decimal)100000000);
                WriteLog?.Invoke($"BTC转币,金额{param.Amount},字节数{hexSize}, 费率{feeRate}");
                if (estimateFee < minFee)
                {
                    estimateFee = minFee;
                }
                else if (estimateFee > maxFee)
                {
                    estimateFee = maxFee;
                }
                ret.Result = estimateFee;
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMddHHmmssffff")} BTC Fee Evaluate 1, SignHex:{signRawTransaction.Result.Hex}, HexSize:{hexSize}, FeeRate:{feeRate}, Fee:{estimateFee}, Hu:{inputs.Count * 180 + outputs.Count * 34 + 10 + inputs.Count}");
                return ret;
            }
            catch (Exception ex)
            {
                ret.Error = "获取推荐费用异常";
                WriteLog?.Invoke(ex);
                Console.WriteLine($"SetRecommendedFee:{ex}");
                return ret;
            }
        }

        /// <summary>
        /// 创建多签交易
        /// </summary>
        /// <param name="sendData"></param>
        /// <returns></returns>
        public ResponseData<string> CreateMultiSigTransaction(SendCoinData sendData)
        {
            int precision = this._configuration.Precision ?? Digits;
            sendData.Amount = sendData.Amount.DataTruncated(precision);
            ResponseData<string> result = new ResponseData<string>();
            TransactionFeeInfo data = new TransactionFeeInfo();
            var recommandPara = new RecommendedParam
            {
                Amount = sendData.Amount,
                SendToAddress = sendData.ToCoinAddress
            };
            var minconf = 2;
            var isUnlockSuccess = UnLockWallet();
            if (!isUnlockSuccess)
            {
                result.Error = "解锁钱包失败";
                return result;
            }
            var multiSigAddress = base._configuration.MultiSigAddress?.Item1;
            var privatekey = base._configuration.MultiSigAddress?.Item2;

            var feeRateResult = SetRecommendedFee(recommandPara, this._configuration.MinerFeeRate, minconf, new List<string>() { multiSigAddress });
            if (!string.IsNullOrEmpty(feeRateResult.Error))
            {
                result.Error = feeRateResult.Error;
                return result;
            }
            var fee = feeRateResult.Result;
            decimal minOutputAmount = 0.00006m; //默认最小交易输出金额限制

            //step 1:获取交易记录，构建金额来源
            var unspentResult = ListUnspent(minconf, 99999999, new List<string>() { multiSigAddress });
            var unspents = unspentResult.Result;
            if (unspents == null || !unspents.Any())
            {
                result.Error = "钱包余额不足";
                return result;
            }
            //step 3: 构建 CreateRawTransactionInput , 将钱发送到对方的地址和自己的地址
            List<CreateRawTransactionInput> inputs = new List<CreateRawTransactionInput>();
            Dictionary<string, decimal> outputs = new Dictionary<string, decimal>();
            Dictionary<string, decimal> retOutputs = new Dictionary<string, decimal>();

            List<string> fromAddress = new List<string>();
            decimal sumAmount = 0;

            var sumUnspents = unspents.Where(s => s.Confirmations >= minconf);
            // 如果可用的金额不够，则跳出
            if (sumUnspents == null || sumUnspents.Sum(s => s.Amount) < sendData.Amount + fee)
            {
                result.Error = "钱包余额不足";
                return result;
            }

            var signRawTransaction = new SignRawTransaction("");
            var unspentsSort = unspents.OrderByDescending(o => o.Confirmations);
            foreach (var item in unspentsSort)
            {
                if (item.Confirmations < minconf)
                    continue;
                sumAmount += item.Amount;
                if (sumAmount <= sendData.Amount + fee)
                {
                    if (item.Amount <= 0)
                        continue;
                    var input = new CreateRawTransactionInput()
                    {
                        Output = item.Vout,
                        TransactionId = item.TxId
                    };
                    inputs.Add(input);
                    fromAddress.Add(item.Address);
                    signRawTransaction.Inputs.Add(new SignRawTransactionInput() { TransactionId = item.TxId, Output = item.Vout, RedeemScript = item.RedeemScript, ScriptPubKey = item.ScriptPubKey });
                    if (sumAmount == sendData.Amount + fee)
                        break;
                }
                else
                {
                    //这里一部分用于发送，一部分转入到自己的另一个地址
                    var input = new CreateRawTransactionInput()
                    {
                        Output = item.Vout,
                        TransactionId = item.TxId
                    };
                    inputs.Add(input);
                    fromAddress.Add(item.Address);
                    signRawTransaction.Inputs.Add(new SignRawTransactionInput() { TransactionId = item.TxId, Output = item.Vout, RedeemScript = item.RedeemScript, ScriptPubKey = item.ScriptPubKey });
                    /*--------转入我的另一个地址--------*/
                    //判断 可发送金额-（发送金额+费用) 是否大于或等于 最小输出金额
                    if ((sumAmount - (fee + sendData.Amount)) >= minOutputAmount)
                    {
                        decimal addressBalance = sumAmount - (sendData.Amount + fee);
                        outputs.Add(multiSigAddress, addressBalance);
                    }
                    break;
                }
            }
            //发送到指定的地址的金额
            outputs.Add(sendData.ToCoinAddress, sendData.Amount);
            retOutputs.Add(sendData.ToCoinAddress, sendData.Amount);
            decimal outputsAmount = outputs.Sum(s => s.Value);
            //必须等于（再确认）
            if (sumAmount != (outputsAmount + fee))
            {
                if ((sumAmount - (outputsAmount + fee)) < 0 || (sumAmount - (outputsAmount + fee)) > minOutputAmount)
                {
                    result.Error = "钱包余额不足";
                    return result;
                }
            }
            var rawTransactionHex = CreateRawTransaction(new CreateRawTransaction()
            {
                Inputs = inputs,
                Outputs = outputs
            });
            signRawTransaction.RawTransactionHex = rawTransactionHex.Result;
            //signRawTransaction.PrivateKeys = new List<string>() { privatekey }; 
            result.Result = JsonConvert.SerializeObject(signRawTransaction);
            LockWallet();
            return result;
        }

        public bool IsWalletInsideAddress(AddressInfo addressInfo)
        {
            var result = this.InvokeMethod<AddressDetail>("getaddressinfo", DefaultApiName, addressInfo.Address);
            return result.Result?.IsMine ?? false;
        }

        public ResponseData<decimal> SetRecommendedFee(List<RecommendedParam> recommendedParams, decimal feeRate, int minconf, string multiSigAddress)
        {
            ResponseData<decimal> ret = new ResponseData<decimal>();
            decimal minFee = 0.00005m;
            decimal maxFee = MaxFee;
            try
            {
                List<string> fromAddress = new List<string>();
                var totalAmount = recommendedParams.Sum(e => e.Amount);
                var unspentsResult = ListUnspent(minconf, 9999999, new List<string>() { multiSigAddress });
                var unspents = unspentsResult.Result;
                var totalUnspent = (unspents?.Sum(e => e.Amount) ?? 0);
                if (totalAmount > totalUnspent)
                {
                    ret.Error = $"多签地址余额不足.";
                    return ret;
                }
                List<CreateRawTransactionInput> inputs = new List<CreateRawTransactionInput>();
                Dictionary<string, decimal> outputs = new Dictionary<string, decimal>();
                decimal sumAmount = 0;
                foreach (var item in unspents)
                {
                    sumAmount += item.Amount;
                    if (sumAmount < totalAmount)
                    {
                        if (item.Amount <= 0)
                        {
                            continue;
                        }
                        var input = new CreateRawTransactionInput()
                        {
                            Output = item.Vout,
                            TransactionId = item.TxId
                        };
                        inputs.Add(input);
                        fromAddress.Add(item.Address);
                    }
                    else
                    {
                        var input = new CreateRawTransactionInput()
                        {
                            Output = item.Vout,
                            TransactionId = item.TxId
                        };
                        inputs.Add(input);
                        fromAddress.Add(item.Address);

                        if (sumAmount > totalAmount)
                        {
                            decimal addressBalance = sumAmount - totalAmount;
                            outputs.Add(multiSigAddress, addressBalance);
                        }
                        break;
                    }
                }
                foreach (var param in recommendedParams)
                {
                    if (outputs.ContainsKey(param.SendToAddress))
                        outputs[param.SendToAddress] = (outputs[param.SendToAddress] + param.Amount);
                    else
                        outputs.Add(param.SendToAddress, param.Amount);
                }
                var rawTransactionHex = CreateRawTransaction(new CreateRawTransaction()
                {
                    Inputs = inputs,
                    Outputs = outputs
                }).Result;
                var signRawTransaction = SignRawTransaction(new SignRawTransaction(rawTransactionHex));
                var hexSize = signRawTransaction.Result.Hex.Length / 2;
                var estimateFee = ((hexSize * feeRate) / (decimal)100000000);
                if (estimateFee < minFee)
                    estimateFee = minFee;
                else if (estimateFee > maxFee)
                    estimateFee = maxFee;
                ret.Result = estimateFee;
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMddHHmmssffff")} BTC Fee Evaluate, SignHex:{signRawTransaction.Result.Hex}, HexSize:{hexSize}, feeRate:{feeRate}, Fee:{estimateFee}, Hu:{436 * inputs.Count + 32 + outputs.Count * 34 + (10 + inputs.Count)}");
                return ret;
            }
            catch (Exception ex)
            {
                ret.Error = "获取推荐费用异常";
                WriteLog?.Invoke(ex);
                Console.WriteLine($"获取推荐费用异常 SetRecommendedFee:{ex.Message + ex.StackTrace}");
                return ret;
            }
        }

        public ResponseData<string> CreateMultiSigTransaction(List<SendCoinData> sendDatas)
        {
            ResponseData<string> result = new ResponseData<string>();
            TransactionFeeInfo data = new TransactionFeeInfo();
            int precision = this._configuration.Precision ?? Digits;
            var totalAmout = sendDatas.Sum(e => e.Amount);
            var minconf = 2;
            if (minconf == 0) minconf = 2;
            var minerFeeRate = this._configuration.MinerFeeRate;
            var recommandParas = new List<RecommendedParam>();
            foreach (var sendData in sendDatas)
            {
                sendData.Amount = sendData.Amount.DataTruncated(precision);
                var recommandPara = new RecommendedParam
                {
                    Amount = sendData.Amount,
                    SendToAddress = sendData.ToCoinAddress
                };
                recommandParas.Add(recommandPara);
            }
            var isUnlockSuccess = UnLockWallet();
            if (!isUnlockSuccess)
            {
                result.Error = "解锁钱包失败";
                return result;
            }
            var multiSigAddress = base._configuration.MultiSigAddress?.Item1;
            var privatekey = base._configuration.MultiSigAddress?.Item2;
            var feeRateResult = SetRecommendedFee(recommandParas, minerFeeRate, minconf, multiSigAddress);
            if (!string.IsNullOrEmpty(feeRateResult.Error))
            {
                result.Error = feeRateResult.Error;
                return result;
            }
            var fee = feeRateResult.Result;
            decimal minOutputAmount = 0.00006m; //默认最小交易输出金额限制

            //step 1:获取交易记录，构建金额来源
            var unspentResult = ListUnspent(minconf, 99999999, new List<string>() { multiSigAddress });
            var unspents = unspentResult.Result;
            if (unspents == null || !unspents.Any())
            {
                result.Error = "多签地址余额不足.";
                return result;
            }
            //step 3: 构建 CreateRawTransactionInput , 将钱发送到对方的地址和自己的地址
            List<CreateRawTransactionInput> inputs = new List<CreateRawTransactionInput>();
            Dictionary<string, decimal> outputs = new Dictionary<string, decimal>();

            List<string> fromAddress = new List<string>();
            decimal sumAmount = 0;

            var sumUnspents = unspents.Where(s => s.Confirmations >= minconf);
            // 如果可用的金额不够，则跳出
            if (sumUnspents == null || sumUnspents.Sum(s => s.Amount) < (totalAmout + fee))
            {
                result.Error = $"多签地址余额不足,Balance:{sumUnspents?.Sum(s => s.Amount)}, Request:{(totalAmout + fee)}.";
                return result;
            }

            var signRawTransaction = new SignRawTransaction("");
            var unspentsSort = unspents.OrderByDescending(o => o.Confirmations);
            foreach (var item in unspentsSort)
            {
                if (item.Confirmations < minconf)
                    continue;
                sumAmount += item.Amount;
                if (sumAmount <= (totalAmout + fee))
                {
                    if (item.Amount <= 0)
                        continue;
                    var input = new CreateRawTransactionInput()
                    {
                        Output = item.Vout,
                        TransactionId = item.TxId
                    };
                    inputs.Add(input);
                    fromAddress.Add(item.Address);
                    signRawTransaction.Inputs.Add(new SignRawTransactionInput() { TransactionId = item.TxId, Output = item.Vout, RedeemScript = item.RedeemScript, ScriptPubKey = item.ScriptPubKey });
                    if (sumAmount == (totalAmout + fee))
                        break;
                }
                else
                {
                    //这里一部分用于发送，一部分转入到自己的另一个地址
                    var input = new CreateRawTransactionInput()
                    {
                        Output = item.Vout,
                        TransactionId = item.TxId
                    };
                    inputs.Add(input);
                    fromAddress.Add(item.Address);
                    signRawTransaction.Inputs.Add(new SignRawTransactionInput() { TransactionId = item.TxId, Output = item.Vout, RedeemScript = item.RedeemScript, ScriptPubKey = item.ScriptPubKey });
                    /*--------转入我的另一个地址--------*/
                    //判断 可发送金额-（发送金额+费用) 是否大于或等于 最小输出金额
                    if ((sumAmount - (fee + totalAmout)) >= minOutputAmount)
                    {
                        decimal addressBalance = sumAmount - (totalAmout + fee);
                        addressBalance = addressBalance.DataTruncated(precision);
                        outputs.Add(multiSigAddress, addressBalance);
                    }
                    break;
                }
            }
            foreach (var sendData in sendDatas)
            {
                if (outputs.ContainsKey(sendData.ToCoinAddress))
                    outputs[sendData.ToCoinAddress] = (outputs[sendData.ToCoinAddress] + sendData.Amount);
                else
                    outputs.Add(sendData.ToCoinAddress, sendData.Amount);
            }
            decimal outputsAmount = outputs.Sum(s => s.Value);
            //必须等于（再确认）
            if (sumAmount != (outputsAmount + fee))
            {
                if ((sumAmount - (outputsAmount + fee)) < 0 || (sumAmount - (outputsAmount + fee)) > minOutputAmount)
                {
                    result.Error = "钱包余额不足";
                    return result;
                }
            }
            var rawTransactionHex = CreateRawTransaction(new CreateRawTransaction()
            {
                Inputs = inputs,
                Outputs = outputs
            });
            signRawTransaction.RawTransactionHex = rawTransactionHex.Result;
            result.Result = JsonConvert.SerializeObject(signRawTransaction);
            LockWallet();
            return result;
        }
    }
}
