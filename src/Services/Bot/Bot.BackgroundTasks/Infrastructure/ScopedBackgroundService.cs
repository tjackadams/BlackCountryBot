using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Bot.BackgroundTasks.Infrastructure
{
    public abstract class ScopedBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ScopedBackgroundService(IServiceScopeFactory serviceScopeFactory) : base()
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task Process(CancellationToken cancellationToken)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                await ProcessInScope(scope.ServiceProvider, cancellationToken);
            }
        }

        public abstract Task ProcessInScope(IServiceProvider serviceProvider, CancellationToken cancellationToken);
    }
}
