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
        public void Book_reserved_seats()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C")),
                new SeatsAreReserved(Client1, Screening1, Seats("A", "B")));

            When(
                new BookSeats(Client1, Screening1, Seats("A", "B")));

            ThenExpect(
                new SeatsAreBooked(Client1, Screening1, Seats("A", "B")));
        }

        [Fact]
        public void Fail_to_book_seats_not_reserved()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C")));

            When(
                new BookSeats(Client1, Screening1, Seats("A", "B")));

            ThenExpect(
                new SeatsBookingFailed(Client1, Screening1, Seats("A", "B")));
        }

        [Fact]
        public void Fail_to_book_seats_already_booked()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C")),
                new SeatsAreReserved(Client1, Screening1, Seats("A", "B")),
                new SeatsAreBooked(Client1, Screening1, Seats("A", "B")));

            When(
                new BookSeats(Client1, Screening1, Seats("A", "B")));

            ThenExpect(
                new SeatsBookingFailed(Client1, Screening1, Seats("A", "B")));
        }
    }
}