using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlackCountryBot.Core.Infrastructure
{
    public abstract class Repository<TDbContext, TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
        where TDbContext : DbContext
    {
        private readonly IDbContextProvider<TDbContext> _dbContextProvider;

        protected Repository(IDbContextProvider<TDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public virtual TDbContext Context => _dbContextProvider.GetDbContext();

        public virtual DbSet<TEntity> Table => Context.Set<TEntity>();
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            EntityEntry entry = Context.ChangeTracker.Entries().FirstOrDefault(e => e.Entity == entity);
            if (entry == null)
            {
                Table.Attach(entity);
            }

            Table.Remove(entity);

            await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return GetQueryable();
        }

        public Task<TEntity> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            return GetAll()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            EntityEntry<TEntity> result = await Table.AddAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            EntityEntry<TEntity> result = Table.Update(entity);
            await Context.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }

        protected virtual IQueryable<TEntity> GetQueryable()
        {
            return Table.AsQueryable();
        }
    }
}
