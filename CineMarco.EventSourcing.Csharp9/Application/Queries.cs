using System.Collections.Generic;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    /// <summary>
    /// Naming convention: start with a noun (i.e. no "Get", no "Query").
    /// E.g. "AvailableSeats"
    /// </summary>
    public interface IQuery : IMarkerInterface { }

    public interface IQuery<TResponse> : IQuery where TResponse: IQueryResponse { }

    /// <summary>
    /// Naming convention: name of the query + "Response" (or "Info")
    /// </summary>
    public interface IQueryResponse : IMarkerInterface { }

    public sealed record ScreeningAvailableSeats(ScreeningId ScreeningId) : IQuery<ScreeningAvailableSeatsResponse>;

    public sealed record ScreeningAvailableSeatsResponse(ScreeningId ScreeningId, IReadOnlyList<SeatNumber> Seats) : IQueryResponse { }
}