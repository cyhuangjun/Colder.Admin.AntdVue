using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity.Enum
{
    public enum ProviderType
    {
        /// <summary>
        /// 默认Bitcoin驱动接口
        /// </summary>
        BTC = 0,
        /// <summary>
        /// 以太坊 驱动接口
        /// </summary>
        ETH = 1, 
        /// <summary>
        /// ETH系列代币驱动接口
        /// </summary>
        ETHTokenCoin = 2,
    }
}
