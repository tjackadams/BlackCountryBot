using System;
using System.IO;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using BlackCountryBot.Web;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlackCountryBot.IntegrationTests
{
    public class SliceFixture
    {
        private static readonly IConfigurationRoot _configuration;
        private static readonly IServiceScopeFactory _scopeFactory;
        private static readonly IServiceProvider _serviceProvider;
        static SliceFixture()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();
            _configuration = builder.Build();

            var startup = new Startup(_configuration);
            var services = new ServiceCollection();

            // override db context
            services.AddDbContext<BlackCountryDbContext>(o =>
            {
                o.UseInMemoryDatabase(databaseName: "BlackCountry");
            });

            startup.ConfigureServices(services);
            _serviceProvider = new Container(rules =>
                    // optional: Enables property injection for Controllers
                    // In current setup `WithMef` it will be overriden by properties marked with `ImportAttribute`
                    rules.With(propertiesAndFields: request => request.ServiceType.Name.EndsWith("Controller")
                        ? PropertiesAndFields.Properties()(request)
                        : null)
                )
                .WithDependencyInjectionAdapter(services)
                .WithCompositionRoot<CompositionRoot>();
            _scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        public static async Task ResetCheckpoint()
        {
            BlackCountryDbContext context = _serviceProvider.GetRequiredService<BlackCountryDbContext>();
            await context.Database.EnsureDeletedAsync();

            await new BlackCountryDbContextSeed()
            .SeedAsync(context, NullLogger<BlackCountryDbContextSeed>.Instance);
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
#pragma warning disable IDE0063 // Use simple 'using' statement
            using (IServiceScope scope = _scopeFactory.CreateScope())
#pragma warning restore IDE0063 // Use simple 'using' statement
            {
                T result = await action(scope.ServiceProvider).ConfigureAwait(false);

                return result;
            }
        }

        public static Task<T> FindAsync<T>(int id)
            where T : class, IEntity
        {
            return ExecuteDbContextAsync(db => db.Set<T>().FindAsync(id));
        }

        public static Task<T> ExecuteDbContextAsync<T>(Func<BlackCountryDbContext, Task<T>> action)
        {
            return ExecuteScopeAsync(sp => action(sp.GetService<BlackCountryDbContext>()));
        }
    }
}
