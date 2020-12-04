using System;
using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils
{
    public record BuildScreening(ScreeningId ScreeningId, ReadOnlyList<SeatNumber> Seats)
    {
        public static BuildScreening WithSeats(params string[] seatNumbers) =>
            new(ScreeningId.Generate(), SeatsWith(seatNumbers));

        private static ReadOnlyList<SeatNumber> SeatsWith(IEnumerable<string> seatNumbers) =>
            seatNumbers.Select(i => new SeatNumber(i)).ToReadOnlyList();

        public DateTimeOffset ScreeningDate { get; private set; }

        public BuildScreening PlannedTomorrow() =>
            this with { ScreeningDate = DateTimeOffset.UtcNow.AddDays(1) };

        public BuildScreening PlannedLater(int minutesBeforeScreening) =>
            this with { ScreeningDate = DateTimeOffset.UtcNow.AddMinutes(minutesBeforeScreening) };

        public ScreeningIsInitialized IsInitialized() =>
            new(ScreeningId, ScreeningDate, Seats);

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
    }
}