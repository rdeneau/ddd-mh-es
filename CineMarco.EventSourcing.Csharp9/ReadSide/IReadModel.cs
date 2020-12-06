using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.ReadSide
{
    public interface IReadModel
    {
        void Project(IDomainEvent @event);
    }
}