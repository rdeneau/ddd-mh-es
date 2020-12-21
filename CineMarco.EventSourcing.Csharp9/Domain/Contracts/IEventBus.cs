using System.Collections.Generic;

namespace CineMarco.EventSourcing.Csharp9.Domain.Contracts
{
    public interface IEventBus
    {
        void Publish(IDomainEvent @event);
    }

    public static class EventExtensions
    {
        public static T PublishedTo<T>(this T @event, IEventBus eventBus) where T: IDomainEvent
        {
            eventBus.Publish(@event);
            return @event;
        }

        public static void PublishedTo<T>(this IEnumerable<T> events, IEventBus eventBus) where T: IDomainEvent
        {
            foreach (var @event in events)
            {
                @event.PublishedTo(eventBus);
            }
        }
    }
}