using CineMarco.EventSourcing.Csharp9.Common.Collections;
using CineMarco.EventSourcing.Csharp9.Domain;

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
    /// Naming convention: name of the query + "Response" (or "Info")
    /// </summary>
    public interface IQueryResponse { }

    public enum QueryResponseStatus
    {
        Ok,
        NotFound,
    }

    public sealed record ScreeningAvailableSeats(ScreeningId ScreeningId) : IQuery<ScreeningAvailableSeatsResponse>;

    public sealed record ScreeningAvailableSeatsResponse(
        ScreeningId               ScreeningId,
        ReadOnlyList<SeatNumber>? Seats  = null,
        QueryResponseStatus       Status = QueryResponseStatus.Ok
    ) : IQueryResponse { }
}