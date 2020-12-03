using System;
using System.Collections.Generic;
using System.Linq;

namespace CineMarco.EventSourcing.Csharp9.Tests.Helpers
{
    public class FakeEventStore : IEventStore
    {
        private readonly List<Event> _events = new();

        public void Initialize(IEnumerable<Event> events)
        {
            _events.AddRange(events);
        }

        public IEnumerable<Event> Search(Func<Event, bool> predicate) =>
            _events.Where(predicate);
    }
}