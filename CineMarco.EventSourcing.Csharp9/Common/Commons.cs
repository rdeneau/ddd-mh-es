using System.Collections.Generic;

namespace CineMarco.EventSourcing.Csharp9.Common
{
    public interface ICommand { } // Marker interface

    public interface IEvent { } // Marker interface

    public interface IEventBus
    {
        void Publish(IEvent @event);
    }

    public interface IEventStore
    {
        IEnumerable<IEvent> Search(string by);
    }
}
