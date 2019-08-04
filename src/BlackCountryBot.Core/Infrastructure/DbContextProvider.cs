using Microsoft.EntityFrameworkCore;

namespace BlackCountryBot.Core.Infrastructure
{
    public class DbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
        where TDbContext : DbContext
    {
        public DbContextProvider(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public TDbContext DbContext { get; }
        public TDbContext GetDbContext()
        {
            return DbContext;
        }
    }
}
