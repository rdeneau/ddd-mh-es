using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Application
{
    public sealed record ReserveSeats(ScreeningId ScreeningId, NumberOfSeats Seats) : ICommand;
}