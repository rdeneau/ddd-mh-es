using System;
using System.Collections.Generic;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    /// <summary>
    /// Naming convention: start with an imperative verb
    /// E.g. "ReserveSeats"
    /// </summary>
    public interface ICommand { }

    public sealed record CheckSeatsReservationExpiration(
        ScreeningId               ScreeningId,
        IReadOnlyList<SeatNumber> Seats
    ) : ICommand;

    public sealed record ReserveSeats(
        ClientId                  ClientId,
        ScreeningId               ScreeningId,
        IReadOnlyList<SeatNumber> Seats,
        DateTimeOffset?           ReservationDate = null
    ) : ICommand;

    public sealed record ReserveSeatsInBulk(
        ClientId      ClientId,
        ScreeningId   ScreeningId,
        NumberOfSeats Seats
    ) : ICommand;
}