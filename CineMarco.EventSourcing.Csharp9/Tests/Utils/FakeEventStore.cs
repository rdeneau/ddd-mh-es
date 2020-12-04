using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Domain.Contracts;

namespace CineMarco.EventSourcing.Csharp9.Tests.Helpers
{
    public class FakeEventStore : IEventStore
    {
        private readonly List<IDomainEvent> _events = new();

        public void Initialize(IEnumerable<IDomainEvent> events)
        {
            _events.AddRange(events);
        }

        public IEnumerable<IDomainEvent> Search(string by) =>
            _events.Where(x => $"{x}".Contains(by)); // Use AuditedEvent record ToString() generated method
    }
}