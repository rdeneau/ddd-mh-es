using System.Collections.Generic;
using CineMarco.EventSourcing.Csharp9.Domain;

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
        public ScreeningAvailableSeatsResponse Handle(ScreeningAvailableSeats query)
        {
            return new(ScreeningId.Undefined, new List<SeatNumber>());
        }
    }
}