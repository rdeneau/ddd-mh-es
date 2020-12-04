using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Domain;
using Shouldly;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils
{
    public class TestBase
    {
        private readonly FakeEventBus   _eventBus   = new();
        private readonly FakeEventStore _eventStore = new();

        private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

        protected bool IgnoreEventTimestamp { get; set; }

        private IEnumerable<IDomainEvent> PublishedEvents => _eventBus.Events;

        protected void Given(params IDomainEvent[] events)
        {
            _eventStore.Initialize(events);
        }

        protected void When(ICommand command)
        {
            var handler = new CommandHandler(_eventStore, _eventBus);
            handler.Handle(command);

            Thread.Sleep(10);
        }

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
    }
}