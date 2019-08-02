using System;
using AutoMapper;
using Bot.Domain.AggregatesModel.TranslationAggregate;

namespace Bot.API.Application.Queries
{
    public class TranslationSummaryViewModel
    {
        public int TranslationId { get; set; }
        public string OriginalPhrase { get; set; }
        public string TranslatedPhrase { get; set; }
        public DateTimeOffset? LastTweetTime { get; set; }
        public int TweetCount { get; set; }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Translation, TranslationSummaryViewModel>()
                .ForMember(dest => dest.TranslationId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
