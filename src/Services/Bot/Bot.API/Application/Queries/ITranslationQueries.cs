using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.API.Application.Queries
{
    public interface ITranslationQueries
    {
        Task<TranslationSummaryViewModel> GetTranslationByIdAsync(int translationId, CancellationToken cancellationToken = default);
        Task<TranslationSummaryViewModel> GetTranslationByPhraseAsync(string originalPhrase, CancellationToken cancellationToken = default);
        Task<List<TranslationSummaryViewModel>> GetAllTranslationsAsync(int? top, CancellationToken cancellationToken = default);
    }
}
