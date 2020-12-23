using System;
using System.Collections.Generic;
using CineMarco.EventSourcing.Csharp9.Common;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    /// <summary>
    /// Naming convention: start with verb in the past tense
    /// E.g. "SeatsHaveBeenReserved"
    /// </summary>
    public interface IDomainEvent { }

    public record AuditedEvent(DateTimeOffset At) : IDomainEvent
    {
        protected AuditedEvent() : this(ClockUtc.Now) { }
    }

    public static class AuditedEventExtension
    {
        public static TEvent At<TEvent>(this TEvent @this, DateTimeOffset? at) where TEvent : AuditedEvent =>
            at.HasValue ? @this with { At = at.Value } : @this;
    }

    public sealed record ScreeningWasInitialized(
        ScreeningId               ScreeningId,
        DateTimeOffset            ScreeningDate,
        IReadOnlyList<SeatNumber> Seats
    ) : AuditedEvent;

    public interface IScreeningReservationEvent : IDomainEvent { }

    public sealed record SeatsWereReserved(
        ClientId                  ClientId,
        ScreeningId               ScreeningId,
        IReadOnlyList<SeatNumber> Seats
    ) : AuditedEvent, IScreeningReservationEvent;

    public enum ReservationFailure
    {
        NotEnoughSeatsAvailable  = 1,
        SomeSeatsAreUnknown      = 2,
        TooClosedToScreeningTime = 3,
    }

    public sealed record SeatsReservationHasFailed(
        ClientId                  ClientId,
        ScreeningId               ScreeningId,
        IReadOnlyList<SeatNumber> Seats,
        ReservationFailure        Reason = ReservationFailure.NotEnoughSeatsAvailable
    ) : AuditedEvent, IScreeningReservationEvent;

    public sealed record SeatsBulkReservationHasFailed(
        ClientId           ClientId,
        ScreeningId        ScreeningId,
        int                NumberOfSeats,
        ReservationFailure Reason = ReservationFailure.NotEnoughSeatsAvailable
    ) : AuditedEvent, IScreeningReservationEvent;

    public sealed record SeatsReservationHasExpired(
        ClientId                  ClientId,
        ScreeningId               ScreeningId,
        IReadOnlyList<SeatNumber> Seats
    ) : AuditedEvent, IScreeningReservationEvent;

    public sealed record SeatsWereBooked(
        ClientId                  ClientId,
        ScreeningId               ScreeningId,
        IReadOnlyList<SeatNumber> Seats
    ) : AuditedEvent, IScreeningReservationEvent;

    public sealed record SeatsBookingHasFailed(
        ClientId                  ClientId,
        ScreeningId               ScreeningId,
        IReadOnlyList<SeatNumber> Seats
    ) : AuditedEvent, IScreeningReservationEvent;
}