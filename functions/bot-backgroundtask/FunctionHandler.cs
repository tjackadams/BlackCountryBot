using System;
using System.IO;
using System.Linq;
using FunctionHandler.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tweetinvi;
using static FunctionHandler.Infrastructure.Http.Responses;

namespace Function
{
    public class FunctionHandler
    {
        private static readonly Random _random = new Random(Guid.NewGuid().GetHashCode());

        public string Handle(string _)
        {
            try
            {
                TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;

                IConfiguration configuration = GetConfiguration();
                IServiceProvider sp = BuildServiceProvider(configuration);

                BotContext context = sp.GetRequiredService<BotContext>();

                DateTimeOffset sevenDaysAgo = DateTimeOffset.UtcNow.AddDays(-7);
                var validTranslations = context.Translations
                        .Where(t => t.LastTweetTime < sevenDaysAgo)
                        .OrderBy(t => t.TweetCount)
                        .Take(10)
                        .ToList();

                if (!validTranslations.Any())
                {
                    return InternalServerError("No translations are available to tweet.");
                }

                Translations translation = validTranslations[_random.Next(0, validTranslations.Count)];

                TweetAsync.PublishTweet(translation.GetTweetInformation()).Wait();

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return Ok();
        }

        private static IConfiguration GetConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings.secrets.json", false, true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        private static IServiceProvider BuildServiceProvider(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            // twitter
            Auth.SetUserCredentials(
                configuration["consumerKey"],
                configuration["consumerSecret"],
                configuration["userAccessToken"],
                configuration["userAccessSecret"]);

            services.AddDbContext<BotContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            }, ServiceLifetime.Scoped);

            return services.BuildServiceProvider();
        }
    }
}
