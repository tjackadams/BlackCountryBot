using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.DependencyInjection;

namespace Bot.BackgroundTasks.Infrastructure
{
    public abstract class ScheduledBackgroundService : ScopedBackgroundService
    {
        private DateTime LastRunTime { get; set; }
        private DateTime NextRunTime { get; set; }
        protected abstract string Schedule { get; }
        private CronExpression CronSchedule { get; set; }
        public ScheduledBackgroundService(IServiceScopeFactory serviceScopeFactory)
            : base(serviceScopeFactory)
        {
            CronSchedule = CronExpression.Parse(Schedule);
            NextRunTime = CronSchedule.GetNextOccurrence(DateTime.UtcNow).Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                DateTime now = DateTime.UtcNow;
                if (ShouldRun(now))
                {
                    await Process(stoppingToken);
                    Increment();
                }

                await Task.Delay(15000, stoppingToken); // 15 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private void Increment()
        {
            LastRunTime = NextRunTime;
            NextRunTime = CronSchedule.GetNextOccurrence(NextRunTime).Value;
        }

        private bool ShouldRun(DateTime timeReference)
        {
            return NextRunTime < timeReference && LastRunTime != NextRunTime;
        }
    }
}
