using System;
using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using BlackCountryBot.BuildingBlocks.EventBus.Extensions;

namespace Bot.API.Application.Behaviours
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly BotContext _dbContext;
        public TransactionBehaviour(
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger,
            BotContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            string typeName = request.GetGenericTypeName();

            try
            {
                if (_dbContext.HasActiveTransaction)
                {
                    return await next();
                }

                Microsoft.EntityFrameworkCore.Storage.IExecutionStrategy strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await _dbContext.BeingTransactionAsync())
                    {
                        using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                        {
                            _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                            response = await next();

                            _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                            await _dbContext.CommitTransactionAsync(transaction);

                            transactionId = transaction.TransactionId;
                        }

                        // publish events
                    }
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}
