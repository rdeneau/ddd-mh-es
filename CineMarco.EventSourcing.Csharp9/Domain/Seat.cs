using System;
using CineMarco.EventSourcing.Csharp9.Common;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public interface ISeat
    {
        SeatNumber Number { get; }
    }

    public record AvailableSeat(SeatNumber Number) : ISeat
    {
        public ReservedSeat Reserve(DateTimeOffset at, ClientId @for) =>
            new(Number, at, @for);

        public override string ToString() =>
            $"Seat #{Number.Value}";
    }

    public record ReservedSeat(SeatNumber Number, DateTimeOffset ReservationDate, ClientId ClientId) : ISeat
    {
        public bool HasReservationExpired(TimeSpan expirationDelay) =>
            ReservationDate <= ClockUtc.Now.Add(-expirationDelay);

        public AvailableSeat RemoveReservation() =>
            new(Number);

        public override string ToString() =>
            $"Seat #{Number.Value} Reserved @ {ReservationDate} by {ClientId}";
    }
}