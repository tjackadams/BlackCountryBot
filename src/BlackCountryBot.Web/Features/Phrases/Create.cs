using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using BlackCountryBot.Core.Models.Phrases;
using FluentValidation;
using MediatR;

namespace BlackCountryBot.Web.Features.Phrases
{
    public class Create
    {
        public class Command : IRequest<Result>
        {
            public string Original { get; set; }
            public string Translation { get; set; }
        }

        public class Result
        {
            public int PhraseId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IMediator _mediator;
            private readonly IRepository<Phrase> _repository;
            public Handler(IMediator mediator, IRepository<Phrase> repository)
            {
                _mediator = mediator;
                _repository = repository;
            }
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var phrase = Phrase.Create(request.Original, request.Translation);

                await _repository.InsertAsync(phrase, cancellationToken);

                await _mediator.Publish(new PhraseCreatedNotification(), cancellationToken);

                return new Result
                {
                    PhraseId = phrase.Id
                };
            }
        }
    }

    public class CreateValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : Create.Command
        where TResponse : Create.Result
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public CreateValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext(request);
            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            return next();
        }
    }

    public class CreateValidator : AbstractValidator<Create.Command>
    {
        public CreateValidator()
        {
            RuleFor(m => m.Original).NotEmpty().WithMessage("Original Phrase cannot be empty!");
            RuleFor(m => m.Translation).NotEmpty().WithMessage("Translated Phrase cannot be empty!");
        }
    }
}
