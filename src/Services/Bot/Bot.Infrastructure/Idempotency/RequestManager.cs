using System;
using System.Threading.Tasks;
using BlackCountryBot.Core.Infrastructure;
using Bot.Domain.Exceptions;

namespace Bot.Infrastructure.Idempotency
{
    public class RequestManager : IRequestManager
    {
        private readonly BotContext _context;

        public RequestManager(BotContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<bool> ExistAsync(Guid id)
        {
            ClientRequest request = await _context.
                FindAsync<ClientRequest>(id);

            return request != null;
        }

        public async Task CreateRequestForCommandAsync<T>(Guid id)
        {
            bool exists = await ExistAsync(id);

            ClientRequest request = exists ?
                throw new BotDomainException($"Request with {id} already exists") :
                new ClientRequest()
                {
                    Id = id,
                    Name = typeof(T).Name,
                    Time = DateTime.UtcNow
                };

            _context.Add(request);

            await _context.SaveChangesAsync();
        }
    }
}
