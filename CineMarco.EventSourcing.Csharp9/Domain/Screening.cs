using CineMarco.EventSourcing.Csharp9.Common;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public record Screening(ScreeningState State, IEventBus EventBus)
    {
        public void ReserveSeats(ScreeningId screeningId, NumberOfSeats count)
        {
            if (State.SeatsLeft.Value >= count.Value)
                EventBus.Publish(new SeatsReserved(screeningId, count));
            else
                EventBus.Publish(new SeatsNotReserved(screeningId, count));
        }
    }
}