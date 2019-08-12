using System;
using System.Collections.Generic;
using System.Linq;
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
            var phrases = await _mediator.Send(new GetAll.Query());
            await Clients.All.SendAsync("getAllPhrases", phrases.Phrases);
        }
    }

    public class PhraseHubDispatcher : INotificationHandler<PhraseCreatedNotification>
    {
        private readonly IHubContext<PhrasesHub> _hub;
        private readonly IMediator _mediator;
        public PhraseHubDispatcher(IHubContext<PhrasesHub> hub, IMediator mediator)
        {
            _hub = hub;
            _mediator = mediator;
        }

        public async Task Handle(PhraseCreatedNotification notification, CancellationToken cancellationToken)
        {
            var phrases = await _mediator.Send(new GetAll.Query());
            await _hub.Clients.All.SendAsync("GETALL_PHRASES", phrases.Phrases);
        }
    }
}
