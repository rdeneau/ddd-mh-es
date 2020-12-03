using System;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public sealed class Seat : Entity
    {
        public SeatNumber Number { get; }

        public Seat(SeatNumber number)
        {
            Number = number;
        }

        public DateTimeOffset? ReservationDate { get; private set; }

        public bool IsReserved => ReservationDate.HasValue;

        public Seat Reserve()
        {
            if (IsReserved)
                throw new ApplicationException("Cannot reserve a seat twice");

            ReservationDate = DateTimeOffset.UtcNow;
            return this;
        }

        public override string ToString() => $"Seat #{Number.Value}{ReservationInfo}, Id = {Id}";

        private string ReservationInfo =>
            ReservationDate.HasValue
                ? $", Reserved @ {ReservationDate.Value}"
                : "";
    }
}