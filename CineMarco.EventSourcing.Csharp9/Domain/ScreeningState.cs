using System.Collections.Generic;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public class ScreeningState
    {
        public ScreeningId ScreeningId { get; private set; } = ScreeningId.Undefined;

        public NumberOfSeats SeatsLeft { get; private set; } = NumberOfSeats.Zero;

        public ScreeningState(IEnumerable<Event> history)
        {
            foreach (dynamic @event in history)
                Apply(@event); // Dynamic dispatch
        }

        // For other event types, in order to have a fallback "Apply" method for the dynamic dispatch
        private void Apply(Event _) { }

        private void Apply(ScreeningInitialized @event)
        {
            (ScreeningId, SeatsLeft) = @event;
        }

        private void Apply(SeatsReserved @event)
        {
            // 💡 No need to check the `ScreeningId`. The event has happened. It's the source of truth.
            SeatsLeft -= @event.Seats;
        }
    }
}