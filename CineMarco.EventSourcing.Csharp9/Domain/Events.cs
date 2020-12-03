using System;
using System.Collections.Generic;
using CineMarco.EventSourcing.Csharp9.Common;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public sealed record ScreeningInitialized(ScreeningId ScreeningId, IEnumerable<Seat> Seats) : AuditedEvent;

    public sealed record SeatsReserved(ScreeningId ScreeningId, ValueList<Seat> Seats) : AuditedEvent;

    public sealed record SeatsNotReserved(ScreeningId ScreeningId, NumberOfSeats Seats) : AuditedEvent;

    public record AuditedEvent(DateTimeOffset At) : IEvent
    {
        protected AuditedEvent() : this(DateTimeOffset.UtcNow) { }
    }
}