using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using Bot.Domain.AggregatesModel.TranslationAggregate;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Polly;

namespace Bot.API.Infrastructure
{
    public class BotContextSeed
    {
        public async Task SeedAsync(BotContext context, IWebHostEnvironment env, IOptions<BotSettings> settings, ILogger<BotContextSeed> logger)
        {
            AsyncPolicy policy = CreatePolicy(logger, nameof(BotContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                using (context)
                {
                    if (!context.Database.ProviderName.Contains("memory", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Database.Migrate();
                    }

                    if (!context.Translations.Any())
                    {
                        var phrases = new List<Translation>
                        {
                            new Translation("Yo sorted me buffday ahht?", "Have you made arrangements for my birthday yet?"),
                            new Translation("I bay gooin thea!", "I'm not going there!"),
                            new Translation("Ah bin ya an won yow want?", "How have you been and how can i help?"),
                            new Translation("Ya babbies a boster!", "Your baby is beautiful!"),
                            new Translation("Giz fowa faggots wun ya mar mate?", "Can i have 4 faggots please my good friend?"),
                            new Translation("Ol ask the oomen if er knows tha wench who's gooin out wiv our kid.", "I will ask my wife if she knows my brothers new girlfriend.")
                    };

                        await context.Translations.AddRangeAsync(phrases);
                        await context.SaveChangesAsync();
                    }
                }
            });
        }

        private static AsyncPolicy CreatePolicy(ILogger<BotContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<NpgsqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
