using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using Bot.BackgroundTasks.Infrastructure;
using Bot.Domain.AggregatesModel.TranslationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tweetinvi;

namespace Bot.BackgroundTasks.Tasks
{
    public class SendTweetScheduledTask : ScheduledBackgroundService
    {
        public SendTweetScheduledTask(IServiceScopeFactory serviceScopeFactory) 
            : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "0 6,18 * * *";

        public override async Task ProcessInScope(IServiceProvider sp, CancellationToken cancellationToken)
        {
            var logger = sp.GetRequiredService<ILogger<SendTweetScheduledTask>>();
            try
            {               
                var context = sp.GetRequiredService<BotContext>();
                var random = new Random((Guid.NewGuid().GetHashCode()));

                DateTimeOffset sevenDaysAgo = DateTimeOffset.UtcNow.AddDays(-7);
                var validTranslations = await context.Translations
                        .Where(t => t.LastTweetTime < sevenDaysAgo)
                        .OrderBy(t => t.TweetCount)
                        .Take(10)
                        .ToListAsync(cancellationToken);

                if (!validTranslations.Any())
                {
                    logger.LogInformation("All Translations have been tweeted this week. Better luck next time. ");
                    return;
                }

                Translation randomTranslation = validTranslations.ElementAtOrDefault(random.Next(0, validTranslations.Count));
                if(randomTranslation == null)
                {
                    logger.LogWarning("Unable to pick a random Translation from the bunch.");
                    return;
                }

                await TweetAsync.PublishTweet(randomTranslation.SendTweet().GetTweetInformation());

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Something went seriously wrong tweeting a random Translation.");
                throw;
            }
        }
    }
}
