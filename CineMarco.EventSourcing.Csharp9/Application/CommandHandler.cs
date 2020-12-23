using System;
using System.Collections.Generic;
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
        private readonly IEventStore       _eventStore;
        private readonly IEventBus         _eventBus;
        private readonly ICommandScheduler _commandScheduler;

        public CommandHandler(IEventStore eventStore, IEventBus eventBus, ICommandScheduler commandScheduler)
        {
            _eventStore       = eventStore;
            _eventBus         = eventBus;
            _commandScheduler = commandScheduler;
        }

        public void Handle(ICommand command) =>
            HandleCore((dynamic) command); // ⚠ Dynamic dispatch

        private void HandleCore(object command) =>
            throw new ArgumentException($"Not supported command {command.GetType().FullName}", nameof(command));

        private void HandleCore(BookSeats command) =>
            ScreeningReservationById(command.ScreeningId)
                .Book(command.Seats, command.ClientId, command.At)
                .PublishedTo(_eventBus);

        private void HandleCore(CheckSeatsReservationExpiration command) =>
            ScreeningReservationById(command.ScreeningId)
                .CheckSeatsReservationExpiration(command.ClientId, command.Seats)
                .PublishedTo(_eventBus);

        private void HandleCore(ReserveSeats command)
        {
            var reservationEvents = ScreeningReservationById(command.ScreeningId)
                                   .Reserve(command.Seats, command.ClientId, command.At)
                                   .ToReadOnlyList();
            reservationEvents.PublishedTo(_eventBus);
            ScheduleCheckSeatsReservationExpiration(reservationEvents);
        }

        private void HandleCore(ReserveSeatsInBulk command)
        {
            var reservationEvents = ScreeningReservationById(command.ScreeningId)
                    .ReserveSeatsInBulkEvent(command.Seats, command.ClientId)
                    .ToReadOnlyList();
            reservationEvents.PublishedTo(_eventBus);
            ScheduleCheckSeatsReservationExpiration(reservationEvents);
        }

        private void ScheduleCheckSeatsReservationExpiration(IEnumerable<IScreeningReservationEvent> reservationEvents)
        {
            foreach (var reservationEvent in reservationEvents)
            {
                if (reservationEvent is SeatsWereReserved seatsAreReserved)
                {
                    var command = new CheckSeatsReservationExpiration(
                                      seatsAreReserved.ClientId,
                                      seatsAreReserved.ScreeningId,
                                      seatsAreReserved.Seats) { At = seatsAreReserved.At};
                    _commandScheduler.Schedule(command, after: ScreeningReservation.ExpirationDelay);
                }
            }
        }

        /// <remarks>
        /// Cette méthode pourrait être déportée dans un `ScreeningReservationRepository`, lui-même
        /// pouvant se baser sur un Repo générique encapsulant un EventStore.
        ///
        /// Exemples :
        /// * https://github.com/baruica/gym-kotlin-es/blob/main/src/main/kotlin/common/EventStore.kt
        /// * https://github.com/baruica/gym-kotlin-es/blob/main/src/main/kotlin/gym/membership/domain/MemberEventStore.kt
        /// * https://github.com/baruica/gym-kotlin-es/blob/main/src/main/kotlin/gym/membership/infrastructure/MemberInMemoryEventStore.kt
        /// </remarks>
        private ScreeningReservation ScreeningReservationById(ScreeningId screeningId)
        {
            var history = _eventStore.Search(@by: $"ScreeningId = {screeningId}");
            var state   = new ScreeningReservationState(history);
            return new(state);
        }
    }
}