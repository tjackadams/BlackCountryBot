using System;
using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using BlackCountryBot.Core.Models.Phrases;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tweetinvi;

namespace BlackCountryBot.Core.Features.Phrases
{
    public class SubmitTweet
    {
        public class Command : IRequest
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IRepository<Phrase> _repository;

            public Handler(ILogger<SubmitTweet> logger, IRepository<Phrase> repository)
            {
                Logger = logger ?? NullLogger<SubmitTweet>.Instance;

                _repository = repository;
            }

            public ILogger<SubmitTweet> Logger { get; private set; }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;

                Phrase phrase = await _repository.GetAsync(request.Id, cancellationToken);
                if (phrase == null)
                {
                    return Unit.Value;
                }

                Logger.LogInformation("manually tweeting {@Tweet}", phrase);

                if (phrase.IsTweetTooLong)
                {
                    Logger.LogError("tweet too long! {TweetLength}", phrase.Tweet().Length);
                    return Unit.Value;
                }

                try
                {
                    await TweetAsync.PublishTweet(phrase.Tweet());
                    await _repository.UpdateAsync(phrase, cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                    return Unit.Value;
                }

                return Unit.Value;
            }
        }
    }
}
