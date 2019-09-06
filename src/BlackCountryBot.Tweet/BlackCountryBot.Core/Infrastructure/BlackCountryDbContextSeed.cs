using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackCountryBot.Core.Models.Phrases;
using Microsoft.Extensions.Logging;

namespace BlackCountryBot.Core.Infrastructure
{
    public class BlackCountryDbContextSeed : IDbContextSeed<BlackCountryDbContext>
    {
        public async Task SeedAsync(BlackCountryDbContext context, ILogger<BlackCountryDbContext> logger, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            try
            {
                if (!context.Phrases.Any())
                {
                    var phrases = new List<Phrase>
                    {
                        Phrase.Create("Yo sorted me buffday ahht?", "Have you made arrangements for my birthday yet?"),
                        Phrase.Create("I bay gooin thea!", "I'm not going there!"),
                        Phrase.Create("Ah bin ya an won yow want?", "How have you been and how can i help?"),
                        Phrase.Create("Ya babbies a boster!", "Your baby is beautiful!"),
                        Phrase.Create("Giz fowa faggots wun ya mar mate?", "Can i have 4 faggots please my good friend?"),
                        Phrase.Create("Ol ask the oomen if er knows tha wench who's gooin out wiv our kid.", "I will ask my wife if she knows my brothers new girlfriend.")
                    };

                    await context.Phrases.AddRangeAsync(phrases);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;

                    logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(BlackCountryDbContext));

                    await SeedAsync(context, logger, retryForAvailability);
                }
            }
        }
    }
}
