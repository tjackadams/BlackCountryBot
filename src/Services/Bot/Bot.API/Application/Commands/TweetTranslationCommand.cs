using MediatR;

namespace Bot.API.Application.Commands
{
    public class TweetTranslationCommand : IRequest<bool>
    {
        public TweetTranslationCommand(int translationId)
        {
            TranslationId = translationId;
        }

        public int TranslationId { get; }
    }
}
