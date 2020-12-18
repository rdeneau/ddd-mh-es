using System;
using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Common.Collections;
using CineMarco.EventSourcing.Csharp9.Domain.Contracts;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public class ScreeningReservation
    {
        public static readonly TimeSpan ClosingDelay    = new(hours: 0, minutes: 15, seconds: 0);
        public static readonly TimeSpan ExpirationDelay = new(hours: 0, minutes: 12, seconds: 0);

        private readonly ScreeningReservationState _state;

        private readonly IEventBus _eventBus;

        public ScreeningReservation(ScreeningReservationState state, IEventBus eventBus)
        {
            _state    = state;
            _eventBus = eventBus;
        }

        public void CheckSeatsReservationExpiration(ClientId clientId, IReadOnlyList<SeatNumber> seats)
        {
            var seatsToFree = SeatsWithReservationExpired(seats).ToReadOnlyList();
            if (seatsToFree.Count > 0)
            {
                _eventBus.Publish(new SeatReservationHasExpired(clientId, _state.Id, seatsToFree));
            }
        }

        private IEnumerable<SeatNumber> SeatsWithReservationExpired(IEnumerable<SeatNumber> seats) =>
            from   seatNumber in seats
            let    seat = _state.Seat(seatNumber) as ReservedSeat
            where  seat?.HasReservationExpired(ExpirationDelay) == true
            select seatNumber;

        public IScreeningReservationEvent Reserve(IReadOnlyList<SeatNumber> seats, ClientId @for, DateTimeOffset? at) =>
            ReserveEvent(seats, @for, at)
                .PublishedTo(_eventBus);

        private IScreeningReservationEvent ReserveEvent(IReadOnlyList<SeatNumber> seats, ClientId clientId, DateTimeOffset? at)
        {
            if (IsTooClosedToScreeningTime())
                return new SeatsReservationFailed(clientId, _state.Id, seats, ReservationFailure.TooClosedToScreeningTime);

            var seatsToReserved = AvailableSeats().Intersect(seats).ToReadOnlyList();
            if (seatsToReserved.Count == seats.Count)
                return new SeatsAreReserved(clientId, _state.Id, seatsToReserved)
                    .With(x => at.HasValue ? x with { At = at.Value } : x);

            if (seats.Except(AllSeats()).Any())
                return new SeatsReservationFailed(clientId, _state.Id, seats, ReservationFailure.SomeSeatsAreUnknown);

            else
                return new SeatsReservationFailed(clientId, _state.Id, seats);
        }

        private bool IsTooClosedToScreeningTime() =>
            ClockUtc.Now > _state.Date.Add(-ClosingDelay);

        public IScreeningReservationEvent ReserveSeatsInBulk(NumberOfSeats count, ClientId clientId) =>
            ReserveSeatsInBulkEvent(count, clientId)
                .PublishedTo(_eventBus);

        private IScreeningReservationEvent ReserveSeatsInBulkEvent(NumberOfSeats count, ClientId clientId)
        {
            var numberOfSeats = count.Value;
            var seatsToReserved = AvailableSeats()
                                  .Take(numberOfSeats)
                                  .ToReadOnlyList();

            if (seatsToReserved.Count < numberOfSeats)
                return new SeatsBulkReservationFailed(clientId, _state.Id, numberOfSeats);
            else
                return new SeatsAreReserved(clientId, _state.Id, seatsToReserved);
        }

        private IEnumerable<SeatNumber> AllSeats() =>
            _state.Seats
                  .Select(x => x.Number);

        private IEnumerable<SeatNumber> AvailableSeats() =>
            _state.Seats
                  .Where(x => x is AvailableSeat)
                  .Select(x => x.Number);
    }
}