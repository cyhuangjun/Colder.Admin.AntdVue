using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity
{
    public class ErrorCodeDefine
    {
        /// <summary>
        /// 法币不存在
        /// </summary>
        public const int CurrencyNoExist = 40000;
        /// <summary>
        /// 虚拟币不存在
        /// </summary>
        public const int CoinNotExist = 40001;
        /// <summary>
        /// 参数无效
        /// </summary>
        public const int ParameterInvalid = 40002;
        /// <summary>
        /// 法币不能为空
        /// </summary>
        public const int CurrencyRequired = 40003;
        /// <summary>
        /// 虚拟币不能为空
        /// </summary>
        public const int CoinNotRequired = 40004;
        /// <summary>
        /// 钱包维护中
        /// </summary>
        public const int WalletMaintenancing = 40005;
        /// <summary>
        /// 非法操作
        /// </summary>
        public const int IllegalOperation = 40006;
        /// <summary>
        /// 加密货币资产不足
        /// </summary>
        public const int CryptocurrencyAssetsNotEnought = 40007;
        /// <summary>
        /// SecretKey不存在
        /// </summary>
        public const int SecretKeyNotFound = 40008;
        /// <summary>
        /// HTTP Head Api_key不存在
        /// </summary>
        public const int ApiKeyNotFound = 40009;
        /// <summary>
        /// 签名不匹配
        /// </summary>
        public const int SignaturesNotMatch = 40010;
        /// <summary>
        /// 账号被冻结
        /// </summary>
        public const int UserIsFrozen = 40011;
        /// <summary>
        /// HTTP Head TimeStamp 不存在
        /// </summary>
        public const int TimeStampNotFound = 40012;
        /// <summary>
        /// HTTP Head signature不存在
        /// </summary>
        public const int SignatureNotFound = 40013;
        /// <summary>
        /// HTTP Head TimeStamp 错误
        /// </summary>
        public const int TimeStampError = 40014;
        /// <summary>
        /// 资产ID必填
        /// </summary>
        public const int AssetsIDRequired = 40015;
        /// <summary>
        /// 用户ID必填
        /// </summary>
        public const int UserIDRequired = 40016;
        /// <summary>
        /// 关联ID必填
        /// </summary>
        public const int RelateIDRequired = 40017;
        /// <summary>
        /// 资产变动数为0
        /// </summary>
        public const int AssetsChangeAmountZero = 40018;
        /// <summary>
        /// 操作超时
        /// </summary>
        public const int OperationTimeout = 40019;
        /// <summary>
        /// 资产不足
        /// </summary>
        public const int AssetsNotEnought = 40020;
        /// <summary>
        /// 用户ID不存在
        /// </summary>
        public const int UserIDNotExist = 40021;
        /// <summary>
        /// 币种未配置
        /// </summary>
        public const int CoinConfigNotFound = 40022;
        /// <summary>
        /// 小于最小充币量
        /// </summary>
        public const int PaymentLessThanMinQty = 40023;
        /// <summary>
        /// 小于最小提币量
        /// </summary>
        public const int TransferLessThanMinQty = 40024;
        /// <summary>
        /// 交易对评估价未发现
        /// </summary>
        public const int EstimatePriceNotFound = 40025;
        /// <summary>
        /// UID不能为空
        /// </summary>
        public const int UIDNotRequired = 40026;
        /// <summary>
        /// 未知错误
        /// </summary>
        public const int UnknownError = 40027;
        /// <summary>
        /// 钱包余额不足
        /// </summary>
        public const int WalletBalanceNotEnought = 40028;
        /// <summary>
        /// 钱包服务错误
        /// </summary>
        public const int WalletServceError = 40028;

        #region 基础
        /// <summary>
        /// 处理成功
        /// </summary>
        public const int Success = -1;
        /// <summary>
        /// 操作失败
        /// </summary>
        public const int Fail = 202;
        /// <summary>
        /// 未授权
        /// </summary>
        public const int Unauthorized = 403;
        /// <summary>
        /// 服务器出错
        /// </summary>
        public const int ServerError = 500;
        #endregion
    }
}
