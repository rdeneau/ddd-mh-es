using System;
using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Common.Collections;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Domain.Contracts;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils.Mocks
{
    public class FakeEventStore : IEventStore
    {
        private readonly List<IDomainEvent> _events = new();

        public List<IDomainEvent> AppendedEvents { get; } = new();

        public Action<IDomainEvent> OnEventAppended { get; set; } = _ => { };

        public void Initialize(IEnumerable<IDomainEvent> events) =>
            _events.AddRange(events);

        public IEnumerable<IDomainEvent> Search(string by) =>
            _events.Where(x => $"{x}".Contains(by)); // Use AuditedEvent record ToString() generated method

        public void Append(IEnumerable<IDomainEvent> events) =>
            Append(events.ToList());

        private void Append(IReadOnlyCollection<IDomainEvent> events)
        {
            _events.AddRange(events);
            AppendedEvents.AddRange(events);
            events.ForEach(OnEventAppended);
        }
    }
}