using CineMarco.EventSourcing.Csharp9.Application;
using Shouldly;

namespace CineMarco.EventSourcing.Csharp9.Tests.Helpers
{
    public class TestBase
    {
        readonly FakeEventStore _eventStore = new();

        readonly FakeEventBus _eventBus = new();

        public void Given(params Event[] events) {
            _eventStore.Initialize(events);
        }

        public void When(ICommand command) {
            var handler = new CommandHandler(_eventStore, _eventBus);
            handler.Handle(command);
        }

        public void ThenExpect(params Event[] events) {
            _eventBus.PublishedEvents.ShouldBe(events);
        }
    }
}