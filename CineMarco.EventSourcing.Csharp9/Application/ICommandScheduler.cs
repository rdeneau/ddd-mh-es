using System;
using CineMarco.EventSourcing.Csharp9.Common;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    public interface ICommandScheduler
    {
        void Schedule(ICommand command, DateTimeOffset at);

        public void Schedule(ICommand command, TimeSpan after) =>
            Schedule(command, ClockUtc.Now.Add(after));
    }
}