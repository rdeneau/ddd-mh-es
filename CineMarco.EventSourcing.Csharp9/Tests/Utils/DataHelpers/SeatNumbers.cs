using System.Linq;
using CineMarco.EventSourcing.Csharp9.Common.Collections;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers
{
    public static class SeatNumbers
    {
        public static ReadOnlyList<SeatNumber> Seats(params string[] seatNumbers) =>
            seatNumbers.Select(i => new SeatNumber(i)).ToReadOnlyList();
    }
}