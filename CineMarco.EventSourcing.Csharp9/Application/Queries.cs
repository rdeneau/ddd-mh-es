using System;
using CineMarco.EventSourcing.Csharp9.Common.Collections;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.ReadSide.Models;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    /// <summary>
    /// Naming convention: start with a noun (i.e. no "Get", no "Query").
    /// E.g. "AvailableSeats"
    /// </summary>
    public interface IQuery { }

    // ReSharper disable once UnusedTypeParameter
    public interface IQuery<TResponse> : IQuery where TResponse: IQueryResponse { }

    /// <summary>
    /// Naming convention: name of the query + "Response"
    /// </summary>
    public interface IQueryResponse { }

    public sealed record ScreeningAvailableSeats(ScreeningId ScreeningId) : IQuery<ScreeningAvailableSeatsResponse>;

    public sealed record ScreeningAvailableSeatsResponse(
        ScreeningId              ScreeningId,
        ReadOnlyList<SeatNumber> Seats
    ) : IQueryResponse
    {
        public static ScreeningAvailableSeatsResponse NotFound(ScreeningId screeningId) =>
            new(screeningId, new ReadOnlyList<SeatNumber>());
    }

    public sealed record ClientScreeningReservations(ClientId ClientId, ScreeningId ScreeningId) : IQuery<ClientScreeningReservationResponse>;

    public sealed record ClientScreeningReservationResponse(
        ClientId                                ClientId,
        ScreeningId                             ScreeningId,
        ReadOnlyList<ClientSeatReservationInfo> Seats
    ) : IQueryResponse
    {
        public static ClientScreeningReservationResponse NotFound(ClientId    clientId, ScreeningId screeningId) =>
            new(clientId, screeningId, new ReadOnlyList<ClientSeatReservationInfo>());
    }

    public sealed record ClientSeatReservationInfo(
        SeatNumber        SeatNumber,
        DateTimeOffset    ReservationDate,
        ReservationStatus ReservationStatus);
}