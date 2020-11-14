using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Coldairarrow.Scheduler.Core
{
    public class CustomHostService : IHostedService
    {
        private ILogger<CustomHostService> _logger;
        private Task _executingTask;
        private readonly IScheduler _scheduler;

        public CustomHostService(ILogger<CustomHostService> logger,
                                 IScheduler scheduler)
        {
            this._logger = logger;
            this._scheduler = scheduler;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {  
            _executingTask = ExecuteAsync();
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        public Task ExecuteAsync()
        {
            return this._scheduler.Start();
        }
    }
}
