using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Domain.Contracts;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public class ScreeningReservation
    {
        public const int ExpirationMinutes = 12;

        private readonly ScreeningReservationState _state;

        private readonly IEventBus _eventBus;

        public ScreeningReservation(ScreeningReservationState state, IEventBus eventBus)
        {
            _state    = state;
            _eventBus = eventBus;
        }

        public void CheckSeatsReservationExpiration(IReadOnlyList<SeatNumber> seats)
        {
            var seatsToFree = SeatsWithReservationExpired(seats).ToReadOnlyList();
            if (seatsToFree.Count > 0)
            {
                _eventBus.Publish(new SeatReservationHasExpired(_state.Id, seatsToFree));
            }
        }

        private IEnumerable<SeatNumber> SeatsWithReservationExpired(IEnumerable<SeatNumber> seats) =>
            from   seatNumber in seats
            let    seat = _state.Seat(seatNumber)
            where  seat?.HasReservationOlderThan(ExpirationMinutes) == true
            select seatNumber;

        public IScreeningReservationEvent ReserveSeats(IReadOnlyList<SeatNumber> seats) =>
            ReserveSeatsEvent(seats)
                .PublishedTo(_eventBus);

        private IScreeningReservationEvent ReserveSeatsEvent(IReadOnlyList<SeatNumber> seats)
        {
            if (IsTooClosedToScreeningTime())
                return new SeatsReservationFailed(_state.Id, seats, ReservationFailure.TooClosedToScreeningTime);

            var seatsToReserved = AvailableSeats().Intersect(seats).ToReadOnlyList();
            if (seatsToReserved.Count == seats.Count)
                return new SeatsAreReserved(_state.Id, seatsToReserved);

            if (seats.Except(AllSeats()).Any())
                return new SeatsReservationFailed(_state.Id, seats, ReservationFailure.SomeSeatsAreUnknown);

            else
                return new SeatsReservationFailed(_state.Id, seats);
        }

        private bool IsTooClosedToScreeningTime() =>
            ClockUtc.Now > _state.Date.AddMinutes(-15);

        public IScreeningReservationEvent ReserveSeatsInBulk(NumberOfSeats count) =>
            ReserveSeatsInBulkEvent(count)
                .PublishedTo(_eventBus);

        private IScreeningReservationEvent ReserveSeatsInBulkEvent(NumberOfSeats count)
        {
            var numberOfSeats = count.Value;
            var seatsToReserved = AvailableSeats()
                                  .Take(numberOfSeats)
                                  .ToReadOnlyList();

            if (seatsToReserved.Count < numberOfSeats)
                return new SeatsBulkReservationFailed(_state.Id, numberOfSeats);
            else
                return new SeatsAreReserved(_state.Id, seatsToReserved);
        }

        private IEnumerable<SeatNumber> AllSeats() =>
            _state.Seats
                  .Select(x => x.Number);

        private IEnumerable<SeatNumber> AvailableSeats() =>
            _state.Seats
                  .Where(x => !x.IsReserved)
                  .Select(x => x.Number);
    }
}