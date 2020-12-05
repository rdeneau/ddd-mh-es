using System;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Domain.Contracts;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    // Global command handler, for all commands
    public record CommandHandler(IEventStore EventStore, IEventBus EventBus)
    {
        public void Handle(ICommand command)
        {
            HandleCore((dynamic) command); // ⚠ Dynamic dispatch
        }

        private void HandleCore(object command)
        {
            throw new ArgumentException($"Not supported command {command.GetType().FullName}", nameof(command));
        }

        private void HandleCore(ReserveSeats command)
        {
            var screeningReservation = ScreeningReservationById(command.ScreeningId);
            screeningReservation.ReserveSeats(command.Seats);
        }

        private void HandleCore(ReserveSeatsInBulk command)
        {
            var screeningReservation = ScreeningReservationById(command.ScreeningId);
            screeningReservation.ReserveSeatsInBulk(command.Seats);
        }

        private ScreeningReservation ScreeningReservationById(ScreeningId screeningId)
        {
            var history = EventStore.Search(@by: $"ScreeningId = {screeningId}");
            var state   = new ScreeningReservationState(history);
            return new(state, EventBus);
        }
    }
}