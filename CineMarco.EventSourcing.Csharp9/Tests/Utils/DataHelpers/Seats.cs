using System.Linq;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils.Fixtures
{
    public static class Seats
    {
        public static ReadOnlyList<SeatNumber> Number(params string[] seatNumbers) =>
            seatNumbers.Select(i => new SeatNumber(i)).ToReadOnlyList();
    }
}