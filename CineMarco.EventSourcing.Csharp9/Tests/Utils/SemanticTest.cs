using System;
using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.Mocks;
using Moq;
using Shouldly;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils
{
    public class SemanticTest
    {
        private readonly FakeEventBus      _eventBus       = new();
        private readonly FakeEventStore    _eventStore     = new();
        private readonly Mock<ICommandBus> _commandBusMock = new();

        private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

        protected bool IgnoreEventTimestamp { get; set; }

        private IEnumerable<IDomainEvent> PublishedEvents => _eventBus.Events;

        protected SemanticTest()
        {
            IgnoreEventTimestamp = true;
        }

        protected void Given(params IDomainEvent[] events)
        {
            _eventStore.Initialize(events);
        }

        protected void When(ICommand command)
        {
            var handler = new CommandHandler(_eventStore, _eventBus, _commandBusMock.Object);
            handler.Handle(command);
        }

        /// <summary>
        /// Check that the given <paramref name="expectedEvents"/> equal the events published
        /// in the event bus <see cref="When"/> executing the command.
        /// </summary>
        /// <remarks>
        /// Use <see cref="IgnoreEventTimestamp"/> to compare (<c>false</c>) or ignore (<c>true</c>, by default)
        /// the timestamp of any <see cref="AuditedEvent"/>.
        /// </remarks>
        protected void ThenExpect(params IDomainEvent[] expectedEvents)
        {
            Sanitize(PublishedEvents)
                .ShouldBe(Sanitize(expectedEvents));
        }

        private IEnumerable<IDomainEvent> Sanitize(IEnumerable<IDomainEvent> events) =>
            IgnoreEventTimestamp
                ? events.Select(Sanitize)
                : events;

        private IDomainEvent Sanitize(IDomainEvent @event) =>
            @event is AuditedEvent auditedEvent
                ? auditedEvent with { At = _now }
                : @event;

        protected void ThenExpectSchedule(ICommand command)
        {
            _commandBusMock.Verify(x => x.Schedule(command, It.IsAny<DateTimeOffset>()));
        }
    }
}