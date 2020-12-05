using System;
using System.Collections.Generic;
using CineMarco.EventSourcing.Csharp9.Common;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public interface IDomainEvent { } // Marker interface

    public record AuditedEvent(DateTimeOffset At) : IDomainEvent
    {
        protected AuditedEvent() : this(ClockUtc.Now) { }
    }

    public sealed record ScreeningIsInitialized(
        ScreeningId ScreeningId,
        DateTimeOffset ScreeningDate,
        IReadOnlyList<SeatNumber> Seats) : AuditedEvent;

    public sealed record SeatsAreReserved(
        ScreeningId ScreeningId,
        IReadOnlyList<SeatNumber> Seats) : AuditedEvent;

    public sealed record SeatReservationHasExpired(
        ScreeningId ScreeningId,
        IReadOnlyList<SeatNumber> Seats) : AuditedEvent;

    public enum ReservationFailure
    {
        NotEnoughSeatsAvailable  = 1,
        SomeSeatsAreUnknown      = 2,
        TooClosedToScreeningTime = 3,
    }

    public sealed record SeatsReservationFailed(
        ScreeningId ScreeningId,
        IReadOnlyList<SeatNumber> Seats,
        ReservationFailure Reason = ReservationFailure.NotEnoughSeatsAvailable) : AuditedEvent;

    public sealed record SeatsBulkReservationFailed(
        ScreeningId ScreeningId,
        int NumberOfSeats,
        ReservationFailure Reason = ReservationFailure.NotEnoughSeatsAvailable) : AuditedEvent;
}