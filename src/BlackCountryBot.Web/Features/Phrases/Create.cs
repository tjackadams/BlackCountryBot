using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using BlackCountryBot.Core.Models.Phrases;
using MediatR;

namespace BlackCountryBot.Web.Features.Phrases
{
    public class Create
    {
        public class Command : IRequest<int>
        {
            public string Original { get; set; }
            public string Translation { get; set; }
        }

        public class Handler : IRequestHandler<Command, int>
        {
            private readonly IRepository<Phrase> _repository;
            public Handler(IRepository<Phrase> repository)
            {
                _repository = repository;
            }
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                var phrase = Phrase.Create(request.Original, request.Translation);

                await _repository.InsertAsync(phrase, cancellationToken);

                return phrase.Id;
            }
        }
    }
}
