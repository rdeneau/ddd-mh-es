using System;
using System.Collections.Generic;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public class ScreeningReservationState
    {
        public ScreeningId Id { get; private set; } = ScreeningId.Undefined;

        public DateTimeOffset Date { get; private set; }

        private Dictionary<SeatNumber, Seat> SeatMap { get; } = new();

        public IEnumerable<Seat> Seats => SeatMap.Values;

        public ScreeningReservationState(IEnumerable<IDomainEvent> history)
        {
            foreach (dynamic @event in history)
                Apply(@event); // ⚠ Dynamic dispatch
        }

        // Fallback "Apply" method, compulsory to secure the previous dynamic dispatch
        // ReSharper disable once UnusedParameter.Local
        private void Apply(IDomainEvent _) { }

        private void Apply(ScreeningIsInitialized @event)
        {
            (Id, Date, _) = @event;
            foreach (var seatNumber in @event.Seats)
            {
                SeatMap.Add(seatNumber, seatNumber.ToSeat());
            }
        }

        private void Apply(SeatsAreReserved @event)
        {
            foreach (var seat in @event.Seats)
            {
                SeatMap[seat] = SeatMap[seat].Reserve(@event.At);
            }
        }
    }
}