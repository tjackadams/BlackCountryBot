using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using BlackCountryBot.Core.Models.Phrases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tweetinvi;

namespace BlackCountryBot.Core.Features.Phrases
{
    public class SubmitRandomTweet
    {
        public class Command : IRequest<Result>
        {
        }

        public class Result
        {
            public Phrase Phrase { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private static readonly Random Random = new Random(Guid.NewGuid().GetHashCode());
            private readonly IRepository<Phrase> _repository;

            public Handler(ILogger<SubmitRandomTweet> logger, IRepository<Phrase> repository)
            {
                Logger = logger ?? NullLogger<SubmitRandomTweet>.Instance;

                _repository = repository;
            }

            public ILogger<SubmitRandomTweet> Logger { get; private set; }
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;

                List<Phrase> lastTen = await _repository.GetAll()
                    .Where(p => !p.LastTweetTime.HasValue || p.LastTweetTime < DateTimeOffset.UtcNow.AddDays(-7))
                    .OrderBy(p => p.NumberOfTweets)
                    .Take(10)
                    .ToListAsync(cancellationToken);

                if (!lastTen.Any())
                {
                    return new Result { Phrase = null };
                }

                Phrase phrase = lastTen[Random.Next(lastTen.Count)];

                Logger.LogInformation("todays chosen tweet {@Tweet}", phrase);

                if (phrase.IsTweetTooLong)
                {
                    Logger.LogError("tweet too long! {TweetLength}", phrase.Tweet().Length);
                    return new Result { Phrase = null };
                }

                try
                {
                    await TweetAsync.PublishTweet(phrase.Tweet());
                    await _repository.UpdateAsync(phrase, cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                    return new Result { Phrase = null };
                }

                return new Result
                {
                    Phrase = phrase
                };
            }
        }
    }
}
