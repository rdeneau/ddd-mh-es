using System;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public sealed record ClientId : Id<ClientId>;

    public sealed record ScreeningId : Id<ScreeningId>;

    public abstract record Id<T>(Guid Value) where T: Id<T>, new()
    {
        public static T Generate() => new() { Value = Guid.NewGuid() };

        protected Id(): this(new Guid()) { }

        public override string ToString() => Value.ToString();
    }
}