using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using BlackCountryBot.Core.Models.Phrases;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlackCountryBot.Web.Features.Phrases
{
    public class Update
    {
        public class Command : IRequest
        {
            public int Id { get; set; }
            public string Original { get; set; }
            public string Translation { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IRepository<Phrase> _repository;
            public Handler(IRepository<Phrase> repository)
            {
                _repository = repository;
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var phrase = await _repository.GetAsync(request.Id, cancellationToken);
                if(phrase == null)
                {
                    return Unit.Value;
                }

                phrase = phrase
                    .UpdatePhrase(request.Original)
                    .UpdateTranslation(request.Translation);

                await _repository.UpdateAsync(phrase, cancellationToken);

                return Unit.Value;
            }
        }
    }

    public class UpdateValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : Update.Command
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public UpdateValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
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

    public class UpdateValidator : AbstractValidator<Update.Command>
    {
        private readonly IRepository<Phrase> _repository;
        public UpdateValidator(IRepository<Phrase> repository)
        {
            _repository = repository;

            RuleFor(m => m.Id).NotEmpty().MustAsync(ExistingPhrase);
            RuleFor(m => m.Original).NotEmpty().WithMessage("Original Phrase cannot be empty!");
            RuleFor(m => m.Translation).NotEmpty().WithMessage("Translated Phrase cannot be empty!");
        }

        private Task<bool> ExistingPhrase(Update.Command command, int phraseId, CancellationToken cancellationToken)
        {
            return _repository.GetAll().AnyAsync(p => p.Id == phraseId, cancellationToken);
        }
    }
}
