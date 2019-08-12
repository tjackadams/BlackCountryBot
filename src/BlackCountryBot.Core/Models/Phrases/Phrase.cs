using System;
using BlackCountryBot.Core.Infrastructure;
using MediatR;

namespace BlackCountryBot.Core.Models.Phrases
{
    public class Phrase : AggregateRoot
    {
        private Phrase()
        {
            AddDomainEvent(new PhraseCreatedNotification());
        }

        public static Phrase Create(string original, string translation)
        {
            original = string.IsNullOrWhiteSpace(original) ? throw new ArgumentNullException(nameof(original)) : original;
            translation = string.IsNullOrWhiteSpace(translation) ? throw new ArgumentNullException(nameof(translation)) : translation;

            return new Phrase
            {
                CreatedTime = DateTimeOffset.UtcNow,
                NumberOfTweets = 0,
                Original = original,
                Translation = translation
            };
        }

        public string Original { get; private set; }
        public string Translation { get; private set; }
        public DateTimeOffset? LastTweetTime { get; private set; }
        public DateTimeOffset CreatedTime { get; private set; }
        public int NumberOfTweets { get; private set; }
    }

    public class PhraseCreatedNotification : INotification
    {

    }

}
