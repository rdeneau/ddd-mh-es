using System;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public sealed record ScreeningId
    {
        public static readonly ScreeningId Undefined = new(new Guid());

        public static ScreeningId Generate() => new(Guid.NewGuid());

        public Guid Value { get; }

        private ScreeningId(Guid value)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}