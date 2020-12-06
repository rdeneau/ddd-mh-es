using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.ReadSide.Models;

namespace CineMarco.EventSourcing.Csharp9.ReadSide
{
    public class ReadModels
    {
        public ScreeningInfos ScreeningInfos { get; } = new();

        public void Aggregate(IEnumerable<IDomainEvent> history)
        {
            foreach (dynamic @event in history)
            {
                Project(@event); // ⚠ Dynamic dispatch
            }
        }

        public void Project(IDomainEvent @event)
        {
            foreach (var model in All)
            {
                model.Project(@event);
            }
        }

        private IEnumerable<IReadModel> All =>
            typeof(ReadModels).GetProperties()
                              .Select(f => f.GetValue(this))
                              .OfType<IReadModel>();
    }
}