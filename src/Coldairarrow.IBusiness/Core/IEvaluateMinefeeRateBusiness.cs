using Coldairarrow.Entity.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.IBusiness.Core
{
    public interface IEvaluateMinefeeRateBusiness
    {
        /// <summary>
        /// 获取ETH Gas Price
        /// </summary>
        /// <returns></returns>
        GasPriceInfo GetETHGasPrice();
        /// <summary>
        /// 获取BTC推荐的矿工费费率
        /// </summary>
        /// <returns></returns>
        decimal GetBTCFeeRate();
    }
}
