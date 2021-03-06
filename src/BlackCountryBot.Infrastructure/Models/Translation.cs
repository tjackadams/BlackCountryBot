﻿using System;

namespace BlackCountryBot.Shared.Translations.Models
{
    public class Translation
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset? LastTweetTime { get; set; }
        public int TweetCount { get; set; }
        public string OriginalPhrase { get; set; }
        public string TranslatedPhrase { get; set; }
    }
}