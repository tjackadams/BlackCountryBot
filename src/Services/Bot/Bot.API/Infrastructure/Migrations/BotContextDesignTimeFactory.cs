using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bot.API.Infrastructure.Migrations
{
    public class BotContextDesignTimeFactory : IDesignTimeDbContextFactory<BotContext>
    {
        public BotContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<BotContext> optionsBuilder = new DbContextOptionsBuilder<BotContext>()
                .UseNpgsql("Data Source=localhost;Integrated Security=True",
                 options => options.MigrationsAssembly(GetType().Assembly.GetName().Name));



            return new BotContext(optionsBuilder.Options, new NullMediator());
        }

        private class NullMediator : IMediator
        {
            public Task Publish(object notification, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
            {
                return Task.CompletedTask;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            {
                return Task.FromResult<TResponse>(default);
            }
        }
    }
}
