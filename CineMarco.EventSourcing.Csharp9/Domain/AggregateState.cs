using System.Collections.Generic;
using System.Linq;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    /// <summary>
    /// <para>
    /// State of an aggregate, reconstructed from the event history using
    /// <see cref="StateExtensions.ReconstructFrom"/> extension method.
    /// </para>
    ///
    /// <para>
    /// ☝ Don't implement <see cref="IAggregateState"/> directly. Instead,
    /// implement <see cref="IStateFrom{TEvent}"/> for each event participating in the
    /// reconstruction of the state.
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
}