using System;
using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.ReadSide;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.Mocks;
using Moq;
using Shouldly;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils
{
    /// <summary>
    /// <para>Base class for semantic tests (BDD style)</para>
    ///
    /// <para>For Commands:</para>
    /// <list type="bullet">
    /// <item><description><see cref="Given(IDomainEvent[])"/></description></item>
    /// <item><description><see cref="When(ICommand)"/></description></item>
    /// <item><description><see cref="ThenExpect(IDomainEvent[])"/> or</description></item>
    /// <item><description><see cref="ThenExpectSchedule{TCommand}(TCommand, TimeSpan?)"/></description></item>
    /// </list>
    ///
    /// <para>For Queries:</para>
    /// <list type="bullet">
    /// <item><description><see cref="Given(IDomainEvent[])"/></description></item>
    /// <item><description><see cref="WhenQuery(IQuery)"/></description></item>
    /// <item><description><see cref="ThenExpect(IQueryResponse)"/></description></item>
    /// </list>
    /// </summary>
    public class SemanticTest
    {
        private readonly FakeEventBus            _eventBus             = new();
        private readonly FakeEventStore          _eventStore           = new();
        private readonly Mock<ICommandScheduler> _commandSchedulerMock = new();
        private readonly ReadModels              _readModels           = new();

        protected readonly DateTimeOffset FixedTimeStamp = DateTimeOffset.UtcNow;

        private IQueryResponse? _response;

        protected bool IgnoreEventTimestamp { get; set; } = true;

        private IEnumerable<IDomainEvent> PublishedEvents => _eventBus.Events;

        protected SemanticTest()
        {
            _eventBus.OnEventPublished = _readModels.Project;
        }

        protected void Given(params IDomainEvent[] events)
        {
            _eventStore.Initialize(events);
            _readModels.Aggregate(events);
        }

        protected void When(ICommand command)
        {
            var handler = new CommandHandler(_eventStore, _eventBus, _commandSchedulerMock.Object);
            handler.Handle(command);
        }

        protected void WhenQuery(IQuery query)
        {
            var handler = new QueryHandler(_readModels);
            _response = handler.Handle((dynamic) query);
        }

        /// <summary>
        /// Check that the given <paramref name="expectedEvents"/> equal the events published
        /// in the event bus <see cref="When"/> executing the command.
        /// </summary>
        /// <remarks>
        /// Use <see cref="IgnoreEventTimestamp"/> to compare (<c>false</c>) or ignore (<c>true</c>, by default)
        /// the timestamp of any <see cref="AuditedEvent"/>.
        /// </remarks>
        protected void ThenExpect(params IDomainEvent[] expectedEvents) =>
            Sanitize(PublishedEvents)
                .ShouldBe(Sanitize(expectedEvents));

        private IEnumerable<IDomainEvent> Sanitize(IEnumerable<IDomainEvent> events) =>
            IgnoreEventTimestamp
                ? events.Select(Sanitize)
                : events;

        private IDomainEvent Sanitize(IDomainEvent @event) =>
            @event is AuditedEvent auditedEvent
                ? auditedEvent with { At = FixedTimeStamp }
                : @event;

        protected void ThenExpect(IQueryResponse expectedResponse) =>
            _response.ShouldBe(expectedResponse);

        protected void ThenExpectSchedule<TCommand>(TCommand command, TimeSpan? delay) where TCommand: ICommand =>
            _commandSchedulerMock.Verify(
                x => x.Schedule(
                    It.Is<TCommand>(y => Verify(() => y.ShouldBe(command, ""))),
                    It.Is<TimeSpan>(t => delay == null || Verify(() => t.ShouldBe(delay.Value, "")))));

        private static bool Verify(Action assertion)
        {
            assertion();
            return true;
        }
    }
}