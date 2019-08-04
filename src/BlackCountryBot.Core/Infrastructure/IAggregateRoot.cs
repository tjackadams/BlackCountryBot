using MediatR;
using System.Collections.Generic;

namespace BlackCountryBot.Core.Infrastructure
{
    public interface IAggregateRoot : IEntity
    {
        IEnumerable<INotification> GetDomainEvents();
        void ClearDomainEvents();
    }
}
