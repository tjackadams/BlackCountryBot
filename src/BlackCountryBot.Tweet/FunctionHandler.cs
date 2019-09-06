﻿using System;
using System.Diagnostics;
using BlackCountryBot.Core.Features.Phrases;
using BlackCountryBot.Core.Infrastructure;
using BlackCountryBot.Core.Models.Phrases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tweetinvi;

namespace Function
{
    public class FunctionHandler
    {
        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ? "appsettings.secrets.json" : "/var/openfaas/secrets/appsettings.secrets.json", false, true)
                    .AddEnvironmentVariables()
                    .Build();

        public string Handle(string _)
        {
            try
            {
                TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;

                IServiceProvider serviceProvider = BuildServiceProvider();
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    ILoggerFactory loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                    ILogger logger = loggerFactory.CreateLogger("Tweet Function");
                    IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    try
                    {
                        logger.LogInformation("begin execution");

                        mediator.Send(new SubmitRandomTweet.Command()).Wait();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ex.Message);
                        return ex.ToStringDemystified();
                    }

                    return "Ran to completion";
                }
            }
            catch (Exception ex)
            {
                return ex.ToStringDemystified();
            }
        }

        private static IServiceProvider BuildServiceProvider()
        {
            Auth.SetUserCredentials(
                 Configuration["consumerKey"],
                 Configuration["consumerSecret"],
                 Configuration["userAccessToken"],
                 Configuration["userAccessSecret"]);


            var services = new ServiceCollection();

            services.AddLogging(o => o.AddConsole().AddDebug());

            services.AddDbContext<BlackCountryDbContext>(options =>
            {
                options.UseSqlServer(Configuration["connectionString"]);
            });

            services.AddMediatR(typeof(Phrase).Assembly);

            services.AddScoped(typeof(IDbContextProvider<>), typeof(DbContextProvider<>));
            services.AddScoped<IRepository<Phrase>, Repository<BlackCountryDbContext, Phrase>>(
                sp => new Repository<BlackCountryDbContext, Phrase>(sp.GetRequiredService<IDbContextProvider<BlackCountryDbContext>>()));

            return services.BuildServiceProvider();
        }
    }
}
