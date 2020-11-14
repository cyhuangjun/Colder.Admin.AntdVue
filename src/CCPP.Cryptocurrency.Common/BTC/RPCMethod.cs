using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common.BTC
{
    public class RPCMethod
    {
        public static string GetBlockCount = "getblockcount"; //获取区块数量
        public static string GetBlockHash = "getblockhash";  //获取对映区块的Hash值
        public static string GetDifficulty = "getdifficulty";  //获取当前挖矿难度
        public static string GetBlock = "getblock";  //获取对映区块信息
        public static string GetBalance = "getbalance";  //获取账号余额
        public static string AddWalletAddress = "getnewaddress";  //向钱包新增一个地址
        public static string GetTransaction = "gettransaction";  //获取单个交易信息
        public static string Getmininginfo = "getmininginfo";  //获取虚拟币基本信息
        public static string GetRawTransaction = "getrawtransaction";   //获取原始交易数据信息
        public static string SendFrom = "sendfrom";  //转币到对方标签
        public static string SendToAddress = "sendtoaddress";  //转币到对方地址
        public static string GetTransactionList = "listtransactions"; //获取交易信息列表
        public static string Move = "move";  //转币
        public static string GetUnspent = "listunspent";  //获取未花费的交易
        public static string ValidateAddress = "validateaddress";  //校验钱包地址
        public static string DecodeRawTransaction = "decoderawtransaction";
        /// <summary>
        /// 锁定钱包
        /// </summary>
        public static string LockWallet = "walletlock";
        /// <summary>
        /// 解锁钱包
        /// </summary>
        public static string UnlockWallet = "walletpassphrase";
        public static string GetPubKey = "validateaddress"; //获得公钥 
        /// <summary>
        /// 创建原始交易
        /// </summary>
        public static string CreateMultiSigTransaction = "createrawtransaction";
        public static string EstimatesMartFee = "estimatesmartfee";
    }
}
