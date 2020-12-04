using System.Collections.Generic;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Domain.Contracts;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils
{
    public class FakeEventBus : IEventBus
    {
        public List<IDomainEvent> Events { get; } = new();

        public void Publish(IDomainEvent @event) => Events.Add(@event);
    }
}