using System;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public class Entity : IEquatable<Entity>
    {
        public Guid Id { get; } = Guid.NewGuid();

        protected Entity() { }

        public bool Equals(Entity? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Entity) obj);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);

        public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);
    }
}