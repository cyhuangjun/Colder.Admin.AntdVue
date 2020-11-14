using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common
{
    /// <summary>
    /// 消耗Gas信息
    /// </summary>
    public class EstimateGasInfo
    {
        /// <summary>
        /// Gas价格
        /// </summary>
        public decimal GasPrice { get; set; }
        /// <summary>
        /// 需要消耗的数量
        /// </summary>
        public decimal NeedAmount { get; set; }
        public decimal EstimateGas { get; set; }
        public decimal OrgEstimateGas { get; set; }
        /// <summary>
        /// 评估数据信息
        /// </summary>
        public object EstimateData { get; set; }
    }
}
