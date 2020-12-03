using System;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public sealed record NumberOfSeats
    {
        public int Value { get; }

        public NumberOfSeats(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Expecting >= 0");

            Value = value;
        }
    }
}