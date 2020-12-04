using System.Collections.Generic;

namespace CineMarco.EventSourcing.Csharp9.Domain.Contracts
{
    public interface IEventStore
    {
        IEnumerable<IEvent> Search(string by);
    }
}