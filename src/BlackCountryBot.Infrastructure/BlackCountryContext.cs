using System;
using System.Data;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.Shared.Translations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;

namespace BlackCountryBot.Infrastructure
{
    public class BlackCountryContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "blackcountry";

        private IDbContextTransaction _currentTransaction;

        public BlackCountryContext(DbContextOptions<BlackCountryContext> options)
            : base(options)
        {
        }

        public DbSet<Translation> Translations { get; set; }

        public bool HasActiveTransaction => _currentTransaction != null;

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Translation>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).UseHiLo();

                e.Property(p => p.Id).HasColumnName("TranslationId");

                e.Property(p => p.OriginalPhrase).IsRequired().HasMaxLength(120);
                e.Property(p => p.TranslatedPhrase).IsRequired().HasMaxLength(120);
            });
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                await transaction.CommitAsync();
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
    }

    public class BlackCountryContextDesignFactory : IDesignTimeDbContextFactory<BlackCountryContext>
    {
        public BlackCountryContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BlackCountryContext>()
                .UseSqlServer("Server=.;Initial Catalog=BlackCountryDb;Integrated Security=true",
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly("BlackCountryBot.Server");
                    } );

            return new BlackCountryContext(optionsBuilder.Options);
        }
    }
}