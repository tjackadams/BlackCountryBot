using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using Bot.Domain.AggregatesModel.TranslationAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bot.API.Application.Commands
{
    public class DeleteTranslationCommandHandler : IRequestHandler<DeleteTranslationCommand, bool>
    {
        private readonly BotContext _context;
        public DeleteTranslationCommandHandler(BotContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<bool> Handle(DeleteTranslationCommand request, CancellationToken cancellationToken)
        {
            Translation translation = await _context.Translations
                .Where(t => t.Id == request.TranslationId)
                .FirstOrDefaultAsync(cancellationToken);

            if (translation == null)
            {
                return true;
            }

            _context.Translations.Remove(translation);

            return await _context.SaveEntitiesAsync(cancellationToken);
        }
    }
}
