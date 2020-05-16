using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BlackCountryBot.Infrastructure;
using BlackCountryBot.Shared.Translations.Models;
using Blazorise;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BlackCountryBot.Web.Pages.Translations
{
    public partial class Create
    {
        public Create()
        {
            Data = new Command();
        }

        [Inject]
        private IMediator _mediator { get; set; }

        [Inject]
        private NavigationManager _router { get; set; }

        public Command Data { get; set; }

        public Validations Validations { get; set; }

        public async Task OnSubmitAsync()
        {
            if (Validations.ValidateAll())
            {
                Validations.ClearAll();

                await _mediator.Send(Data);

                _router.NavigateTo("/translations");
            }
        }

        public class Command : IRequest<int>
        {
            [Required(ErrorMessage = "Please enter a phrase.")]
            [MaxLength(120, ErrorMessage = "Sorry - The length of this phrase is too long to tweet!")]
            public string OriginalPhrase { get; set; }

            [Required(ErrorMessage = "Please enter a phrase.")]
            [MaxLength(120, ErrorMessage = "Sorry - The length of this phrase is too long to tweet!")]
            public string TranslatedPhrase { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Command, Translation>(MemberList.Source);
            }
        }

        public class Handler : IRequestHandler<Command, int>
        {
            private readonly BlackCountryContext _db;
            private readonly IMapper _mapper;

            public Handler(IMapper mapper, BlackCountryContext db)
            {
                _mapper = mapper;
                _db = db;
            }

            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                var translation = _mapper.Map<Translation>(request);

                await _db.AddAsync(translation, cancellationToken);

                await _db.SaveEntitiesAsync(cancellationToken);

                return translation.Id;
            }
        }
    }
}