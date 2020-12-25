using System.Collections.Generic;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Common.Collections;

namespace CineMarco.EventSourcing.Csharp9.Domain.Contracts
{
    public interface IEventStore
    {
        IEnumerable<IDomainEvent> Search(string by);

        void Append(IEnumerable<IDomainEvent> events);
    }

    public static class EventStorageExtensions
    {
        public static IReadOnlyList<IDomainEvent> AppendEventsTo(this IEnumerable<IDomainEvent> events, IEventStore eventStore) =>
            events.ToReadOnlyList()
                  .Then(eventStore.Append);
    }
}
