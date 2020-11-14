using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common.BTC
{
    /// <summary>
    /// The list transactions info.
    /// TODO: Add field corresponding to the list unspent rpc method
    /// </summary>
    public class TransactionUnspentInfo
    {
        #region Public Properties

        /// <summary>
        /// 交易id
        /// </summary>
        [JsonProperty("txid")]
        public string TxId { get; set; }

        /// <summary>
        /// vout
        /// </summary>
        [JsonProperty("vout")]
        public int Vout { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        [JsonProperty("account")]
        public string Account { get; set; }

        /// <summary>
        /// script pubkey
        /// </summary>
        [JsonProperty("scriptPubKey")]
        public string ScriptPubKey { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 确认数
        /// </summary>
        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        /// <summary>
        /// spendable
        /// </summary>
        [JsonProperty("spendable")]
        public bool Spendable { get; set; }

        /// <summary>
        /// solvable
        /// </summary>
        [JsonProperty("solvable")]
        public bool Solvable { get; set; }

        /// <summary>
        /// safe
        /// </summary>
        [JsonProperty("safe")]
        public bool Safe { get; set; }

        [JsonProperty("redeemScript")]
        public string RedeemScript { set; get; }

        #endregion
    }
}