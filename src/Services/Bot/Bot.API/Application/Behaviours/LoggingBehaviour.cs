﻿using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.BuildingBlocks.EventBus.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bot.API.Application.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogInformation("----- Handling command {CommandName} ({@Command})", request.GetGenericTypeName(), request);
            TResponse response = await next();
            _logger.LogInformation("----- Command {CommandName} handled - response {@Response}", request.GetGenericTypeName(), response);

            return response;
        }
    }
}
