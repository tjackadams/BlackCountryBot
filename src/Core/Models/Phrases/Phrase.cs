using Core.Infrastructure;
using System;

namespace Core.Models.Phrases
{
    public class Phrase : AggregateRoot
    {
        private Phrase() { }

        public string Original { get; private set; }
        public string Translation { get; private set; }
        public DateTimeOffset? LastTweetTime { get; private set; }
        public DateTimeOffset CreatedTime { get; private set; }
        public int NumberOfTweets { get; private set; }
    }
}
