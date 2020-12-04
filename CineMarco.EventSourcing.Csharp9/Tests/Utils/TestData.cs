using System;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils
{
    public static class Planned
    {
        public static DateTimeOffset Later(int minutes) =>
            DateTimeOffset.UtcNow.AddMinutes(minutes);

        public static DateTimeOffset Tomorrow =>
            DateTimeOffset.UtcNow.AddDays(1);
    }

    public static class Seats
    {
        public static ReadOnlyList<SeatNumber> Number(params string[] seatNumbers) =>
            seatNumbers.Select(i => new SeatNumber(i)).ToReadOnlyList();
    }

    public class ScreeningData
    {
        public ScreeningId ScreeningId { get; } = ScreeningId.Generate();

        public DateTimeOffset ScreeningDate { get; private set; }

        public ReadOnlyList<SeatNumber> SeatNumbers { get; private set; } = new();

        public ScreeningIsInitialized IsInitialized(DateTimeOffset screeningDate, ReadOnlyList<SeatNumber> seatNumbers) =>
            new(
                ScreeningId,
                ScreeningDate = screeningDate,
                SeatNumbers   = seatNumbers);

        public SeatsAreReserved HasSeatsReserved(params string[] seatNumbers) =>
            new(ScreeningId, SeatsWith(seatNumbers));

        public SeatsReservationFailed HasFailedToReserveSeats(params string[] seatNumbers) =>
            new(ScreeningId, SeatsWith(seatNumbers));

        public SeatsReservationFailed HasFailedToReserveSeatsTooClosedToScreeningTime(params string[] seatNumbers) =>
            new(ScreeningId, SeatsWith(seatNumbers), ReservationFailure.TooClosedToScreeningTime);

        public SeatsReservationFailed HasFailedToReserveSeatsUnknown(params string[] seatNumbers) =>
            new(ScreeningId, SeatsWith(seatNumbers), ReservationFailure.SomeSeatsAreUnknown);

        public SeatsBulkReservationFailed HasFailedToBulkReserveSeats(int numberOfSeats) =>
            new(ScreeningId, numberOfSeats);

        public ICommand ReserveSeats(params string[] seatNumbers) =>
            new ReserveSeats(ScreeningId, SeatsWith(seatNumbers));

        public ICommand ReserveSeatsInBulk(int numberOfSeats) =>
            new ReserveSeatsInBulk(ScreeningId, new NumberOfSeats(numberOfSeats));

        private static ReadOnlyList<SeatNumber> SeatsWith(string[] seatNumbers) =>
            Seats.Number(seatNumbers);
    }
}