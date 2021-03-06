﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 评估消耗Gas参数
    /// </summary>
    public class EstimateMinerfeeData
    {
        /// <summary>
        /// 发送地址
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 接收地址
        /// </summary>
        public string To { get; set; }
        /// <summary>
        /// 发送数量
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 代币合约地址
        /// </summary>
        public string TokenAddress { get; set; }


        public IList<Tuple<string, string>> ETHMultiSigResult { set; get; }
        public string MultiSigAddress { set; get; }
    }
}
