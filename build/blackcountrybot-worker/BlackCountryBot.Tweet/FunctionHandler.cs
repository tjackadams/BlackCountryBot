﻿using System;
using System.Linq;
using BlackCountryBot.Core.Infrastructure;
using BlackCountryBot.Core.Models.Phrases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tweetinvi;

namespace BlackCountryBot.Worker
{
    public class FunctionHandler
    {
        private static readonly Random Random = new Random(Guid.NewGuid().GetHashCode());
        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .Build();
        public string Handle(string _)
        {
            try
            {
                IServiceProvider serviceProvider = BuildServiceProvider();

                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    ILoggerFactory loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                    ILogger logger = loggerFactory.CreateLogger("Tweet Function");

                    try
                    {
                        logger.LogInformation("begin execution");

                        IRepository<Phrase> repo = scope.ServiceProvider.GetRequiredService<IRepository<Phrase>>();

                        logger.LogInformation("getting 10 unloved tweets.");

                        var lastTen = repo.GetAll()
                            .Where(p => !p.LastTweetTime.HasValue || p.LastTweetTime < DateTimeOffset.UtcNow.AddDays(-7))
                            .OrderBy(p => p.NumberOfTweets)
                            .Take(10)
                            .ToList();

                        Phrase phrase = lastTen[Random.Next(lastTen.Count)];

                        logger.LogInformation("the chosen phrase {@Phrase}", phrase);


                        Tweet.PublishTweet(phrase.Tweet());
                        repo.Update(phrase);
                    }
                    catch (Exception ex)
                    {
                        return FormatException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return FormatException(ex);
            }

            return "Ran to completion";
        }

        private static string FormatException(Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                message = ex.Message,
                exception = ex.GetType().Name,
                stackTrace = ex.StackTrace
            });
        }

        private static IServiceProvider BuildServiceProvider()
        {
            Auth.SetUserCredentials(
                Configuration["consumerKey"],
                 Configuration["conumerSecret"],
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
