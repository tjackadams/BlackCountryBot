using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using BlackCountryBot.Core.Models.Phrases;
using MediatR;

namespace BlackCountryBot.Core.Features.Phrases
{
    public class Delete
    {
        public class Command : IRequest
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IMediator _mediator;
            private readonly IRepository<Phrase> _repository;
            public Handler(IMediator mediator, IRepository<Phrase> repository)
            {
                _mediator = mediator;
                _repository = repository;
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var phrase = await _repository.GetAsync(request.Id, cancellationToken);
                if(phrase == null)
                {
                    return Unit.Value;
                }

                await _repository.DeleteAsync(phrase, cancellationToken);
                await _mediator.Publish(new PhraseDeletedNotification(), cancellationToken);

                return Unit.Value;
            }
        }
    }
}
