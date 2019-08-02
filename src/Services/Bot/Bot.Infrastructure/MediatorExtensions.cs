using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using Bot.Domain.Seedwork;
using MediatR;

namespace Bot.Infrastructure
{
    internal static class MediatorExtensions
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, BotContext context)
        {
            IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Entity>> domainEntities = context.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            INotification[] domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToArray();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            IEnumerable<Task> tasks = domainEvents
                .Select(async (domainEvent) =>
                {
                    await mediator.Publish(domainEvent);
                });

            await Task.WhenAll(tasks);
        }
    }
}
