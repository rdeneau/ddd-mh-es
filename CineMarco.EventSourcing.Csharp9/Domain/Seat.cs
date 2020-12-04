using System;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public sealed record SeatNumber(string Value)
    {
        public Seat ToSeat() => new(this);

        public override string ToString() => Value;
    }

    public sealed record Seat(SeatNumber Number, DateTimeOffset? ReservationDate = null)
    {
        public bool IsReserved => ReservationDate.HasValue;

        public Seat Reserve(DateTimeOffset? at = null)
        {
            if (IsReserved)
                throw new ApplicationException("Cannot reserve a seat twice");
            else
                return this with { ReservationDate = at ?? DateTimeOffset.UtcNow };
        }

        public override string ToString() => $"Seat #{Number.Value}{ReservationInfo}";

        private string ReservationInfo =>
            ReservationDate.HasValue
                ? $", Reserved @ {ReservationDate.Value}"
                : "";
    }
}