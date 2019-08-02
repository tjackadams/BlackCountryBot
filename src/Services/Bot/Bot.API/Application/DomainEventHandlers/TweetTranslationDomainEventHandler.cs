using System.Threading;
using System.Threading.Tasks;
using Bot.Domain.Events;
using MediatR;
using Tweetinvi;

namespace Bot.API.Application.DomainEventHandlers
{
    public class TweetTranslationDomainEventHandler : INotificationHandler<TweetTranslationDomainEvent>
    {
        public async Task Handle(TweetTranslationDomainEvent notification, CancellationToken cancellationToken)
        {
            TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;

            await TweetAsync.PublishTweet(notification.Translation.GetTweetInformation());
        }
    }
}
