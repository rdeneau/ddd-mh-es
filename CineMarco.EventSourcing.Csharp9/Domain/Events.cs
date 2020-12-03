using System;
using CineMarco.EventSourcing.Csharp9.Common;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public sealed record ScreeningInitialized(ScreeningId ScreeningId, NumberOfSeats Seats) : AuditedEvent;

    public sealed record SeatsReserved(ScreeningId ScreeningId, NumberOfSeats Seats) : AuditedEvent;

    public sealed record SeatsNotReserved(ScreeningId ScreeningId, NumberOfSeats Seats) : AuditedEvent;

    public record AuditedEvent(DateTimeOffset At) : IEvent
    {
        protected AuditedEvent() : this(DateTimeOffset.UtcNow) {}
    }
}