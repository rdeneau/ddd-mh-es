using System;
using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Common.Collections;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Domain.Contracts;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    /// <summary>
    /// Global command handler, for all commands
    /// </summary>
    public class CommandHandler
    {
        private readonly ICommandScheduler _commandScheduler;
        private readonly IEventStore       _eventStore;

        public CommandHandler(ICommandScheduler commandScheduler, IEventStore eventStore)
        {
            _eventStore       = eventStore;
            _commandScheduler = commandScheduler;
        }

        public void HandleAny(ICommand command) =>
            Handle((dynamic) command); // ⚠ Dynamic dispatch

        private void Handle(object command) =>
            throw new ArgumentException($"Not supported command {command.GetType().FullName}", nameof(command));

        private void Handle(BookSeats command) =>
            ScreeningReservationById(command.ScreeningId)
                .Book(command.Seats, command.ClientId, command.At)
                .AppendEventsTo(_eventStore);

        private void Handle(CheckSeatsReservationExpiration command) =>
            ScreeningReservationById(command.ScreeningId)
                .CheckSeatsReservationExpiration(command.ClientId, command.Seats)
                .AppendEventsTo(_eventStore);

        private void Handle(ReserveSeats command) =>
            ScreeningReservationById(command.ScreeningId)
                .Reserve(command.Seats, command.ClientId, command.At)
                .AppendEventsTo(_eventStore)
                .Then(ScheduleToCheckSeatsReservationExpiration);

        private void Handle(ReserveSeatsInBulk command) =>
            ScreeningReservationById(command.ScreeningId)
                .ReserveSeatsInBulkEvent(command.Seats, command.ClientId)
                .AppendEventsTo(_eventStore)
                .Then(ScheduleToCheckSeatsReservationExpiration);

        private void ScheduleToCheckSeatsReservationExpiration(IReadOnlyList<IDomainEvent> reservationEvents)
        {
            foreach (var @event in reservationEvents.OfType<SeatsWereReserved>())
            {
                var command = new CheckSeatsReservationExpiration(
                                  @event.ClientId,
                                  @event.ScreeningId,
                                  @event.Seats) { At = @event.At };
                _commandScheduler.Schedule(command, ScreeningReservation.ExpirationDelay);
            }
        }

        private ScreeningReservation ScreeningReservationById(ScreeningId screeningId)
        {
            var history = _eventStore.Search(@by: $"ScreeningId = {screeningId}");
            var state   = new ScreeningReservationState(history);
            return new(state);
        }
    }
}