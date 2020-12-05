using System;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    public interface ICommandBus
    {
        void Schedule(ICommand command, DateTimeOffset at);
    }
}