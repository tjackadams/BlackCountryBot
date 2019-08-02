using System.Threading;
using System.Threading.Tasks;
using Bot.Domain.AggregatesModel.TranslationAggregate;
using MediatR;

namespace Bot.API.Application.Commands
{
    public class TweetTranslationCommandHandler : IRequestHandler<TweetTranslationCommand, bool>
    {
        private readonly ITranslationRepository _translationRepository;
        public TweetTranslationCommandHandler(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }
        public async Task<bool> Handle(TweetTranslationCommand message, CancellationToken cancellationToken)
        {
            Translation translation = await _translationRepository.GetAsync(message.TranslationId);
            if (translation == null)
            {
                return false;
            }

            translation.SendTweet();

            return await _translationRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
