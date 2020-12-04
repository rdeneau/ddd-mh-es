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
            Handle((dynamic) command); // ⚠ Dynamic dispatch
        }

        private void Handle(object command)
        {
            throw new ArgumentException($"Not supported command {command.GetType().FullName}", nameof(command));
        }

        private void Handle(ReserveSeats command)
        {
            var screening = ScreeningById(command.ScreeningId);
            screening.ReserveSeats(command.Seats);
        }

        private void Handle(ReserveSeatsInBulk command)
        {
            var screening = ScreeningById(command.ScreeningId);
            screening.ReserveSeatsInBulk(command.Seats);
        }

        private Screening ScreeningById(ScreeningId screeningId)
        {
            var history = EventStore.Search(@by: $"ScreeningId = {screeningId}");
            var state   = new ScreeningState(history);
            return new Screening(state, EventBus);
        }
    }
}