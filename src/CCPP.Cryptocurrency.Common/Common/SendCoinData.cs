using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 发送虚拟币参数接口
    /// </summary>
    public class SendCoinData
    {
        /// <summary>
        /// 地址或标签
        /// </summary>
        public string FromAccount { get; set; }
        /// <summary>
        /// 目标地址
        /// </summary>
        public string ToCoinAddress { get; set; }
        /// <summary>
        /// 目标地址标签
        /// </summary>
        public string ToCoinAddressTag { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 发送账号密码
        /// </summary>
        public string FromAccountPassword { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal GasPrice { get; set; }
        public decimal EstimateGas { get; set; }
        /// <summary>
        /// 代币地址
        /// </summary>
        public string TokenCoinAddress { get; set; }
        public decimal? Nonce { get; set; }
        /// <summary>
        /// 私钥
        /// </summary>
        public string PrivateKey { get; set; }
        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey { get; set; }
        /// <summary>
        /// 代币名称标识
        /// </summary>
        public string TokenCoinKey { get; set; }
        /// <summary>
        /// 代币和父币兑换比例
        /// </summary>
        public decimal? TokenCoinExchange { get; set; }
        /// <summary>
        /// 钱包发行总量
        /// </summary>
        public decimal? WalletSupplyAmount { get; set; }
        /// <summary>
        /// 矿工费比例
        /// </summary>
        public decimal MinerFeeRate { get; set; }
        /// <summary>
        /// 最小确认数
        /// </summary>
        public int Minconf { get; set; }
        /// <summary>
        /// 找零地址
        /// </summary>
        public string ChargeAddress { get; set; }
        /// <summary>
        /// 账号种子
        /// </summary>
        public string Seed { get; set; }
        /// <summary>
        /// 原始交易的 16 进制字符串(BTC)
        /// </summary>
        public string HexString { get; set; }

        public IList<Tuple<string, string>> ETHMultiSigResult { set; get; }
    }
}
