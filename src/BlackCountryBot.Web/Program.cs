using System;
using BlackCountryBot.Core.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlackCountryBot.Web
{
    public class Program
    {
        public static readonly string AppName = "BlackCountryBot.Web";
        public static int Main(string[] args)
        {
            IWebHost host = CreateWebHostBuilder(args).Build();
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    logger.LogInformation("Applying migrations ({ApplicationContext})...", AppName);
                    host.MigrateDbContext<BlackCountryDbContext>((context, sp) =>
                    {
                        ILogger<BlackCountryDbContextSeed> seedLogger = sp.GetRequiredService<ILogger<BlackCountryDbContextSeed>>();
                        new BlackCountryDbContextSeed()
                        .SeedAsync(context, seedLogger)
                        .Wait();
                    });

                    logger.LogInformation("Starting web host ({ApplicationContext})...", AppName);
                    host.Run();

                    return 0;
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);

                    return 1;
                }
            }

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(false)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables();
                })
                .UseStartup<Startup>();
        }
    }
}
