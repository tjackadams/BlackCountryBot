using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BlackCountryBot.Infrastructure;
using BlackCountryBot.Shared.Translations.Models;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace BlackCountryBot.Web.Pages.Translations
{
    public partial class Index
    {
        [Inject]
        private IMediator _mediator { get; set; }

        public List<Result.TranslationModel> Data { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await _mediator.Send(new Query());
            Data = result.Translations;
        }

        public class Query : IRequest<Result>
        {
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly BlackCountryContext _db;
            private readonly IMapper _mapper;

            public Handler(BlackCountryContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var results = await _db.Translations.ToListAsync(cancellationToken);

                return new Result
                {
                    Translations = _mapper.Map<List<Result.TranslationModel>>(results)
                };
            }
        }

        public class Result
        {
            public List<TranslationModel> Translations { get; set; }

            public class TranslationModel
            {
                public int Id { get; set; }
                public string OriginalPhrase { get; set; }
                public string TranslatedPhrase { get; set; }
                public int TweetCount { get; set; }
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Translation, Result.TranslationModel>();
            }
        }
    }
}