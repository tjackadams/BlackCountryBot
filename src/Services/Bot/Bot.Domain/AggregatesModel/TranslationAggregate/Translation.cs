using System;
using Bot.Domain.Events;
using Bot.Domain.Exceptions;
using Bot.Domain.Seedwork;

namespace Bot.Domain.AggregatesModel.TranslationAggregate
{
    public class Translation : Entity, IAggregateRoot
    {
        protected Translation()
        {
            CreatedTime = DateTimeOffset.UtcNow;
        }

        public Translation(string original, string translation)
            : this()
        {
            OriginalPhrase = original;
            TranslatedPhrase = translation;
        }

        public DateTimeOffset CreatedTime { get; private set; }
        public DateTimeOffset? LastTweetTime { get; private set; }
        public int TweetCount { get; private set; }
        public string OriginalPhrase { get; private set; }
        public string TranslatedPhrase { get; private set; }

        public void SetOriginalPhrase(string originalPhrase)
        {
            OriginalPhrase = originalPhrase;
        }

        public void SetTranslatedPhrase(string translatedPhrase)
        {
            TranslatedPhrase = translatedPhrase;
        }

        public Translation SendTweet()
        {
            LastTweetTime = DateTimeOffset.UtcNow;
            TweetCount += 1;

            AddDomainEvent(new TweetTranslationDomainEvent(this));

            return this;
        }

        public string GetTweetInformation()
        {
            return $"{OriginalPhrase}\n\nTranslation: {TranslatedPhrase}\n\n#BlackCountry";
        }
    }
}
