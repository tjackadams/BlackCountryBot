using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BlackCountryBot.Core.Infrastructure;
using BlackCountryBot.Core.Models.Phrases;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlackCountryBot.Web.Features.Phrases
{
    public class GetAll
    {
        public class Query : IRequest<Result>
        {

        }

        public class Result
        {
            public List<PhraseViewModel> Phrases { get; set; }
            public class PhraseViewModel
            {

            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Phrase, Result.PhraseViewModel>();
            }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IRepository<Phrase> _repo;
            private readonly IConfigurationProvider _configuration;
            public Handler(IRepository<Phrase> repo, IConfigurationProvider configuration)
            {
                _repo = repo;
                _configuration = configuration;
            }

            public async Task<Result> Handle(Query message, CancellationToken token)
            {
                List<Result.PhraseViewModel> phrases = await _repo.GetAll()
                    .OrderBy(p => p.LastTweetTime)
                    .ProjectTo<Result.PhraseViewModel>(_configuration)
                    .ToListAsync();

                return new Result
                {
                    Phrases = phrases
                };
            }
        }
    }
}
