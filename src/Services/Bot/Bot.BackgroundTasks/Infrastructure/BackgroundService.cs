using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Bot.BackgroundTasks.Infrastructure
{
    public abstract class BackgroundService : IHostedService
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(cancellationToken);

            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                await Process(stoppingToken);

                await Task.Delay(15000, stoppingToken); // 15 second delay
            } while (!stoppingToken.IsCancellationRequested);
        }

        protected abstract Task Process(CancellationToken cancellationToken);
    }
}
