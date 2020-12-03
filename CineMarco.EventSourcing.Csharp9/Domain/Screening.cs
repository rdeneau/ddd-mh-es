using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Common;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public record Screening(ScreeningState State, IEventBus EventBus)
    {
        public void ReserveSeats(ScreeningId screeningId, NumberOfSeats count)
        {
            var seatsToReserved = AvailableSeats()
                                  .Take(count.Value)
                                  .ToList();

            if (seatsToReserved.Count < count.Value)
                EventBus.Publish(new SeatsNotReserved(screeningId, count));
            else
                EventBus.Publish(new SeatsReserved(screeningId, seatsToReserved.Select(x => x.Reserve()).ToValueList()));
        }

        private IEnumerable<Seat> AvailableSeats() =>
            State.Seats.Where(x => !x.IsReserved);
    }
}