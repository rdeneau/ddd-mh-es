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
        ScreeningId ScreeningId, IReadOnlyList<SeatNumber> Seats) : AuditedEvent;

    public sealed record SeatsAreReserved(ScreeningId ScreeningId, IReadOnlyList<SeatNumber> Seats) : AuditedEvent;

    public enum ReservationFailureReason { NotEnoughSeatsAvailable }

    public sealed record SeatsBulkReservationFailed(
        ScreeningId ScreeningId, int NumberOfSeats,
        ReservationFailureReason Reason = ReservationFailureReason.NotEnoughSeatsAvailable) : AuditedEvent;
}