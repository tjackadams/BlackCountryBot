using System.Linq;
using System.Threading.Tasks;
using Bot.Domain.Seedwork;

namespace Bot.Domain.AggregatesModel.TranslationAggregate
{
    public interface ITranslationRepository : IRepository<Translation>
    {
        Translation Add(Translation translation);

        void Update(Translation translation);

        Task<Translation> GetAsync(int translationId);

        Task DeleteAsync(int translationId);

        IQueryable<Translation> Translations { get; }
    }
}
