using CCPP.Cryptocurrency.Common.ETH;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCPP.Cryptocurrency.Common.Provider
{
    public class ETHTokenClientServiceProvider : ETHClientServiceProvider
    {
        /// <summary>
        /// 转币地址契约地址
        /// </summary>
        private const string TransferMethodID = "0xa9059cbb";
        /// <summary>
        /// 获取代币余额地址
        /// </summary>
        private const string BalanceMethodID = "0x70a08231";

        public ETHTokenClientServiceProvider(Configuration configuration) : base(configuration)
        {

        } 

        /// <summary>
        /// 转币
        /// </summary>
        /// <param name="fromAccountTag">自己的钱包标签</param>
        /// <param name="toCoinAddress">收币方标签</param>
        /// <param name="amount">数量，单位为 ETH</param>
        /// <returns>转账成功返回交易ID</returns>
        public override ResponseData<string> SendTransaction(SendCoinData data)
        {
            int precision = this._configuration.Precision ?? WeiPrecision;
            //代币不需要转ETH相关的币
            var sendTransaction = new SendTransaction
            {
                From = data.FromAccount,
                To = data.TokenCoinAddress,
                Value = ""
            };
            if (data.GasPrice > 0)
            {
                sendTransaction.GasPrice = data.GasPrice.LongDECToHEX();
            }
            if (data.EstimateGas > 0)
            {
                sendTransaction.Gas = data.EstimateGas.LongDECToHEX();
            }
            if (data.Nonce.HasValue)
            {
                sendTransaction.Nonce = data.Nonce.Value.LongDECToHEX();
            }
            sendTransaction.Data = GetSendData(data.ToCoinAddress, data.Amount);
            return this.InvokeMethod<string>(ETHRPCMethod.SendFrom, DefaultApiName, sendTransaction, data.FromAccountPassword);
        }


        /// <summary>
        /// 获取代币余额
        /// </summary>
        /// <param name="accountTag">账号信息</param>
        /// <param name="tokenContractAddress">代币合约地址</param>
        /// <returns></returns>
        public override ResponseData<decimal> GetBalance(BalanceData balanceData)
        {
            //  0x70a08231（MethodID） +指定的以太坊账号地址（共64个字符，前面用0补足）
            var address = balanceData.Address;
            if (address.IndexOf("0x") == 0)
            {
                address = address.Substring(2);
            }
            address = address.PadLeft(64, '0');
            address = $"{BalanceMethodID}{address}";
            var data = new TokenBalance
            {
                To = balanceData.TokenContractAddress,
                Data = address
            };
            int precision = _configuration.Precision ?? WeiPrecision;
            var callResult = this.InvokeMethod<string>(ETHRPCMethod.EthCall, DefaultApiName, data, "latest");
            ResponseData<decimal> result = new ResponseData<decimal>
            {
                Error = callResult.Error,
                Result = callResult.Result.UnitToBig(precision)
            };
            return result;
        }

        /// <summary>
        /// 评估需要消耗的数量
        /// </summary>
        /// <param name="from">转出地址</param>
        /// <param name="to">转到地址</param>
        /// <param name="amount">转出数量</param>
        /// <param name="price">转出单价</param>
        /// <returns></returns>
        public override ResponseData<EstimateGasInfo> EstimateGas(string from, string to, decimal amount, decimal price, string tokenAddress = "")
        {
            if (price <= 0)
            {
                price = GetGasPrice().Result;
            }
            var data = GetSendData(to, amount);
            var estimateData = new
            {
                from = from,
                to = tokenAddress,
                asPrice = price.LongDECToHEX(),
                value = "",
                data = data
            };
            ResponseData<string> gasResult = this.InvokeMethod<string>(ETHRPCMethod.EstimateGas, DefaultApiName, estimateData);
            var gasAmount = gasResult.Result.LongHEXToDEC() * GasMultiple;
            decimal totalGas = (gasAmount * price) * GasMultiple * 1.2M;
            totalGas = totalGas.UnitToBig(WeiPrecision);
            EstimateGasInfo estimateGasInfo = new EstimateGasInfo
            {
                GasPrice = price,
                NeedAmount = totalGas,
                EstimateGas = gasAmount,
                OrgEstimateGas = gasResult.Result.LongHEXToDEC(),
                EstimateData = estimateData
            };
            ResponseData<EstimateGasInfo> result = new ResponseData<EstimateGasInfo>
            {
                Result = estimateGasInfo
            };
            return result;
        }

        /// <summary>
        /// 评估需要消耗的数量
        /// </summary>
        /// <param name="from">转出地址</param>
        /// <param name="to">转到地址</param>
        /// <param name="amount">转出数量</param>
        /// <param name="price">转出单价</param>
        /// <returns></returns>
        public override ResponseData<EstimateGasInfo> EstimateGas(EstimateGasData estimateGasData)
        {
            if (estimateGasData.Price <= 0)
            {
                estimateGasData.Price = GetGasPrice().Result;
            }
            var sendData = GetSendData(estimateGasData.To, estimateGasData.Amount);
            var estimateData = new
            {
                from = estimateGasData.From,
                to = estimateGasData.TokenAddress,
                asPrice = estimateGasData.Price.LongDECToHEX(),
                value = "0x0",
                data = sendData
            };
            ResponseData<string> gasResult = null;
            decimal gasAmount = 300000;
            decimal totalGas = 0;
            try
            {
                gasResult = this.InvokeMethod<string>(ETHRPCMethod.EstimateGas, DefaultApiName, estimateData);
                gasAmount = gasResult.Result.LongHEXToDEC() * GasMultiple;
                totalGas = (300000m * estimateGasData.Price);
            }
            catch (Exception ex)
            {
                totalGas = 300000m * estimateGasData.Price;
            }
            totalGas = totalGas.UnitToBig(WeiPrecision);
            EstimateGasInfo estimateGasInfo = new EstimateGasInfo
            {
                GasPrice = estimateGasData.Price,
                NeedAmount = totalGas,
                EstimateGas = gasAmount,
                OrgEstimateGas = string.IsNullOrEmpty(gasResult?.Result) ? 0 : gasResult.Result.LongHEXToDEC(),
                EstimateData = estimateData
            };
            ResponseData<EstimateGasInfo> result = new ResponseData<EstimateGasInfo>
            {
                Result = estimateGasInfo
            };
            return result;
        }

        protected override bool IsSuccess(ETHTransactionReceipt receipt)
        {
            return receipt.Status.GetIntFromHex() == 1 && receipt.Logs != null && receipt.Logs.Any();
        }

        /// <summary>
        /// 获取发送数据信息
        /// </summary>
        /// <param name="toAddress">需要转到的地址信息</param>
        /// <param name="amount">金额数量</param>
        /// <returns></returns>
        protected virtual string GetSendData(string toAddress, decimal amount)
        {
            toAddress = toAddress.RemoveHex();
            toAddress = toAddress.PadLeft(64, '0');
            var precision = _configuration.Precision ?? WeiPrecision;
            amount = amount.BigToUnit(precision);
            var amountHex = amount.LongDECToHEX();
            amountHex = amountHex.RemoveHex();
            amountHex = amountHex.PadLeft(64, '0');
            return $"{TransferMethodID}{toAddress}{amountHex}";
        }

        /// <summary>
        /// 是否是代币
        /// </summary>
        public override bool IsToken => true;

        protected override string GetSendData(SendCoinData data)
        {
            var signResults = data.ETHMultiSigResult.OrderBy(e => e.Item1).ToList();
            int precision = _configuration.Precision ?? WeiPrecision;
            var methodID = "0x0ae4a584";
            var sigVOffset = "0e0".PadLeft(64, '0');
            var sigROffset = "160".PadLeft(64, '0');
            var sigSOffset = "1e0".PadLeft(64, '0');
            var bGethOffset = "260".PadLeft(64, '0');
            var tokenCoinAddress = data.TokenCoinAddress.Substring(2).PadLeft(64, '0');
            var sendETHAmount = "0".PadLeft(64, '0');
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
            var size = "44".PadLeft(64, '0');
            var sendType = "a9059cbb";
            var toAddress = data.ToCoinAddress.Substring(2).PadLeft(64, '0');
            var sendAmount = data.Amount.BigToUnitHex(precision).Substring(2).PadLeft(64, '0');
            var content = $"{methodID}{sigVOffset}{sigROffset}{sigSOffset}{bGethOffset}{tokenCoinAddress}{sendETHAmount}{dataOffset}{sigVSize}{sigV0}{sigV1}{sigV2}{sigRSize}{sigR0}{sigR1}{sigR2}{sigSSize}{sigS0}{sigS1}{sigS2}{bGethSize}{bGeth}{size}{sendType}{toAddress}{sendAmount}";
            return content;
        }

        public override ResponseData<string> CreateMultiSigTransaction(SendCoinData sendData)
        {
            try
            {
                int precision = _configuration.Precision ?? WeiPrecision;
                var multiSigAddress = base._configuration.MultiSigAddress?.Item1;
                if (string.IsNullOrEmpty(multiSigAddress))
                    return new ResponseData<string>() { Error = "未初始化ETH多签地址." };
                string fromAddress, tokenAddress;
                if (multiSigAddress.IndexOf("0x") == 0)
                    fromAddress = multiSigAddress.Substring(2);
                else
                    fromAddress = multiSigAddress;
                if (sendData.TokenCoinAddress.IndexOf("0x") == 0)
                    tokenAddress = sendData.TokenCoinAddress.Substring(2);
                else
                    tokenAddress = sendData.TokenCoinAddress;
                string amount = "0".PadLeft(64, '0');
                string nonce = GetNonce(multiSigAddress).ConvertToHex().Substring(2).PadLeft(64, '0');
                var sendType = "a9059cbb";
                var address = sendData.ToCoinAddress.Substring(2).PadLeft(64, '0');
                var sendAmount = sendData.Amount.BigToUnitHex(precision).Substring(2).PadLeft(64, '0');
                var data = $"{sendType}{address}{sendAmount}";
                var paramValue = $"0x1900{fromAddress}{tokenAddress}{amount}{data}{nonce}";
                Console.WriteLine($"paramValue:{paramValue}");
                var result = this.InvokeMethod<string>(ETHRPCMethod.WEB3SHA3, DefaultApiName, paramValue);
                return result;
            }
            catch (Exception e)
            {
                return new ResponseData<string>() { Error = $"CreateMultiSigTransaction Error. Details:{e.Message}" };
            }
        }
    }
}
