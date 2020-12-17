using System.Linq;
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

    public class QueryHandler : IQueryHandler<ScreeningAvailableSeats, ScreeningAvailableSeatsResponse>,
                                IQueryHandler<ClientScreeningReservations, ClientScreeningReservationResponse>
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

        public ClientScreeningReservationResponse Handle(ClientScreeningReservations query)
        {
            if (!_readModels.ClientReservationInfos.TryGetValue(query.ClientId, out var clientReservationInfo))
            {
                return new(query.ClientId, query.ScreeningId, null, QueryResponseStatus.NotFound);
            }

            var clientSeatReservationInfos = clientReservationInfo.Reservations.Select(
                x => new ClientSeatReservationInfo(x.SeatNumber, x.ReservationDate, x.ReservationStatus));
            return new(query.ClientId, query.ScreeningId, clientSeatReservationInfos.ToReadOnlyList());
        }
    }
}