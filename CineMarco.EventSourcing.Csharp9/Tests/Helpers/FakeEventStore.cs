using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Domain.Contracts;

namespace CineMarco.EventSourcing.Csharp9.Tests.Helpers
{
    public class FakeEventStore : IEventStore
    {
        private readonly List<IEvent> _events = new();

        public void Initialize(IEnumerable<IEvent> events)
        {
            _events.AddRange(events);
        }

        public IEnumerable<IEvent> Search(string by) =>
            _events.Where(x => $"{x}".Contains(by)); // Use AuditedEvent record ToString() generated method
    }
}