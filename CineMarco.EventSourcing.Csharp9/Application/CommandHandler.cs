using System;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Domain.Contracts;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    /// <summary>
    /// Global command handler, for all commands
    /// </summary>
    public class CommandHandler
    {
        private readonly IEventStore _eventStore;
        private readonly IEventBus   _eventBus;
        private readonly ICommandBus _commandBus;

        public CommandHandler(IEventStore eventStore, IEventBus eventBus, ICommandBus commandBus)
        {
            _eventStore = eventStore;
            _eventBus   = eventBus;
            _commandBus = commandBus;
        }

        public void Handle(ICommand command)
        {
            HandleCore((dynamic) command); // ⚠ Dynamic dispatch
        }

        private void HandleCore(object command)
        {
            throw new ArgumentException($"Not supported command {command.GetType().FullName}", nameof(command));
        }

        private void HandleCore(CheckSeatsReservationExpiration command)
        {
            var screeningReservation = ScreeningReservationById(command.ScreeningId);
            screeningReservation.CheckSeatsReservationExpiration(command.Seats);
        }

        private void HandleCore(ReserveSeats command)
        {
            var screeningReservation = ScreeningReservationById(command.ScreeningId);
            var reservationEvent     = screeningReservation.ReserveSeats(command.Seats);
            ScheduleCheckSeatsReservationExpiration(reservationEvent);
        }

        private void HandleCore(ReserveSeatsInBulk command)
        {
            var screeningReservation = ScreeningReservationById(command.ScreeningId);
            var reservationEvent     = screeningReservation.ReserveSeatsInBulk(command.Seats);
            ScheduleCheckSeatsReservationExpiration(reservationEvent);
        }

        private void ScheduleCheckSeatsReservationExpiration(IScreeningReservationEvent reservationEvent)
        {
            if (reservationEvent is SeatsAreReserved seatsAreReserved)
            {
                _commandBus.Schedule(new CheckSeatsReservationExpiration(seatsAreReserved.ScreeningId, seatsAreReserved.Seats),
                                     at: ClockUtc.Now.AddMinutes(ScreeningReservation.ExpirationMinutes));
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