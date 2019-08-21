using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.Core.Models.Phrases;
using BlackCountryBot.Web.Features.Phrases;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace BlackCountryBot.Web.Hubs
{
    public class PhrasesHub : Hub
    {
        private readonly IMediator _mediator;
        public PhrasesHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task OnConnectedAsync()
        {
            GetAll.Result phrases = await _mediator.Send(new GetAll.Query());
            await Clients.All.SendAsync("getAllPhrases", phrases.Phrases);
        }

        public Task CreatePhrase(Create.Command command)
        {
            return _mediator.Send(command);
        }

        public Task DeletePhrase(Delete.Command command)
        {
            return _mediator.Send(command);
        }

        public Task UpdatePhrase(Update.Command command)
        {
            return _mediator.Send(command);
        }
    }

    public class PhraseHubDispatcher : INotificationHandler<PhraseCreatedNotification>, INotificationHandler<PhraseDeletedNotification>, INotificationHandler<PhraseUpdatedNotification>
    {
        private readonly IHubContext<PhrasesHub> _hub;
        private readonly IMediator _mediator;
        public PhraseHubDispatcher(IHubContext<PhrasesHub> hub, IMediator mediator)
        {
            _hub = hub;
            _mediator = mediator;
        }

        public Task Handle(PhraseCreatedNotification notification, CancellationToken cancellationToken)
        {
            return SendAllPhrasesAsync(cancellationToken);
        }

        public Task Handle(PhraseDeletedNotification notification, CancellationToken cancellationToken)
        {
            return SendAllPhrasesAsync(cancellationToken);
        }

        public Task Handle(PhraseUpdatedNotification notification, CancellationToken cancellationToken)
        {
            return SendAllPhrasesAsync(cancellationToken);
        }

        private async Task SendAllPhrasesAsync(CancellationToken cancellationToken)
        {
            GetAll.Result phrases = await _mediator.Send(new GetAll.Query(), cancellationToken);
            await _hub.Clients.All.SendAsync("getAllPhrases", phrases.Phrases, cancellationToken);
        }
    }
}
