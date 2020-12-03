using System;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public sealed record ScreeningId
    {
        public Guid Value { get; }

        private ScreeningId(Guid value)
        {
            Value = value;
        }

        public static readonly ScreeningId Undefined = new(new Guid());

        public static ScreeningId Generate() => new(Guid.NewGuid());
    }
}