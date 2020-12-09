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

        #region 基础
        /// <summary>
        /// 处理成功
        /// </summary>
        public const int Success = 200;
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
