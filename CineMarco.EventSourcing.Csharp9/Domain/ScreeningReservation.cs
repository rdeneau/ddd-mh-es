using System;
using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Common.Collections;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public class ScreeningReservation
    {
        public static readonly TimeSpan ClosingDelay    = new(hours: 0, minutes: 15, seconds: 0);
        public static readonly TimeSpan ExpirationDelay = new(hours: 0, minutes: 12, seconds: 0);

        private readonly ScreeningReservationState _state;

        public ScreeningReservation(ScreeningReservationState state)
        {
            _state = state;
        }

        public IEnumerable<IScreeningReservationEvent> CheckSeatsReservationExpiration(ClientId clientId, IReadOnlyList<SeatNumber> seats)
        {
            var seatsToFree = SeatsWithReservationExpired(seats).ToReadOnlyList();
            if (seatsToFree.Count > 0)
            {
                yield return new SeatsReservationHasExpired(clientId, _state.Id, seatsToFree);
            }
        }

        private IEnumerable<SeatNumber> SeatsWithReservationExpired(IEnumerable<SeatNumber> seats) =>
            from   seatNumber in seats
            let    seat = _state.Seat(seatNumber) as ReservedSeat
            where  seat?.HasReservationExpired(ExpirationDelay) == true
            select seatNumber;

        public IEnumerable<IScreeningReservationEvent> Book(IReadOnlyList<SeatNumber> seats, ClientId clientId, DateTimeOffset? date)
        {
            var seatsToBooked = ReservedSeats().Intersect(seats).ToReadOnlyList();
            if (seatsToBooked.Count == seats.Count)
                yield return new SeatsWereBooked(clientId, _state.Id, seats).At(date);
            else
                yield return new SeatsBookingHasFailed(clientId, _state.Id, seats);
        }

        public IEnumerable<IScreeningReservationEvent> Reserve(IReadOnlyList<SeatNumber> seats, ClientId clientId, DateTimeOffset? date)
        {
            if (IsTooClosedToScreeningTime())
            {
                yield return new SeatsReservationHasFailed(clientId, _state.Id, seats, ReservationFailure.TooClosedToScreeningTime);
                yield break;
            }

            var seatsToReserved = AvailableSeats().Intersect(seats).ToReadOnlyList();
            if (seatsToReserved.Count == seats.Count)
                yield return new SeatsWereReserved(clientId, _state.Id, seatsToReserved).At(date);
            else if (seats.Except(AllSeats()).Any())
                yield return new SeatsReservationHasFailed(clientId, _state.Id, seats, ReservationFailure.SomeSeatsAreUnknown);
            else
                yield return new SeatsReservationHasFailed(clientId, _state.Id, seats);
        }

        private bool IsTooClosedToScreeningTime() =>
            ClockUtc.Now > _state.Date.Add(-ClosingDelay);

        public IEnumerable<IScreeningReservationEvent> ReserveSeatsInBulkEvent(NumberOfSeats count, ClientId clientId)
        {
            var numberOfSeats = count.Value;
            var seatsToReserved = AvailableSeats()
                                  .Take(numberOfSeats)
                                  .ToReadOnlyList();

            if (seatsToReserved.Count < numberOfSeats)
                yield return new SeatsBulkReservationHasFailed(clientId, _state.Id, numberOfSeats);
            else
                yield return new SeatsWereReserved(clientId, _state.Id, seatsToReserved);
        }

        private IEnumerable<SeatNumber> AllSeats()       => SeatsBeing<ISeat>();
        private IEnumerable<SeatNumber> AvailableSeats() => SeatsBeing<AvailableSeat>();
        private IEnumerable<SeatNumber> ReservedSeats()  => SeatsBeing<ReservedSeat>();

        private IEnumerable<SeatNumber> SeatsBeing<T>() where T: ISeat =>
            _state.Seats
                  .Where(x => x is T)
                  .Select(x => x.Number);
    }
}