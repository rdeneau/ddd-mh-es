using System.Collections.ObjectModel;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.ReadSide.Models
{
    public class ScreeningInfos : KeyedCollection<ScreeningId, ScreeningInfo>, IReadModel
    {
        public void Project(IDomainEvent @event) =>
            Apply((dynamic) @event);

        /// <summary>
        /// Fallback "Apply" method, compulsory to secure the dynamic dispatch in <see cref="Project"/>
        /// </summary>
        // ReSharper disable once UnusedParameter.Local // `_` argument
        private void Apply(IDomainEvent _) { }

        private void Apply(ScreeningWasInitialized @event)
        {
            Add(new ScreeningInfo(@event.ScreeningId, @event.Seats));
        }

        private void Apply(SeatsWereReserved @event)
        {
            if (TryGetValue(@event.ScreeningId, out var screeningInfo))
                screeningInfo.ReserveSeats(@event.Seats);
        }

        private void Apply(SeatsReservationHasExpired @event)
        {
            if (TryGetValue(@event.ScreeningId, out var screeningInfo))
                screeningInfo.FreeSeats(@event.Seats);
        }

        protected override ScreeningId GetKeyForItem(ScreeningInfo item) =>
            item.ScreeningId;
    }
}