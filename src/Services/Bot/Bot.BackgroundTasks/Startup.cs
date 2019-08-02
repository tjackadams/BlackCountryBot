using System;
using BlackCountryBot.Core.Infrastructure;
using Bot.BackgroundTasks.Extensions;
using Bot.BackgroundTasks.Tasks;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Tweetinvi;

namespace Bot.BackgroundTasks
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomHealthCheck(Configuration)
                .AddMediatR(typeof(Startup))
                .AddCustomDbContext(Configuration)
                .AddCustomIntegrations(Configuration)
                .AddCustomTasks();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }

    public static class CustomExtensionMethods
    {

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
                services.AddEntityFrameworkSqlServer()
                    .AddDbContext<BotContext>(options =>
                    {
                        options.UseSqlServer(configuration["ConnectionString"],
                            sqlServerOptionsAction: sqlOptions =>
                            {

                                sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                            });
                    }, ServiceLifetime.Scoped);
            

            return services;
        }

        public static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
        {
            // twitter
            Auth.SetUserCredentials(
                configuration["TwitterConsumerKey"],
                configuration["TwitterConsumerSecret"],
                configuration["TwitterUserAccessToken"],
                configuration["TwitterUserAccessSecret"]);

            return services;
        }

        public static IServiceCollection AddCustomTasks(this IServiceCollection services)
        {
            services.AddSingleton<IHostedService, SendTweetScheduledTask>();
            return services;
        }
    }
}
