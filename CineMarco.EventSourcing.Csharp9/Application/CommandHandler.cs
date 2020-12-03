using System;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    // Global command handler, for all commands
    public record CommandHandler(IEventStore EventStore, IEventBus EventBus)
    {
        public void Handle(ICommand cmd)
        {
            switch (cmd) {
                case ReserveSeats command:
                    // Q: is it worthy to filter the events?
                    var screeningHistory = EventStore.Search(@event => @event switch {
                        ScreeningInitialized x => x.ScreeningId == command.ScreeningId,
                        SeatsReserved        x => x.ScreeningId == command.ScreeningId,
                        _ => false
                    });
                    // 💡 SRP: state reconstitution in its own class (with the apply methods), separated from behaviors (the real aggregate class that never updates its state, just publishes events)
                    var screeningState = new ScreeningState(screeningHistory);
                    var screening      = new Screening(screeningState, EventBus);
                    screening.ReserveSeats(command.ScreeningId, command.Seats);
                    break;

                default:
                    throw new ArgumentException($"Not supported command {cmd.GetType().FullName}", nameof(cmd));
            }
        }
    }
}