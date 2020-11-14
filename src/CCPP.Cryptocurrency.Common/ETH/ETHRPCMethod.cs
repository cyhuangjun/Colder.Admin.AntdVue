using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common.ETH
{
    /// <summary>
    /// ETH RPC调用方法
    /// </summary>
    public class ETHRPCMethod
    {
        /// <summary>
        /// 向钱包新增一个地址
        /// </summary>
        public const string AddWalletAddress = "personal_newAccount";
        /// <summary>
        /// 锁定账号
        /// </summary>
        public const string UnlockAccount = "personal_unlockAccount";

        /// <summary>
        /// 锁定账号
        /// </summary>
        public const string LockAccount = "personal_lockAccount";

        /// <summary>
        /// 转币到对方地址
        /// </summary>
        public static string SendFrom = "personal_sendTransaction";

        /// <summary>
        /// ETC转币到对方地址
        /// </summary>
        public static string ETCSendFrom = "personal_signAndSendTransaction";
        /// <summary>
        /// 获取账号余额
        /// </summary>
        public static string GetBalance = "eth_getBalance";
        /// <summary>
        /// 获取区块数量
        /// </summary>
        public static string GetBlockCount = "eth_blockNumber";
        /// <summary>
        /// 根据区块高度获取区块信息
        /// </summary>

        public static string GetBlockByHeight = "eth_getBlockByNumber";

        /// <summary>
        /// 获取单个交易信息
        /// </summary>
        public static string GetTransaction = "eth_getTransactionByHash";
        /// <summary>
        /// 评估消耗的Gas数量信息
        /// </summary>
        public static string EstimateGas = "eth_estimateGas";
        /// <summary>
        /// 获取gas价格
        /// </summary>
        public static string GasPrice = "eth_gasPrice";
        /// <summary>
        /// 获取交易凭证信息
        /// </summary>
        public static string GetTransactionReceipt = "eth_getTransactionReceipt";

        /// <summary>
        /// 调用合约方法
        /// </summary>
        public static string EthCall = "eth_call";

        public static string GetBlockHash = "getblockhash";  //获取对映区块的Hash值
        public static string GetDifficulty = "getdifficulty";  //获取当前挖矿难度




        public static string Getmininginfo = "getmininginfo";  //获取虚拟币基本信息
        public static string GetRawTransaction = "getrawtransaction";   //获取原始交易数据信息

        public static string SendToAddress = "sendtoaddress";  //转币到对方地址
        public static string GetTransactionList = "listtransactions"; //获取交易信息列表
        public static string Move = "move";  //转币
        public static string GetUnspent = "listunspent";  //获取未花费的交易
        public static string ValidateAddress = "validateaddress";  //校验钱包地址
        public static string DecodeRawTransaction = "decoderawtransaction";

        public static string WEB3SHA3 = "web3_sha3";

        public static string PersonalSign = "personal_sign";
    }
}