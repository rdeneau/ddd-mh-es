using System;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Domain.Contracts;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    /// <summary>
    /// Global command handler, for all commands
    /// </summary>
    public class CommandHandler
    {
        private readonly IEventStore       _eventStore;
        private readonly IEventBus         _eventBus;
        private readonly ICommandScheduler _commandScheduler;

        public CommandHandler(IEventStore eventStore, IEventBus eventBus, ICommandScheduler commandScheduler)
        {
            _eventStore       = eventStore;
            _eventBus         = eventBus;
            _commandScheduler = commandScheduler;
        }

        public void Handle(ICommand command)
        {
            HandleCore((dynamic) command); // ⚠ Dynamic dispatch
        }

        private void HandleCore(object command)
        {
            throw new ArgumentException($"Not supported command {command.GetType().FullName}", nameof(command));
        }

        private void HandleCore(BookSeats command)
        {
            var screeningReservation = ScreeningReservationById(command.ScreeningId);
            var reservationEvent     = screeningReservation.Book(command.Seats, command.ClientId, command.At);
            ScheduleCheckSeatsReservationExpiration(reservationEvent);
        }

        private void HandleCore(CheckSeatsReservationExpiration command)
        {
            var screeningReservation = ScreeningReservationById(command.ScreeningId);
            screeningReservation.CheckSeatsReservationExpiration(command.ClientId, command.Seats);
        }

        private void HandleCore(ReserveSeats command)
        {
            var screeningReservation = ScreeningReservationById(command.ScreeningId);
            var reservationEvent     = screeningReservation.Reserve(command.Seats, command.ClientId, command.At);
            ScheduleCheckSeatsReservationExpiration(reservationEvent);
        }

        private void HandleCore(ReserveSeatsInBulk command)
        {
            var screeningReservation = ScreeningReservationById(command.ScreeningId);
            var reservationEvent     = screeningReservation.ReserveSeatsInBulk(command.Seats, command.ClientId);
            ScheduleCheckSeatsReservationExpiration(reservationEvent);
        }

        private void ScheduleCheckSeatsReservationExpiration(IScreeningReservationEvent reservationEvent)
        {
            if (reservationEvent is SeatsAreReserved seatsAreReserved)
            {
                var command = new CheckSeatsReservationExpiration(
                                  seatsAreReserved.ClientId,
                                  seatsAreReserved.ScreeningId,
                                  seatsAreReserved.Seats) { At = seatsAreReserved.At};
                _commandScheduler.Schedule(command, after: ScreeningReservation.ExpirationDelay);
            }
        }

        private ScreeningReservation ScreeningReservationById(ScreeningId screeningId)
        {
            var history = _eventStore.Search(@by: $"ScreeningId = {screeningId}");
            var state   = new ScreeningReservationState(history);
            return new(state, _eventBus);
        }
    }
}