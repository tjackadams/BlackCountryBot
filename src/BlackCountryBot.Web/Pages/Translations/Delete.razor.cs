using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BlackCountryBot.Infrastructure;
using BlackCountryBot.Shared.Translations.Models;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace BlackCountryBot.Web.Pages.Translations
{
    public partial class Delete
    {
        [Inject]
        private IMediator _mediator { get; set; }

        [Inject]
        private NavigationManager _router { get; set; }

        [Parameter]
        public int Id { get; set; }

        public Command Data { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Data = await _mediator.Send(new Query {Id = Id});
        }

        public async Task OnSubmitAsync()
        {
            await _mediator.Send(Data);

            _router.NavigateTo("/translations");
        }


        public class Query : IRequest<Command>
        {
            public int Id { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Translation, Command>();
            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly IConfigurationProvider _configuration;
            private readonly BlackCountryContext _db;

            public QueryHandler(BlackCountryContext db, IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public Task<Command> Handle(Query request, CancellationToken cancellationToken)
            {
                return _db.Translations
                    .Where(t => t.Id == request.Id)
                    .ProjectTo<Command>(_configuration)
                    .SingleOrDefaultAsync(cancellationToken);
            }
        }

        public class Command : IRequest
        {
            public int Id { get; set; }
            public string OriginalPhrase { get; set; }
            public string TranslatedPhrase { get; set; }
            public int TweetCount { get; set; }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly BlackCountryContext _db;

            public CommandHandler(BlackCountryContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var translation = await _db.Translations.FindAsync(request.Id);

                _db.Translations.Remove(translation);

                return default;
            }
        }
    }
}