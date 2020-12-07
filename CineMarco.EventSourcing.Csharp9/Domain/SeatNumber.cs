using System;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public sealed record SeatNumber(string Value) : IComparable<SeatNumber>
    {
        public AvailableSeat ToAvailableSeat() => new(this);

        public override string ToString() => Value;

        public int CompareTo(SeatNumber? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(Value, other.Value, StringComparison.Ordinal);
        }
    }
}