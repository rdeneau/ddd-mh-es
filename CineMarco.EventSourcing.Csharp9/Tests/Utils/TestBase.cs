using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Domain;
using Shouldly;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils
{
    public class TestBase
    {
        private readonly FakeEventStore _eventStore = new();

        private readonly FakeEventBus _eventBus = new();

        protected void Given(params IDomainEvent[] events)
        {
            _eventStore.Initialize(events);
        }

        protected void When(ICommand command)
        {
            var handler = new CommandHandler(_eventStore, _eventBus);
            handler.Handle(command);
        }

        protected void ThenExpect(params IDomainEvent[] events)
        {
            _eventBus.PublishedEvents.ShouldBe(events);
        }
    }
}