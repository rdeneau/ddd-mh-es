using System.Collections.Generic;
using System.Linq;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public class ScreeningState
    {
        private ScreeningId ScreeningId { get; set; } = ScreeningId.Undefined;

        public List<Seat> Seats { get; } = new();

        public ScreeningState(IEnumerable<IEvent> history)
        {
            foreach (dynamic @event in history)
                Apply(@event); // Dynamic dispatch
        }

        // Fallback "Apply" method, compulsory to secure the previous dynamic dispatch
        private void Apply(IEvent _) { }

        private void Apply(ScreeningInitialized @event)
        {
            var (screeningId, seats) = @event;
            Seats.AddRange(seats);
            ScreeningId = screeningId;
        }

        private void Apply(SeatsReserved @event)
        {
            // 💡 No need to check the `ScreeningId`. The event has happened. It's the source of truth.

            for (var i = 0; i < Seats.Count; i++)
            {
                var seat = @event.Seats.FirstOrDefault(x => x.Id == Seats[i].Id);
                if (seat != null)
                    Seats[i] = seat;
            }
        }
    }
}