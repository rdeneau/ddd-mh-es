using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Tests.Utils;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers;
using Xunit;
using static CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers.Clients;
using static CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers.Screenings;
using static CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers.SeatNumbers;

namespace CineMarco.EventSourcing.Csharp9.Tests.Commands
{
    public class BookSeatsShould : SemanticTest
    {
        [Fact]
        public void Book_reserved_seats_given_payment_succeeded()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C")),
                new SeatsAreReserved(Client1, Screening1, Seats("A", "B")));

            When(
                new BookSeats(Client1, Screening1, Seats("A", "B")));

            ThenExpect(
                new SeatsAreBooked(Client1, Screening1, Seats("A", "B")));
        }
    }
}