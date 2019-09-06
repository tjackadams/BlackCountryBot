using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlackCountryBot.Core.Infrastructure
{
    public interface IDbContextSeed<TDbContext>
        where TDbContext : DbContext
    {
        Task SeedAsync(TDbContext context, ILogger<TDbContext> logger, int? retry = 0);
    }
}
