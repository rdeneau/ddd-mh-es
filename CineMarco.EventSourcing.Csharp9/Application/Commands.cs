using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    public interface ICommand { } // Marker interface

    public sealed record ReserveSeats(ScreeningId ScreeningId, NumberOfSeats Seats) : ICommand;
}