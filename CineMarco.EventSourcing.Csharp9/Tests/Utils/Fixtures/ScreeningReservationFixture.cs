using System;
using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils.Fixtures
{
    public class ScreeningReservationFixture
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

        public SeatReservationHasExpired HasSeatsReservationExpired(params string[] seatNumbers) =>
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