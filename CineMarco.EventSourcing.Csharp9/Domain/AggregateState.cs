using System.Collections.Generic;
using System.Linq;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    /// <summary>
    /// <para>
    /// State of an aggregate, reconstructed from the event history, for instance using
    /// <see cref="StateExtensions.ReconstructFrom"/> extension method that works by reflection,
    /// calling any matching <see cref="IStateFrom{TEvent}.Apply(TEvent)"/> methods.
    /// </para>
    ///
    /// <para>
    /// ☝ Don't implement <see cref="IAggregateState"/> directly. Instead,
    /// implement <see cref="IStateFrom{TEvent}"/> for each event participating in the
    /// reconstruction of the state. Then you could let the aggregate state class derive
    /// from <see cref="AggregateState"/> base class which simply calls
    /// <see cref="StateExtensions.ReconstructFrom"/> in its constructor,
    /// or don't have a base class and call it yourself.
    /// </para>
    ///
    /// <para>
    /// Another option (less recommended) is a "manual reconstruction": loop through the events and make a dynamic dispatch
    /// <c>Apply((dynamic) @event)</c> but also write a default fallback <c>Apply(object @event) {}</c> :s
    /// </para>
    ///
    /// <para>
    /// Final option (less recommended too): pattern matching on the event type and then call <c>Apply(specificTypeEvent)</c>.
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

    public interface IStateFrom<in TEvent> : IAggregateState
        where TEvent : IDomainEvent
    {
        void Apply(TEvent @event);
    }

    public static class StateExtensions
    {
        /// <summary>
        /// Apply events from the given <paramref name="history"/> that are handled by the <paramref name="state"/>
        /// calling the matching <see cref="IStateFrom{TEvent}.Apply(TEvent)"/> methods.
        /// </summary>
        public static void ReconstructFrom(this IAggregateState state, IEnumerable<IDomainEvent> history)
        {
            var handledEvents = state.GetType().GetInterfaces()
                                     .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IStateFrom<>))
                                     .Select(x => new
                                     {
                                         EventType   = x.GetGenericArguments()[0],
                                         ApplyMethod = x.GetMethod(nameof(IStateFrom<IDomainEvent>.Apply)),
                                     })
                                     .ToList();

            foreach (var @event in history)
            {
                var handledEvent = handledEvents.FirstOrDefault(x => x.EventType == @event.GetType());
                handledEvent?.ApplyMethod?.Invoke(state, new object?[] { @event });
            }
        }
    }

    public abstract class AggregateState : IAggregateState
    {
        protected AggregateState(IEnumerable<IDomainEvent> history) =>
            this.ReconstructFrom(history);

    }
}