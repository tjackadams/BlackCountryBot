using Microsoft.EntityFrameworkCore;

namespace BlackCountryBot.Core.Infrastructure
{
    public interface IDbContextProvider<out TDbContext>
        where TDbContext : DbContext
    {
        TDbContext GetDbContext();
    }
}
