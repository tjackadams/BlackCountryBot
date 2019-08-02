using MediatR;

namespace Bot.API.Application.Commands
{
    public class SetTranslationContentCommand : IRequest<bool>
    {
        public int TranslationId { get; private set; }
        public string OriginalPhrase { get; private set; }
        public string TranslatedPhrase { get; private set; }

        public SetTranslationContentCommand(int translationId, string originalPhrase, string translatedPhrase)
        {
            TranslationId = translationId;
            OriginalPhrase = originalPhrase;
            TranslatedPhrase = translatedPhrase;
        }
    }
}
