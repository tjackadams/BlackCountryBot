using System.Threading;
using System.Threading.Tasks;
using Bot.Domain.AggregatesModel.TranslationAggregate;
using MediatR;

namespace Bot.API.Application.Commands
{
    public class CreateTranslationHandler : IRequestHandler<CreateTranslationCommand, bool>
    {
        private readonly ITranslationRepository _translationRepository;
        public CreateTranslationHandler(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }
        public async Task<bool> Handle(CreateTranslationCommand message, CancellationToken cancellationToken)
        {
            var translation = new Translation(
                message.OriginalPhrase,
                message.TranslatedPhrase
                );

            _translationRepository.Add(translation);

            return await _translationRepository.UnitOfWork
                .SaveEntitiesAsync();
        }
    }
}
