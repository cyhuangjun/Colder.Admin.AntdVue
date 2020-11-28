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
            CreateJob<MoveToSysWalletJob>("CoinTransactionMoveToSysJob", settings.MoveToSysWalletJobCronExpression);
            CreateJob<DepositAccountingJob>("DepositAccountingJob", settings.DepositAccountingJobCronExpression);
            CreateJob<CallbackJob>("CallbackJob", settings.CallbackJobCronExpression);
            CreateJob<TransfersSyncJob>("TransfersSyncJob", settings.TransfersSyncJobCronExpression);
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
