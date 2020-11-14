using System.Collections.Generic;

namespace CCPP.Cryptocurrency.Common.BTC
{
    /// <summary>
    /// 携带费用的交易返回类
    /// </summary>
    public class TransactionSetFeeInfo
    {
        /// <summary>
        /// 返回状态码
        /// </summary>
        public ETransactionFeeStatusCode StatusCode { get;set;}


        /// <summary>
        /// 交易记录
        /// </summary>
        public TransactionFeeInfo TransactionInfo { get;set;}
         
    }

    /// <summary>
    /// 实际带费用需要返回的记录
    /// </summary>
    public class TransactionFeeInfo
    {
        /// <summary>
        /// 交易Id
        /// </summary>
        public string TxId { get; set; }

        /// <summary>
        /// 发送者的地址集合
        /// </summary>
        public List<string> FromAddress { get;set;}

        /// <summary>
        /// 接受者的地址集合
        /// </summary>
        public List<string> ToAddress { get;set;}

        /// <summary>
        /// 发送的总金额（只包括发给别人的）
        /// </summary>
        public decimal Amount { get;set;}

        /// <summary>
        /// 发送的费用
        /// </summary>
        public decimal Fee { get;set;}
         
        /// <summary>
        /// 最小确认数
        /// </summary>
        public int Minconf { get;set;}
         
    }

    /// <summary>
    /// 带费用的交易状态返回枚举
    /// </summary>
    public enum ETransactionFeeStatusCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 费用不足
        /// </summary>
        FeeLessThan = 1,

        /// <summary>
        /// 费用过高
        /// </summary>
        FeeMoreThan = 2,

        /// <summary>
        /// 余额不足
        /// </summary>
        AmountLessThan = 3,

        /// <summary>
        /// 输出金额不等于输入金额
        /// </summary>
        InputNotEqualOutput = 4,
        
        /// <summary>
        /// 未花费的可用金额不足
        /// </summary>
        UnspentAmountLessThan = 5,

        /// <summary>
        /// 该地址不正确
        /// </summary>
        TheAddressIsIncorrect = 6,

        /// <summary>
        /// 发送交易账户绝对不能为空账户，因为空账户是钱包默认账户
        /// </summary>
        FromAccountIsEmpty = 7,

        /// <summary>
        /// 金额小于0
        /// </summary>
        AmountLtZero = 8,

        /// <summary>
        /// 最小发送金额
        /// </summary>
        AmountLtMinAmount = 9

    }

}
