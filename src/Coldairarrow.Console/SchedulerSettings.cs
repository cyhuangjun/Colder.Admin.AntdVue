using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Scheduler
{
    public class SchedulerSettings
    {
        public string DepositSynchronizationJobCronExpression { set; get; }
        public string ConfirmTransactionSynchronizationCronExpression { set; get; }
        public string ContractTransactionExpression { get; set; }
        public string WithdrawalSynchronizationCronExpression { get; set; }
        public string MoveToSysWalletJobCronExpression { get; set; }
        public string DepositAccountingJobCronExpression { set; get; } 
        public string CallbackJobCronExpression { get; set; } 
        public string TransfersSyncJobCronExpression { set; get; }
    }
}
