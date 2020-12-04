using System.Collections.Generic;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public class ScreeningState
    {
        public ScreeningId Id { get; set; } = ScreeningId.Undefined;

        private Dictionary<SeatNumber, Seat> SeatMap { get; } = new();

        public IEnumerable<Seat> Seats => SeatMap.Values;

        public ScreeningState(IEnumerable<IDomainEvent> history)
        {
            foreach (dynamic @event in history)
                Apply(@event); // ⚠ Dynamic dispatch
        }

        // Fallback "Apply" method, compulsory to secure the previous dynamic dispatch
        private void Apply(IDomainEvent _) { }

        private void Apply(ScreeningIsInitialized @event)
        {
            Id = @event.ScreeningId;
            foreach (var seatNumber in @event.Seats)
                SeatMap.Add(seatNumber, seatNumber.ToSeat());
        }

        private void Apply(SeatsAreReserved @event)
        {
            foreach (var seat in @event.Seats)
                SeatMap[seat] = SeatMap[seat].Reserve(@event.At);
        }
    }
}