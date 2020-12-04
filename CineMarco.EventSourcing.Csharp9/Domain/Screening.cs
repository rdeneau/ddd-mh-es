using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Domain.Contracts;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public record Screening(ScreeningState State, IEventBus EventBus)
    {
        public void ReserveSeats(ScreeningId screeningId, NumberOfSeats count)
        {
            var numberOfSeats = count.Value;

            var seatsToReserved = AvailableSeats()
                                  .Take(numberOfSeats)
                                  .ToList();

            if (seatsToReserved.Count < numberOfSeats)
                EventBus.Publish(new SeatsBulkReservationFailed(screeningId, numberOfSeats));
            else
                EventBus.Publish(new SeatsAreReserved(screeningId, seatsToReserved.ToReadOnlyList()));
        }

        private IEnumerable<SeatNumber> AvailableSeats() =>
            State.Seats.Values
                 .Where(x => !x.IsReserved)
                 .Select(x => x.Number);
    }
}