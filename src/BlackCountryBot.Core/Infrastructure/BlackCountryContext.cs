using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.Core.Models.Phrases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlackCountryBot.Core.Infrastructure
{
    public class BlackCountryContext : DbContext
    {
        private readonly IMediator _mediator;

        public ILogger<BlackCountryContext> Logger { get; private set; }

        public BlackCountryContext(DbContextOptions<BlackCountryContext> options, IMediator mediator, ILogger<BlackCountryContext> logger)
            : base(options)
        {
            _mediator = mediator;

            Logger = logger ?? NullLogger<BlackCountryContext>.Instance;
        }

        public DbSet<Phrase> Phrases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Phrase>(e =>
            {
                e
                .Property(p => p.Id)
                .ForSqlServerUseSequenceHiLo();

                e
                .HasKey(p => p.Id);

                e
                .Property(p => p.Original)
                .HasMaxLength(100)
                .IsRequired();

                e
                .Property(p => p.Translation)
                .HasMaxLength(100)
                .IsRequired();
            });
        }

        public override int SaveChanges()
        {
            int result = base.SaveChanges();

            foreach (EntityEntry entry in ChangeTracker.Entries().ToArray())
            {
                TriggerDomainEventsAsync(entry).Wait();
            }

            return result;
        }

        private async Task TriggerDomainEventsAsync(EntityEntry entry, CancellationToken cancellationToken = default)
        {
            if (entry is IAggregateRoot agg)
            {
                IEnumerable<INotification> domainEvents = agg.GetDomainEvents();

                foreach (INotification domainEvent in domainEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }
            }
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            int result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            foreach (EntityEntry entry in ChangeTracker.Entries().ToArray())
            {
                await TriggerDomainEventsAsync(entry, cancellationToken);
            }

            return result;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            int result = base.SaveChanges(acceptAllChangesOnSuccess);
            foreach (EntityEntry entry in ChangeTracker.Entries().ToArray())
            {
                TriggerDomainEventsAsync(entry).Wait();
            }

            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            int result = await base.SaveChangesAsync(cancellationToken);
            foreach (EntityEntry entry in ChangeTracker.Entries().ToArray())
            {
                await TriggerDomainEventsAsync(entry, cancellationToken);
            }

            return result;
        }
    }

    public class BlackCountryContextFactory : IDesignTimeDbContextFactory<BlackCountryContext>
    {
        public BlackCountryContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BlackCountryContext>();
            optionsBuilder.UseSqlServer("Data Source=localhost;Integrated Security=True");

            return new BlackCountryContext(optionsBuilder.Options, null, NullLogger<BlackCountryContext>.Instance);
        }
    }
}
