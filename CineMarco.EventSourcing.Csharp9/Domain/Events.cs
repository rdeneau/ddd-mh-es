namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public sealed record ScreeningInitialized(ScreeningId ScreeningId, NumberOfSeats Seats) : Event();

    public sealed record SeatsReserved(ScreeningId ScreeningId, NumberOfSeats Seats) : Event();

    public sealed record SeatsNotReserved(ScreeningId ScreeningId, NumberOfSeats Seats) : Event();
}