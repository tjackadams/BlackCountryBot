using System.Threading;
using System.Threading.Tasks;
using Bot.Domain.AggregatesModel.TranslationAggregate;
using Bot.Infrastructure.Idempotency;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bot.API.Application.Commands
{
    public class SetTranslationContentCommandHandler : IRequestHandler<SetTranslationContentCommand, bool>
    {
        private readonly ITranslationRepository _translationRepository;
        public SetTranslationContentCommandHandler(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }
        public async Task<bool> Handle(SetTranslationContentCommand message, CancellationToken cancellationToken)
        {
            Translation translationToUpdate = await _translationRepository.GetAsync(message.TranslationId);
            if (translationToUpdate == null)
            {
                return false;
            }

            translationToUpdate.SetOriginalPhrase(message.OriginalPhrase);
            translationToUpdate.SetTranslatedPhrase(message.TranslatedPhrase);

            return await _translationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }

    public class SetTranslationContentIdentifiedCommandHandler : IdentifiedCommandHandler<SetTranslationContentCommand, bool>
    {
        public SetTranslationContentIdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager, ILogger<IdentifiedCommandHandler<SetTranslationContentCommand, bool>> logger) 
            : base(mediator, requestManager, logger)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;
        }
    }
}
