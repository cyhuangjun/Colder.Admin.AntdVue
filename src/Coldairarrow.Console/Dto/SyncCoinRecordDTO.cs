using Coldairarrow.Entity.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Scheduler.Dto
{
    public class SyncCoinRecordDTO
    {
        /// <summary>
        /// 虚拟币ID
        /// </summary>
        public string CoinID { get; set; }
        /// <summary>
        /// 是否正在运行
        /// </summary>
        public bool IsRunning { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 通知异常时间
        /// </summary>
        public DateTime NotificateErrorTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }
        public Coin Coin { get; set; }
        /// <summary>
        /// 同步记录信息
        /// </summary>
        public CoinSynchronizationBlock Record { get; set; }
        /// <summary>
        /// 原始高度
        /// </summary>
        public long OrgLastHeight { get; set; }
    }
}
