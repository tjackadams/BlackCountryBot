using System;
using System.IO;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using BlackCountryBot.Web;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;

namespace BlackCountryBot.IntegrationTests
{
    public class SliceFixture
    {
        private static readonly Checkpoint _checkpoint;
        private static readonly IConfigurationRoot _configuration;
        private static readonly IServiceScopeFactory _scopeFactory;
        static SliceFixture()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();
            _configuration = builder.Build();

            var startup = new Startup(_configuration);
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            ServiceProvider provider = services.BuildServiceProvider();
            _scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            _checkpoint = new Checkpoint();

            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                BlackCountryContext dbContext = scope.ServiceProvider.GetService<BlackCountryContext>();
                dbContext.Database.Migrate();
            }
        }

        public static Task ResetCheckpoint()
        {
            return _checkpoint.Reset(_configuration["ConnectionString"]);
        }

        public static Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return ExecuteScopeAsync(sp =>
            {
                IMediator mediator = sp.GetRequiredService<IMediator>();

                return mediator.Send(request);
            });
        }
        public static async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                BlackCountryContext dbContext = scope.ServiceProvider.GetService<BlackCountryContext>();

                T result = await action(scope.ServiceProvider).ConfigureAwait(false);

                return result;
            }
        }

        public static Task<T> FindAsync<T>(int id)
            where T : class, IEntity
        {
            return ExecuteDbContextAsync(db => db.Set<T>().FindAsync(id));
        }

        public static Task<T> ExecuteDbContextAsync<T>(Func<BlackCountryContext, Task<T>> action)
        {
            return ExecuteScopeAsync(sp => action(sp.GetService<BlackCountryContext>()));
        }
    }
}
