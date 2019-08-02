using Bot.API.Application.Commands;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Bot.API.Application.Validations
{
    public class CreateTranslationCommandValidator : AbstractValidator<CreateTranslationCommand>
    {

        public CreateTranslationCommandValidator(ILogger<CreateTranslationCommandValidator> logger)
        {
            RuleFor(m => m.OriginalPhrase)
                .NotEmpty()
                .WithMessage("Original Phrase cannot be empty!")
                .MaximumLength(140)
                .WithMessage("Original Phrase must be less than 140 characters");

            RuleFor(m => m.TranslatedPhrase)
                .NotEmpty()
                .WithMessage("Translated Phrase cannot be empty!")
                .MaximumLength(140)
                .WithMessage("Translated Phrase must be less than 140 characters");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
