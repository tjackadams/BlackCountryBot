using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure
{
    public interface IDbContextProvider<out TDbContext>
        where TDbContext : DbContext
    {
        TDbContext GetDbContext();
    }
}
