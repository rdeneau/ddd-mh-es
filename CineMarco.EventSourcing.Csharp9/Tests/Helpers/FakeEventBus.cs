using System.Collections.Generic;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Domain.Contracts;

namespace CineMarco.EventSourcing.Csharp9.Tests.Helpers
{
    public class FakeEventBus : IEventBus
    {
        public List<IEvent> PublishedEvents { get; } = new();

        public void Publish(IEvent @event) => PublishedEvents.Add(@event);
    }
}