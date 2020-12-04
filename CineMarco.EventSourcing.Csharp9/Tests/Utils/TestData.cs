using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils
{
    public record ScreeningData(ScreeningId ScreeningId, ReadOnlyList<SeatNumber> Seats)
    {
        public static ScreeningData WithSeats(params string[] seatNumbers) =>
            new( ScreeningId.Generate(), SeatsOf(seatNumbers));

        private static ReadOnlyList<SeatNumber> SeatsOf(IEnumerable<string> seatNumbers) =>
            seatNumbers.Select(n => new SeatNumber(n)).ToReadOnlyList();

        public ScreeningIsInitialized IsInitialized() =>
            new(ScreeningId, Seats);

        public SeatsAreReserved HasSeatsReserved(params string[] seatNumbers) =>
            new(ScreeningId, SeatsOf(seatNumbers));

        public SeatsBulkReservationFailed HasFailedToReserveSeats(int numberOfSeats) =>
            new(ScreeningId, numberOfSeats);

        public ICommand ReserveSeatsInBulk(int numberOfSeats) =>
            new ReserveSeatsInBulk(ScreeningId, new NumberOfSeats(numberOfSeats));
    }
}