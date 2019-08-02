using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BlackCountryBot.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Bot.API.Application.Queries
{
    public class TranslationQueries : ITranslationQueries
    {
        private readonly IMapper _mapper;
        private readonly BotContext _context;
        public TranslationQueries(IMapper mapper, BotContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public Task<List<TranslationSummaryViewModel>> GetAllTranslationsAsync(int? top, CancellationToken cancellationToken = default)
        {
            var query = _context.Translations
                .OrderByDescending(t => t.TweetCount)
                .AsQueryable();

            if (top.HasValue)
            {
                query.Take(top.Value);
            }
                return query
                .ProjectTo<TranslationSummaryViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        public Task<TranslationSummaryViewModel> GetTranslationByIdAsync(int translationId, CancellationToken cancellationToken = default)
        {
            return _context.Translations
                .Where(t => t.Id == translationId)
                .ProjectTo<TranslationSummaryViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<TranslationSummaryViewModel> GetTranslationByPhraseAsync(string originalPhrase, CancellationToken cancellationToken = default)
        {
            return _context.Translations
                .Where(t => t.OriginalPhrase == originalPhrase)
                .ProjectTo<TranslationSummaryViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
