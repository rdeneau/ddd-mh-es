using System.Linq;
using CineMarco.EventSourcing.Csharp9.Common.Collections;
using CineMarco.EventSourcing.Csharp9.ReadSide;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    public interface IQueryHandler<in TQuery, out TResponse>
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
            _readModels.Screenings.TryGetValue(query.ScreeningId, out var reservation)
                ? new(query.ScreeningId, reservation.AvailableSeats.ToReadOnlyList())
                : ScreeningAvailableSeatsResponse.NotFound(query.ScreeningId);

        public ClientScreeningReservationResponse Handle(ClientScreeningReservations query)
        {
            if (!_readModels.ClientReservations.TryGetValue(query.ClientId, out var clientReservationInfo))
            {
                return ClientScreeningReservationResponse.NotFound(query.ClientId, query.ScreeningId);
            }

            var clientSeatReservationInfos =
                clientReservationInfo.Reservations
                                     .Where(x => x.ScreeningId == query.ScreeningId)
                                     .Select(x => new ClientSeatReservationInfo(x.SeatNumber, x.ReservedAt, x.Status));
            return new(query.ClientId, query.ScreeningId, clientSeatReservationInfos.ToReadOnlyList());
        }
    }
}