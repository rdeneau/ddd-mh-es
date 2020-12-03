using System;
using System.Collections.Generic;

namespace CineMarco.EventSourcing.Csharp9
{
    public record Event(DateTimeOffset At)
    {
        public Event() : this(DateTimeOffset.UtcNow) {}
    }

    public interface ICommand { } // Marker interface

    public interface IEventBus
    {
        void Publish(Event @event);
    }

    public interface IEventStore
    {
        IEnumerable<Event> Search(string by);
    }
}

// Hack for C# 9
namespace System.Runtime.CompilerServices
{
    public class IsExternalInit
    {
    }
}
