using System.Collections.Generic;

namespace CineMarco.EventSourcing.Csharp9.Domain.Contracts
{
    public interface IEventStore
    {
        IEnumerable<IDomainEvent> Search(string by);
    }
}