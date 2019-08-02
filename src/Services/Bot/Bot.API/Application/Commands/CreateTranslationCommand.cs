using MediatR;

namespace Bot.API.Application.Commands
{
    public class CreateTranslationCommand : IRequest<bool>
    {
        public CreateTranslationCommand(string originalPhrase, string translatedPhrase)
        {
            OriginalPhrase = originalPhrase;
            TranslatedPhrase = translatedPhrase;
        }

        public string OriginalPhrase { get; set; }
        public string TranslatedPhrase { get; set; }
    }
}
