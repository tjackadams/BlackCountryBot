using System;

namespace FunctionHandler.Infrastructure
{
    public partial class Translations
    {
        public string GetTweetInformation()
        {
            LastTweetTime = DateTimeOffset.UtcNow;
            TweetCount += 1;

            return $"{OriginalPhrase}\n\nTranslation: {TranslatedPhrase}\n\n#BlackCountry";
        }
    }
}
