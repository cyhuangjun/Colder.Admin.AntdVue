using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Scheduler
{
    public interface IScheduler
    {
        Task Start();
    }
}
