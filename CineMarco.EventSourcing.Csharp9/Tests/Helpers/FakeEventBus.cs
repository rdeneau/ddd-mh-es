using System.Collections.Generic;

namespace CineMarco.EventSourcing.Csharp9.Tests.Helpers
{
    public class FakeEventBus : IEventBus
    {
        public List<Event> PublishedEvents { get; } = new();

        public void Publish(Event @event) => PublishedEvents.Add(@event);
    }
}