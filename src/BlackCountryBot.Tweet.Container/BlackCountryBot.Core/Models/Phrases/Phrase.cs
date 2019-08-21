using System;
using System.Linq;
using BlackCountryBot.Core.Infrastructure;
using MediatR;

namespace BlackCountryBot.Core.Models.Phrases
{
    public class Phrase : AggregateRoot
    {
        private Phrase()
        {
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

        public string Tweet()
        {
            LastTweetTime = DateTimeOffset.Now;
            NumberOfTweets++;
            return $"{Original} \n\n Translation: {Translation} \n\n #BlackCountry";
        }

        public Phrase UpdatePhrase(string newPhrase)
        {
            newPhrase = string.IsNullOrWhiteSpace(newPhrase) ? throw new ArgumentNullException(nameof(newPhrase)) : newPhrase;

            Original = newPhrase;

            if (!GetDomainEvents().OfType<PhraseUpdatedNotification>().Any())
            {
                AddDomainEvent(new PhraseUpdatedNotification());
            }

            return this;
        }

        public Phrase UpdateTranslation(string newTranslation)
        {
            newTranslation = string.IsNullOrWhiteSpace(newTranslation) ? throw new ArgumentNullException(nameof(newTranslation)) : newTranslation;

            Translation = newTranslation;

            if (!GetDomainEvents().OfType<PhraseUpdatedNotification>().Any())
            {
                AddDomainEvent(new PhraseUpdatedNotification());
            }

            return this;
        }
    }

    public class PhraseCreatedNotification : INotification
    {

    }

    public class PhraseDeletedNotification : INotification
    {

    }

    public class PhraseUpdatedNotification : INotification
    {

    }
}
