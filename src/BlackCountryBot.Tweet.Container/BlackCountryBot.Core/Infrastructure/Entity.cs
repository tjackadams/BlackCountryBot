using System;
using System.Collections.Generic;

namespace BlackCountryBot.Core.Infrastructure
{
    public abstract class Entity : IEntity, IEquatable<Entity>
    {
        protected Entity()
        {

        }

        public virtual int Id { get; set; }
        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        public static bool operator ==(Entity left, Entity right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }

            return left.Equals(right);
        }
        public bool Equals(Entity other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            if (other.IsTransient() || IsTransient())
            {
                return false;
            }

            return EqualityComparer<int>.Default.Equals(other.Id, Id);
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                return Id.GetHashCode() ^ 31;
            }

            return base.GetHashCode();
        }

        public bool IsTransient()
        {
            return Id == default;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            var item = (Entity)obj;

            if (item.IsTransient() || IsTransient())
            {
                return false;
            }

            return EqualityComparer<int>.Default.Equals(item.Id, Id);
        }
    }
}
