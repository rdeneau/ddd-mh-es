using System;
using System.Collections.Generic;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Common.Collections;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public class ScreeningReservationState : IAggregateState<ScreeningWasInitialized>,
                                             IAggregateState<SeatsWereBooked>,
                                             IAggregateState<SeatsWereReserved>,
                                             IAggregateState<SeatsReservationHasExpired>
    {
        public ScreeningId Id { get; private set; } = new();

        public DateTimeOffset Date { get; private set; } = ClockUtc.Now;

        private Dictionary<SeatNumber, ISeat> SeatMap { get; } = new();

        public IEnumerable<ISeat> Seats => SeatMap.Values;

        public ScreeningReservationState(IEnumerable<IDomainEvent> history) =>
            this.RestoreFrom(history);

        public void Apply(ScreeningWasInitialized @event)
        {
            (Id, Date, _) = @event;
            @event.Seats.ForEach(seatNumber =>
            {
                SeatMap.Add(seatNumber, seatNumber.ToAvailableSeat());
            });
        }

        public void Apply(SeatsWereBooked @event) =>
            @event.Seats.ForEach(seatNumber =>
            {
                var seat = (ReservedSeat) SeatMap[seatNumber];
                SeatMap[seatNumber] = seat.Book(@event.At, @event.ClientId);
            });

        public void Apply(SeatsWereReserved @event) =>
            @event.Seats.ForEach(seatNumber =>
            {
                var seat = (AvailableSeat) SeatMap[seatNumber];
                SeatMap[seatNumber] = seat.Reserve(@event.At, @event.ClientId);
            });

        public void Apply(SeatsReservationHasExpired @event) =>
            @event.Seats.ForEach(seatNumber =>
            {
                var seat = (ReservedSeat) SeatMap[seatNumber];
                SeatMap[seatNumber] = seat.RemoveReservation();
            });

        public ISeat? Seat(SeatNumber seatNumber) =>
            SeatMap.TryGetValue(seatNumber, out var seat)
                ? seat
                : null;
    }
}