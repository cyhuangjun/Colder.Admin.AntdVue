using Coldairarrow.Scheduler.Job;
using Coldairarrow.Util;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Scheduler
{
    public class DefaultScheduler : IScheduler
    {
        private static Task<Quartz.IScheduler> scheduler;
        public static Task<Quartz.IScheduler> Current
        {
            get
            {
                if (scheduler == null)
                    scheduler = StdSchedulerFactory.GetDefaultScheduler(); 
                return scheduler;
            }
        }
        public Task Start()
        {
            StartTask();
            return Current.Result.Start();
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        public void StartTask()
        {
            var settings = ConfigHelper.Configuration.GetSection("Scheduler").Get<SchedulerSettings>();
            CreateJob<DepositSynchronizationJob>("DepositSynchronizationJob", settings.DepositSynchronizationJobCronExpression);
            CreateJob<ConfirmTransactionSynchronizationJob>("ConfirmTransactionSynchronizationJob", settings.ConfirmTransactionSynchronizationCronExpression);
            CreateJob<ContractTransactionJob>("ContractTransactionJob", settings.ContractTransactionExpression);
            CreateJob<WithdrawalSynchronizationJob>("WithdrawalSynchronizationJob", settings.WithdrawalSynchronizationCronExpression);
            CreateJob<MoveToSysWalletJob>("_CoinTransactionMoveToSysJob", settings.MoveToSysWalletJobCronExpression);


            //CreateJob<VGPayJob>("_VGPayJob_", settings.VGPayJobExpression);
            //CreateJob<CoinAmountCheckJob>("_CoinAmountCheckJob", settings.CoinAmountCheckJobCronExpression); 
            //CreateJob<CoinTransactionFeeSyncJob>("_CoinTransactionFeeSyncJob", settings.CoinTransactionFeeExpression);
            //CreateJob<SynchronousExchangeRateJob>("_SynchronousExchangeRateJob", settings.SynchronousExchangeRateJobCronExpression);  
            //CreateJob<MoveTokenJob>("_MoveTokenJob", settings.MoveTokenCronExpression);
            //CreateJob<CoinOutAmountCheckJob>("_CoinOutAmountCheckJob", settings.CoinOutAmountCheckJobCronExpression);
            //CreateJob<CoinTransactionFreezeJob>("_CoinTransactionFreezeJob_", settings.CoinTransactionFreezeExpression);
            //CreateJob<CoinBalanceAlarmJob>("_CoinBalanceAlarmJob_", settings.CoinBalanceAlarmExpression);
            //CreateJob<MoveCoinToSecurityWalletJob>("_MoveCoinToSecurityWalletJob_", settings.MoveCoinToSecurityWalletExpression);
            //CreateJob<MoveCoinToSecurityWalletConfirmJob>("_MoveCoinToSecurityWalletConfirmJob_", settings.MoveCoinToSecurityWalletConfirmExpression);
            //CreateJob<ExchangeIndexJob>("_c2c_exchange_index_job_", settings.C2CExchangeIndexSetting.C2CExchangeIndexJobCronExpression);
            //CreateJob<SiteCoinAmountCheckJob>("_SiteCoinAmountCheckJob", settings.CoinAmountCheckJobCronExpression);
            //CreateJob<SiteCoinConfirmTransactionJob>("_SiteCoinConfirmTransactionJob", settings.CoinConfirmTransactionSynchronizeCronExpression);
            //CreateJob<SiteCoinContractTransactionJob>("_SiteCoinContractTransactionJob", settings.CoinContractTransactionExpression);
            //CreateJob<SiteCoinTransactionFeeJob>("_SiteCoinTransactionFeeJob", settings.CoinTransactionFeeExpression);
            //CreateJob<SiteMoveTokenRecordJob>("_SiteMoveTokenRecordJob", settings.MoveTokenCronExpression);
            //CreateJob<SiteMoveCoinJob>("_SiteMoveCoinJob", settings.CoinTransactionMoveToSysJobCronExpression);
            //CreateJob<CustomerBalanceJob>("_CustomerBalanceJob", settings.CustomerBalanceSetting.CustomerBalanceJobCronExpression);
            //CreateJob<AutoCoinTransactionOutJob>("_AutoCoinTransactionOutJob_", settings.AutoCoinTransactionOutExpression);
        }

        private void CreateJob<T>(string uid, string cronExpression) where T : IJob
        {
            var job = JobBuilder.Create<T>()
                .WithIdentity("job" + uid, "group" + uid)
                .Build();
            var cronTrigger = (ICronTrigger)TriggerBuilder.Create()
                                                .WithIdentity("trigger" + uid, "group" + uid)
                                                .StartNow()
                                                .WithCronSchedule(cronExpression)
                                                .Build();
            var ft = Current.Result.ScheduleJob(job, cronTrigger);
        }
    }
}
