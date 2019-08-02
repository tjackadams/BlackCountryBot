using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Bot.Domain.AggregatesModel.TranslationAggregate;
using Bot.Domain.Seedwork;
using Bot.Infrastructure;
using Bot.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;

namespace BlackCountryBot.Core.Infrastructure
{
    public class BotContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "bot";
        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;
        public BotContext(DbContextOptions<BotContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            Debug.WriteLine("BotContext::ctor ->" + GetHashCode());
        }

        private BotContext(DbContextOptions<BotContext> options)
            : base(options) { }
        public bool HasActiveTransaction => _currentTransaction != null;

        public DbSet<Translation> Translations { get; set; }

        public async Task<IDbContextTransaction> BeingTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return null;
            }

            _currentTransaction = await Database.BeginTransactionAsync();

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction)
            {
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not the current transaction.");
            }

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public IDbContextTransaction GetCurrentTransaction()
        {
            return _currentTransaction;
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await _mediator.DispatchDomainEventsAsync(this);

            int _ = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClientRequestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TranslationEntityTypeConfiguration());
        }
    }

    public class BlackCountryContextFactory : IDesignTimeDbContextFactory<BotContext>
    {
        public BotContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<BotContext> optionsBuilder = new DbContextOptionsBuilder<BotContext>()
                .UseNpgsql("Data Source=localhost;Integrated Security=True");

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
