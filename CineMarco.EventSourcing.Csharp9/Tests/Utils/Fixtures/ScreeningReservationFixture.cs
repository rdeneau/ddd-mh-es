using System;
using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Common.Collections;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils.Fixtures
{
    public class ScreeningReservationFixture
    {
        private ClientId Guest { get; } = ClientId.Generate();

        private ScreeningId ScreeningId { get; } = ScreeningId.Generate();

        public ScreeningAvailableSeats AvailableSeats() =>
            new(ScreeningId);

        public ScreeningAvailableSeatsResponse AvailableSeatsResponse(params string[] seatNumbers) =>
            new(ScreeningId, SeatsWith(seatNumbers));

        public ScreeningIsInitialized IsInitialized(ReadOnlyList<SeatNumber> seatNumbers, DateTimeOffset? screeningDate = null) =>
            new(ScreeningId, screeningDate ?? Occurring.Tomorrow, seatNumbers);

        public SeatsAreReserved HasSeatsReserved(params string[] seatNumbers) =>
            HasSeatsReserved(Guest, seatNumbers);

        public SeatsAreReserved HasSeatsReserved(ClientId clientId, params string[] seatNumbers) =>
            new(clientId, ScreeningId, SeatsWith(seatNumbers));

        public SeatReservationHasExpired HasSeatsReservationExpired(params string[] seatNumbers) =>
            new(ScreeningId, SeatsWith(seatNumbers));

        public SeatsReservationFailed HasFailedToReserveSeats(params string[] seatNumbers) =>
            new(Guest, ScreeningId, SeatsWith(seatNumbers));

        public SeatsReservationFailed HasFailedToReserveSeatsTooClosedToScreeningTime(params string[] seatNumbers) =>
            new(Guest, ScreeningId, SeatsWith(seatNumbers), ReservationFailure.TooClosedToScreeningTime);

        public SeatsReservationFailed HasFailedToReserveSeatsUnknown(params string[] seatNumbers) =>
            new(Guest, ScreeningId, SeatsWith(seatNumbers), ReservationFailure.SomeSeatsAreUnknown);

        public SeatsBulkReservationFailed HasFailedToBulkReserveSeats(int numberOfSeats) =>
            new(Guest, ScreeningId, numberOfSeats);

        public ICommand CheckSeatsReservationExpiration(params string[] seatNumbers) =>
            new CheckSeatsReservationExpiration(ScreeningId, SeatsWith(seatNumbers));

        public ICommand ReserveSeats(params string[] seatNumbers) =>
            ReserveSeats(Guest, seatNumbers);

        public ICommand ReserveSeats(ClientId clientId, params string[] seatNumbers) =>
            new ReserveSeats(clientId, ScreeningId, SeatsWith(seatNumbers));

        public ICommand ReserveSeatsInBulk(int numberOfSeats) =>
            new ReserveSeatsInBulk(Guest, ScreeningId, new NumberOfSeats(numberOfSeats));

        private static ReadOnlyList<SeatNumber> SeatsWith(string[] seatNumbers) =>
            Seats.Number(seatNumbers);
    }
}