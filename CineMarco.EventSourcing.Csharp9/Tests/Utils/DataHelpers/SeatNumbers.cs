using System;
using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Common.Collections;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.ReadSide.Models;
using ClientSeatReservationInfo = CineMarco.EventSourcing.Csharp9.Application.ClientSeatReservationInfo;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers
{
    public static class SeatNumbers
    {
        public static ReadOnlyList<SeatNumber> Seats(params string[] seatNumbers) =>
            seatNumbers.Select(i => new SeatNumber(i)).ToReadOnlyList();

        public static ReadOnlyList<ClientSeatReservationInfo> Reserved(this IEnumerable<SeatNumber> seats, DateTimeOffset at) =>
            seats.Select(seat => new ClientSeatReservationInfo(seat, at, ReservationStatus.Reserved)).ToReadOnlyList();
    }
}