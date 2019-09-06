using System;
using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using BlackCountryBot.Web.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlackCountryBot.Web.HostedServices
{
    public class StartupHostedService<TDbContext> : IHostedService, IDisposable
        where TDbContext : DbContext
    {
        private readonly ILogger<StartupHostedService<TDbContext>> _logger;
        private readonly IServiceProvider _sp;
        private readonly StartupHostedServiceHealthCheck _healthCheck;
        public StartupHostedService(ILogger<StartupHostedService<TDbContext>> logger, IServiceProvider sp, StartupHostedServiceHealthCheck healthCheck)
        {
            _logger = logger;
            _sp = sp;
            _healthCheck = healthCheck;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Startup Background Service is starting.");

            using (IServiceScope scope = _sp.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;

                ILogger<TDbContext> dbLogger = serviceProvider.GetRequiredService<ILogger<TDbContext>>();

                TDbContext context = serviceProvider.GetRequiredService<TDbContext>();

                IDbContextSeed<TDbContext> seeder = serviceProvider.GetRequiredService<IDbContextSeed<TDbContext>>();

                try
                {
                    dbLogger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TDbContext).Name);

                    await InvokeSeeder(seeder, context, dbLogger);

                    await Task.Delay(30000);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TDbContext).Name);
                }

                _healthCheck.StartupTaskCompleted = true;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Startup Background Service is stopping.");

            return Task.CompletedTask;
        }

        private static async Task InvokeSeeder<TContext>(IDbContextSeed<TContext> seeder, TContext context, ILogger<TContext> logger)
            where TContext : DbContext
        {
            context.Database.Migrate();
            await seeder.SeedAsync(context, logger);
        }

        public void Dispose()
        {

        }
    }
}
