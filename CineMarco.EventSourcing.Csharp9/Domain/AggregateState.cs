using System.Collections.Generic;
using System.Linq;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    /// <summary>
    /// <para>
    /// State of an aggregate, reconstructed from the event history, for instance using
    /// <see cref="StateExtensions.RestoreFrom"/> extension method that works by reflection,
    /// calling any matching <see cref="IAggregateState{TEvent}.Apply(TEvent)"/> methods.
    /// </para>
    ///
    /// <para>
    /// It's not necessary to implement <see cref="IAggregateState"/> directly.
    /// Just implement <see cref="IAggregateState{TEvent}"/> for each event participating in the
    /// restoration of the state. Then call <see cref="StateExtensions.RestoreFrom"/> inside the state constructor.
    /// </para>
    /// </summary>
    /// <remarks>
    /// 💡 State is separated from the Aggregate class to follow SRP (Single Responsibility Principle):
    /// <list type="bullet">
    /// <item><description>State handles reconstruction</description></item>
    /// <item><description>Aggregate handles business behaviors</description></item>
    /// </list>
    /// </remarks>
    public interface IAggregateState { }

    public interface IAggregateState<in TEvent> : IAggregateState
        where TEvent : IDomainEvent
    {
        void Apply(TEvent @event);
    }

    public static class StateExtensions
    {
        /// <summary>
        /// Apply events from the given <paramref name="history"/> that are handled by the <paramref name="state"/>
        /// calling the matching <see cref="IAggregateState{TEvent}.Apply(TEvent)"/> methods, using Reflection to
        /// determine which <see cref="IAggregateState{TEvent}"/> interfaces are implemented by <paramref name="state"/>.
        /// </summary>
        /// <remarks>
        /// Other options, less recommended, to restore the state from the event history:
        /// <list type="bullet">
        /// <item><description>
        /// Loop through the events and make a <c>dynamic</c> dispatch <c>Apply((dynamic) @event)</c>
        /// but don't forget to have a fallback <c>Apply(object @event) {}</c> for unhandled events.
        /// </description></item>
        /// <item><description>
        /// Use pattern matching on the event type to call <see cref="IAggregateState{TEvent}.Apply(TEvent)"/>
        /// </description></item>
        /// </list>
        /// </remarks>
        public static void RestoreFrom(this IAggregateState state, IEnumerable<IDomainEvent> history)
        {
            var handledEvents = state.GetType()
                                     .GetInterfaces()
                                     .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IAggregateState<>))
                                     .Select(x => new
                                     {
                                         EventType   = x.GetGenericArguments()[0],
                                         ApplyMethod = x.GetMethod(nameof(IAggregateState<IDomainEvent>.Apply)),
                                     })
                                     .ToList();

            foreach (var @event in history)
            {
                var handledEvent = handledEvents.FirstOrDefault(x => x.EventType == @event.GetType());
                handledEvent?.ApplyMethod?.Invoke(state, new object?[] { @event });
            }
        }

        /// <summary>
        /// Can be used to mutate the given <paramref name="state"/>,
        /// applying the given <paramref name="event"/> and then returning it.
        /// </summary>
        public static TEvent AppliedOn<TEvent>(this TEvent @event, IAggregateState<TEvent> state) where TEvent: IDomainEvent
        {
            state.Apply(@event);
            return @event;
        }
    }
}