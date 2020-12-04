using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Domain;
using Shouldly;
using Xunit.Abstractions;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils
{
    public class TestBase
    {
        private readonly FakeEventStore    _eventStore = new();
        private readonly FakeEventBus      _eventBus   = new();
        private readonly ITestOutputHelper _outputHelper;

        protected TestBase(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

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
            // Print debug info to understand random test failure
            if (events.Length == _eventBus.PublishedEvents.Count)
            {
                for (var i = 0; i < events.Length; i++)
                {
                    _outputHelper.WriteLine($"Events #{i} are equal: {events[i] == _eventBus.PublishedEvents[i]}");
                }
            }

            _eventBus.PublishedEvents.ShouldBe(events);
        }
    }
}