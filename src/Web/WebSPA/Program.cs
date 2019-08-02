using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace WebSPA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .ConfigureAppConfiguration((builderContext, config) =>
                    {
                        config.AddEnvironmentVariables();
                    })
                    .ConfigureLogging((hostingContext, builder) =>
                    {
                        builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        builder.AddConsole();
                        builder.AddDebug();
                    })
                    .UseSerilog((builderContext, config) =>
                    {
                        config
                            .MinimumLevel.Information()
                            .Enrich.FromLogContext()
                            .WriteTo.Console();
                    })
                    .Build();
        }
    }
}
