using System;
using System.Collections.Generic;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Common.Collections;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public class ScreeningReservationState : IStateFrom<ScreeningIsInitialized>,
                                             IStateFrom<SeatsAreReserved>,
                                             IStateFrom<SeatReservationHasExpired>
    {
        public ScreeningId Id { get; private set; } = ScreeningId.Undefined;

        public DateTimeOffset Date { get; private set; } = ClockUtc.Now;

        private Dictionary<SeatNumber, Seat> SeatMap { get; } = new();

        public IEnumerable<Seat> Seats => SeatMap.Values;

        public ScreeningReservationState(IEnumerable<IDomainEvent> history) =>
            this.ReconstructFrom(history);

        public void Apply(ScreeningIsInitialized @event)
        {
            (Id, Date, _) = @event;
            @event.Seats.ForEach(seatNumber =>
            {
                SeatMap.Add(seatNumber, seatNumber.ToSeat());
            });
        }

        public void Apply(SeatsAreReserved @event) =>
            @event.Seats.ForEach(seatNumber =>
            {
                SeatMap[seatNumber] = SeatMap[seatNumber].Reserve(@event.At);
            });

        public void Apply(SeatReservationHasExpired @event) =>
            @event.Seats.ForEach(seatNumber =>
            {
                SeatMap[seatNumber] = SeatMap[seatNumber].RemoveReservation();
            });

        public Seat? Seat(SeatNumber seatNumber) =>
            SeatMap.TryGetValue(seatNumber, out var seat)
                ? seat
                : null;
    }
}