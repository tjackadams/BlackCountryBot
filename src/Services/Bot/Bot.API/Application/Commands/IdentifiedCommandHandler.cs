using System;
using System.Threading;
using System.Threading.Tasks;
using Bot.Infrastructure.Idempotency;
using MediatR;
using Microsoft.Extensions.Logging;
using BlackCountryBot.BuildingBlocks.EventBus.Extensions;

namespace Bot.API.Application.Commands
{
    public class IdentifiedCommandHandler<TCommand, TResponse> : IRequestHandler<IdentifiedCommand<TCommand, TResponse>, TResponse>
        where TCommand : IRequest<TResponse>
    {
        private readonly IMediator _mediator;
        private readonly IRequestManager _requestManager;
        private readonly ILogger<IdentifiedCommandHandler<TCommand, TResponse>> _logger;
        public IdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<TCommand, TResponse>> logger)
        {
            _mediator = mediator;
            _requestManager = requestManager;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected virtual TResponse CreateResultForDuplicateRequest()
        {
            return default;
        }

        public async Task<TResponse> Handle(IdentifiedCommand<TCommand, TResponse> message, CancellationToken cancellationToken)
        {
            bool alreadyExists = await _requestManager.ExistAsync(message.Id);
            if (alreadyExists)
            {
                return CreateResultForDuplicateRequest();
            }

            await _requestManager.CreateRequestForCommandAsync<TCommand>(message.Id);

            try
            {
                TCommand command = message.Command;
                string commandName = command.GetGenericTypeName();
                string idProperty = string.Empty;
                string commandId = string.Empty;

                switch (command)
                {
                    case SetTranslationContentCommand setTranslationContentCommand:
                        idProperty = nameof(setTranslationContentCommand.TranslationId);
                        commandId = $"{setTranslationContentCommand.TranslationId}";
                        break;

                    default:
                        idProperty = "Id?";
                        commandId = "n/a";
                        break;
                }

                _logger.LogInformation(
                        "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                        commandName,
                        idProperty,
                        commandId,
                        command);

                TResponse result = await _mediator.Send(command, cancellationToken);

                _logger.LogInformation(
                    "----- Command result: {@Result} - {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                    result,
                    commandName,
                    idProperty,
                    commandId,
                    command);

                return result;
            }
            catch
            {
                return default;
            }
        }
    }
}
