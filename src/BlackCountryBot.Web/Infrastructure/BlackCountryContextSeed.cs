using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackCountryBot.Infrastructure;
using BlackCountryBot.Shared.Translations.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;

namespace BlackCountryBot.Server.Infrastructure
{
    public class BlackCountryContextSeed
    {
        public async Task SeedAsync(BlackCountryContext context, IWebHostEnvironment env,
            ILogger<BlackCountryContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(BlackCountryContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                await using (context)
                {
                    await context.Database.MigrateAsync();

                    if (!await context.Translations.AnyAsync())
                    {
                        var phrases = new List<Translation>
                        {
                            new Translation
                            {
                                OriginalPhrase = "Yo sorted me buffday ahht?",
                                TranslatedPhrase = "Have you made arrangements for my birthday yet?"
                            },
                            new Translation
                                {OriginalPhrase = "I bay gooin thea!", TranslatedPhrase = "I'm not going there!"},
                            new Translation
                            {
                                OriginalPhrase = "Ah bin ya an won yow want?",
                                TranslatedPhrase = "How have you been and how can i help?"
                            },
                            new Translation
                                {OriginalPhrase = "Ya babbies a boster!", TranslatedPhrase = "Your baby is beautiful!"},
                            new Translation
                            {
                                OriginalPhrase = "Giz fowa faggots wun ya mar mate?",
                                TranslatedPhrase = "Can i have 4 faggots please my good friend?"
                            },
                            new Translation
                            {
                                OriginalPhrase = "Ol ask the oomen if er knows tha wench who's gooin out wiv our kid.",
                                TranslatedPhrase = "I will ask my wife if she knows my brothers new girlfriend."
                            }
                        };

                        await context.Translations.AddRangeAsync(phrases);
                    }

                    await context.SaveChangesAsync();
                }
            });
        }

        private static IAsyncPolicy CreatePolicy(ILogger<BlackCountryContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().WaitAndRetryAsync(
                retries,
                retry => TimeSpan.FromSeconds(5),
                (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception,
                        "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                        prefix, exception.GetType().Name, exception.Message, retry, retries);
                }
            );
        }
    }
}