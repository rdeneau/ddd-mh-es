using System;
using CineMarco.EventSourcing.Csharp9.Common;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public sealed record SeatNumber(string Value) : IComparable<SeatNumber>
    {
        public Seat ToSeat() => new(this);

        public override string ToString() => Value;

        public int CompareTo(SeatNumber? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(Value, other.Value, StringComparison.Ordinal);
        }
    }

    public sealed record Seat(SeatNumber Number, DateTimeOffset? ReservationDate = null)
    {
        public bool HasReservationExpired(TimeSpan expirationDelay) =>
            ReservationDate <= ClockUtc.Now.Add(-expirationDelay);

        public bool IsReserved => ReservationDate.HasValue;

        public Seat Reserve(DateTimeOffset at) =>
            this with { ReservationDate = at };

        public Seat RemoveReservation() =>
            this with { ReservationDate = null };

        public override string ToString() =>
            $"Seat #{Number.Value}{ReservationInfo}";

        private string ReservationInfo =>
            ReservationDate.HasValue
                ? $", Reserved @ {ReservationDate.Value}"
                : "";
    }
}