using CineMarco.EventSourcing.Csharp9.Common.Collections;
using CineMarco.EventSourcing.Csharp9.ReadSide;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    public interface IQueryHandler<TQuery, TResponse>
        where TQuery    : IQuery<TResponse>
        where TResponse : IQueryResponse
    {
        TResponse Handle(TQuery query);
    }

    public class QueryHandler : IQueryHandler<ScreeningAvailableSeats, ScreeningAvailableSeatsResponse>
    {
        private readonly ReadModels _readModels;

        public QueryHandler(ReadModels readModels)
        {
            _readModels = readModels;
        }

        public ScreeningAvailableSeatsResponse Handle(ScreeningAvailableSeats query) =>
            _readModels.ScreeningInfos.TryGetValue(query.ScreeningId, out var reservation)
                ? new(query.ScreeningId, reservation.AvailableSeats.ToReadOnlyList())
                : new(query.ScreeningId, Status: QueryResponseStatus.NotFound);
    }
}