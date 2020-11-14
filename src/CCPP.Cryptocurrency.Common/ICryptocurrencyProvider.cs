using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    public interface ICryptocurrencyProvider
    {
        bool IsNeedSecurityKey { get; }
        /// <summary>
        /// 是否通过区块获取交易记录
        /// </summary>
        bool IsUseBlockGetTransaction { get; }
        /// <summary>
        /// 提币是否需要注册账号地址
        /// </summary>
        bool IsNeedRegisterAccount { get; }
        /// <summary>
        /// 是否需要系统地址信息
        /// </summary>
        bool IsNeedSysAddress { get; }
        /// <summary>
        /// 是否需要使用用户系统标志
        /// </summary>
        bool IsUseAddressIdentity { get; } 
        /// <summary>
        /// 精度
        /// </summary>
        int Digits { get; }
        /// <summary>
        /// 是否有获取凭证信息
        /// </summary>
        bool HasTransactionReceipt { get; }

        /// <summary>
        /// 是否是代币
        /// </summary>
        bool IsToken { get; }

        /// <summary>
        /// 是否没有区块高度
        /// </summary>
        bool HasNotBlockCount { get; }

        bool IsWalletInsideAddress(AddressInfo addressInfo);
        ResponseData<GenerateAddressResult> CreateWalletAddress(CreateWalletAddressInput createAddressInfo);
        /// <summary>
        /// 获取区块数量
        /// </summary>
        /// <returns></returns>
        ResponseData<int> GetBlockCount();
        /// <summary>
        /// 获取对映区块的Hash值
        /// </summary>
        /// <param name="height">区块高度</param>
        /// <returns></returns>
        ResponseData<string> GetBlockHash(long height);
        /// <summary>
        /// 获取对映区块信息
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        ResponseData<CoinBlock> GetBlockInfo(long height);
        /// <summary>
        /// 获取交易信息
        /// </summary>
        /// <param name="accountTag">账号标签</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        ResponseData<IList<CoinTransaction>> GetTransaction(string accountTag, int pageSize, int pageIndexOrSkipCount); 
        /// <summary>
        /// 转币
        /// </summary>
        /// <returns>转账成功返回交易ID</returns>
        ResponseData<string> SendTransaction(SendCoinData data);
        /// <summary>
        /// 获取账号余额
        /// </summary> 
        /// <returns></returns>
        ResponseData<decimal> GetBalance(BalanceData balanceData);
        /// <summary>
        /// 获取钱包所有可用余额
        /// </summary>
        /// <param name="confirm">最小确认数</param>
        /// <returns></returns>
        ResponseData<decimal> GetAllBalance(int confirm);

        /// <summary>
        /// 获取原始交易数据信息
        /// </summary>
        /// <param name="txid">交易编号</param>
        /// <returns></returns>
        ResponseData<RawTransaction> GetRawTransaction(string txid);
        /// <summary>
        /// 获取未花费的交易
        /// </summary>
        /// <returns></returns>
        ResponseData<IList<UnspentTransaction>> GetUnspentTransaction(params string[] addressList);
        /// <summary>
        /// 校验钱包地址
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <returns></returns>
        ResponseData<ValidateAddress> ValidateAddress(string address);
        /// <summary>
        /// 获取单笔交易记录信息
        /// </summary>
        /// <param name="txid">交易ID</param>
        /// <returns></returns>
        ResponseData<CoinTransaction> GetTransaction(string txid);

        /// <summary>
        /// 获取合约交易信息
        /// </summary>
        /// <param name="txid">交易ID</param>
        /// <returns></returns>
        ResponseData<CoinTransaction> GetContractTransaction(string txid);                
        /// <summary>
        /// 评估需要消耗的数量
        /// </summary>
        /// <param name="data">评估参数</param>
        /// <returns></returns>
        ResponseData<EstimateGasInfo> EstimateGas(EstimateGasData data);       
        /// <summary>
        /// 获取交易凭证信息
        /// </summary>
        /// <param name="txID">交易ID</param>
        /// <returns></returns>
        ResponseData<TransactionReceipt> GetTransactionReceipt(string txID);
        /// <summary>
        /// 获取交易手续费
        /// </summary>
        /// <param name="txID">交易ID</param>
        /// <param name="minConfirms">最小确认数</param>
        /// <returns></returns>
        ResponseData<decimal> GetTransactionFee(string txID, int minConfirms);

        /// <summary>
        /// 创建多签交易
        /// </summary>
        /// <param name="sendData"></param>
        /// <returns></returns>
        ResponseData<string> CreateMultiSigTransaction(SendCoinData sendData);
        /// <summary>
        /// 发送原始交易
        /// </summary>
        /// <param name="hexString">原始交易的 16 进制字符串</param>
        /// <returns></returns>
        ResponseData<string> SentMultiSigTransaction(SendCoinData data); 

        ResponseData<T> InvokeMethod<T>(string a_sMethod, string apiName, params object[] a_params);
    }
}
