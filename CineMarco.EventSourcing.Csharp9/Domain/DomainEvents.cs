using System;
using System.Collections.Generic;
using CineMarco.EventSourcing.Csharp9.Common;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public interface IDomainEvent { } // Marker interface

    public sealed record ScreeningInitialized(ScreeningId ScreeningId, IEnumerable<Seat> Seats) : AuditedEvent;

    public sealed record SeatsReserved(ScreeningId ScreeningId, ValueList<Seat> Seats) : AuditedEvent;

    public sealed record SeatsNotReserved(ScreeningId ScreeningId, NumberOfSeats Seats) : AuditedEvent;

    public record AuditedEvent(DateTimeOffset At) : IDomainEvent
    {
        protected AuditedEvent() : this(ClockUtc.Now) { }
    }
}