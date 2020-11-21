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


        public string CoinAmountCheckJobCronExpression { set; get; } 
        public string SynchronousExchangeRateJobCronExpression { get; set; } 
        public string OrderTimedOutJobCronExpression { get; set; } 
        public string CoinTransactionFeeExpression { get; set; }  
        public string MoveTokenCronExpression { get; set; }
        public string CoinOutAmountCheckJobCronExpression { get; set; }
        public string VGPayJobExpression { get; set; }
        public string AutoCoinTransactionOutExpression { set; get; }
    }
}
