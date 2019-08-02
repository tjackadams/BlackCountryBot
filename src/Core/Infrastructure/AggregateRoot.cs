using MediatR;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.Infrastructure
{
    public abstract class AggregateRoot : Entity, IAggregateRoot
    {
        private readonly ICollection<INotification> _domainEvents = new Collection<INotification>();

        protected virtual void AddDomainEvent(INotification @event)
        {
            _domainEvents.Add(@event);
        }

        public virtual IEnumerable<INotification> GetDomainEvents()
        {
            return _domainEvents;
        }

        public virtual void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
