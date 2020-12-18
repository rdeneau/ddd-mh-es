using System;
using System.Collections.Generic;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    /// <summary>
    /// Naming convention: start with an imperative verb
    /// E.g. "ReserveSeats"
    /// </summary>
    public interface ICommand { }

    public record AuditedCommand(DateTimeOffset At) : ICommand
    {
        protected AuditedCommand() : this(ClockUtc.Now) { }
    }

    public sealed record CheckSeatsReservationExpiration(
        ScreeningId               ScreeningId,
        IReadOnlyList<SeatNumber> Seats
    ) : AuditedCommand;

    public sealed record ReserveSeats(
        ClientId                  ClientId,
        ScreeningId               ScreeningId,
        IReadOnlyList<SeatNumber> Seats
    ) : AuditedCommand;

    public sealed record ReserveSeatsInBulk(
        ClientId      ClientId,
        ScreeningId   ScreeningId,
        NumberOfSeats Seats
    ) : AuditedCommand;
}