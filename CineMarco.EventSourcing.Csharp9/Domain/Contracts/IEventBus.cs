namespace CineMarco.EventSourcing.Csharp9.Domain.Contracts
{
    public interface IEventBus
    {
        void Publish(IDomainEvent @event);
    }
}