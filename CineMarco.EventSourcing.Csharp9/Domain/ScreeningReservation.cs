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
                yield return new SeatsReservationHasExpired(clientId, _state.Id, seatsToFree).AppliedOn(_state);
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
                yield return new SeatsWereBooked(clientId, _state.Id, seats).At(date).AppliedOn(_state);
            else
                yield return new SeatsBookingHasFailed(clientId, _state.Id, seats).AppliedOn(_state);
        }

        public IEnumerable<IScreeningReservationEvent> Reserve(IReadOnlyList<SeatNumber> seats, ClientId clientId, DateTimeOffset? date)
        {
            var reservationFailure = ComputeReservationFailure(seats);
            if (reservationFailure == null)
                yield return new SeatsWereReserved(clientId, _state.Id, seats).At(date).AppliedOn(_state);
            else
                yield return new SeatsReservationHasFailed(clientId, _state.Id, seats, reservationFailure.Value).AppliedOn(_state);
        }

        public ReservationFailure? ComputeReservationFailure(IReadOnlyList<SeatNumber> seats)
        {
            if (IsTooClosedToScreeningTime())
                return ReservationFailure.TooClosedToScreeningTime;

            var seatsToReserved = AvailableSeats().Intersect(seats).ToReadOnlyList();
            if (seatsToReserved.Count == seats.Count)
                return null;

            if (seats.Except(AllSeats()).Any())
                return ReservationFailure.SomeSeatsAreUnknown;

            else
                return ReservationFailure.NotEnoughSeatsAvailable;
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
                yield return new SeatsWereReserved(clientId, _state.Id, seatsToReserved).AppliedOn(_state);
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