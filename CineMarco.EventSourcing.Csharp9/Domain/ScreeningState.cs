using System.Collections.Generic;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public class ScreeningState
    {
        private ScreeningId ScreeningId { get; set; } = ScreeningId.Undefined;

        public Dictionary<SeatNumber, Seat> Seats { get; } = new();

        public ScreeningState(IEnumerable<IDomainEvent> history)
        {
            foreach (dynamic @event in history)
                Apply(@event); // ⚠ Dynamic dispatch
        }

        // Fallback "Apply" method, compulsory to secure the previous dynamic dispatch
        private void Apply(IDomainEvent _) { }

        private void Apply(ScreeningIsInitialized @event)
        {
            ScreeningId = @event.ScreeningId;
            foreach (var seatNumber in @event.Seats)
                Seats.Add(seatNumber, seatNumber.ToSeat());
        }

        private void Apply(SeatsAreReserved @event)
        {
            foreach (var seat in @event.Seats)
                Seats[seat] = Seats[seat].Reserve(@event.At);
        }
    }
}