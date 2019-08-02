
using System;
using System.IO;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using Bot.API;
using Bot.API.Infrastructure;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot.FunctionalTests
{
    public class SliceFixture
    {
        private static readonly IConfigurationRoot _configuration;
        private static readonly IServiceScopeFactory _scopeFactory;
        static SliceFixture()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            _configuration = builder.Build();

            var startup = new Startup(_configuration);
            var services = new ServiceCollection();

            IServiceProvider sp = startup.ConfigureServices(services);
            _scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
        }

        public static async Task ResetCheckpoint()
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                BotContext context = scope.ServiceProvider.GetRequiredService<BotContext>();
                await context.Database.EnsureDeletedAsync();

                await new BotContextSeed()
                    .SeedAsync(context, null, scope.ServiceProvider.GetRequiredService<IOptions<BotSettings>>(), scope.ServiceProvider.GetRequiredService<ILogger<BotContextSeed>>());
            }
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
                T result = await action(scope.ServiceProvider).ConfigureAwait(false);

                return result;
            }
        }

        public static async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                await action(scope.ServiceProvider).ConfigureAwait(false);
            }
        }

        //public static Task<T> FindAsync<T>(int id)
        //    where T : class, IEntity
        //{
        //    return ExecuteDbContextAsync<T>(db => db.Set<T>().FindAsync(id).AsTask());
        //}

        public static Task<T> ExecuteDbContextAsync<T>(Func<BotContext, Task<T>> action)
        {
            return ExecuteScopeAsync(sp => action(sp.GetService<BotContext>()));
        }
    }
}
