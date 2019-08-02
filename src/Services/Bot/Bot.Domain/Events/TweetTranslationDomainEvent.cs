using Bot.Domain.AggregatesModel.TranslationAggregate;
using MediatR;

namespace Bot.Domain.Events
{
    public class TweetTranslationDomainEvent : INotification
    {
        public Translation Translation { get; }

        public TweetTranslationDomainEvent(Translation translation)
        {
            Translation = translation;
        }
    }
}
