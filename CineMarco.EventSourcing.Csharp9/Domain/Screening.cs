using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Domain.Contracts;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public record Screening(ScreeningState State, IEventBus EventBus)
    {
        public void ReserveSeats(IReadOnlyList<SeatNumber> seats)
        {
            EventBus.Publish(ReserveSeatsEvent(seats));
        }

        private IDomainEvent ReserveSeatsEvent(IReadOnlyList<SeatNumber> seats)
        {
            var seatsToReserved = AvailableSeats().Intersect(seats).ToReadOnlyList();
            if (seatsToReserved.Count == seats.Count)
                return new SeatsAreReserved(State.Id, seatsToReserved);

            if (seats.Except(AllSeats()).Any())
                return new SeatsReservationFailed(State.Id, seats, ReservationFailure.SomeSeatsAreUnknown);

            else
                return new SeatsReservationFailed(State.Id, seats);
        }

        public void ReserveSeatsInBulk(NumberOfSeats count)
        {
            EventBus.Publish(ReserveSeatsInBulkEvent(count));
        }

        private IDomainEvent ReserveSeatsInBulkEvent(NumberOfSeats count)
        {
            var numberOfSeats = count.Value;
            var seatsToReserved = AvailableSeats()
                                  .Take(numberOfSeats)
                                  .ToReadOnlyList();

            if (seatsToReserved.Count < numberOfSeats)
                return new SeatsBulkReservationFailed(State.Id, numberOfSeats);
            else
                return new SeatsAreReserved(State.Id, seatsToReserved);
        }

        private IEnumerable<SeatNumber> AllSeats() =>
            State.Seats
                 .Select(x => x.Number);

        private IEnumerable<SeatNumber> AvailableSeats() =>
            State.Seats
                 .Where(x => !x.IsReserved)
                 .Select(x => x.Number);
    }
}