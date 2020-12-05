using System.Collections.Generic;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    /// <summary>
    /// Naming convention: start with an imperative verb
    /// E.g. "ReserveSeats"
    /// </summary>
    public interface ICommand : IMarkerInterface { }

    public sealed record CheckSeatsReservationExpiration(ScreeningId ScreeningId, IReadOnlyList<SeatNumber> Seats) : ICommand;

    public sealed record ReserveSeats(ScreeningId ScreeningId, IReadOnlyList<SeatNumber> Seats) : ICommand;

    public sealed record ReserveSeatsInBulk(ScreeningId ScreeningId, NumberOfSeats Seats) : ICommand;
}