using MediatR;

namespace Bot.API.Application.Commands
{
    public class DeleteTranslationCommand : IRequest<bool>
    {
        public DeleteTranslationCommand(int translationId)
        {
            TranslationId = translationId;
        }

        public int TranslationId { get; }
    }
}
