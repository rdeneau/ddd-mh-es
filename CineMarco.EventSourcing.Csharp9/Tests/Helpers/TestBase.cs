using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Domain;
using Shouldly;

namespace CineMarco.EventSourcing.Csharp9.Tests.Helpers
{
    public class TestBase
    {
        private readonly FakeEventStore _eventStore = new();

        private readonly FakeEventBus _eventBus = new();

        protected void Given(params IEvent[] events)
        {
            _eventStore.Initialize(events);
        }

        protected void When(ICommand command)
        {
            var handler = new CommandHandler(_eventStore, _eventBus);
            handler.Handle(command);
        }

        protected void ThenExpect(params IEvent[] events)
        {
            _eventBus.PublishedEvents.ShouldBe(events);
        }
    }
}