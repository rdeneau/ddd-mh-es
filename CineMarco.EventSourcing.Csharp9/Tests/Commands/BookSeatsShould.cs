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
                new ScreeningWasInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C")),
                new SeatsWereReserved(Client1, Screening1, Seats("A", "B")));

            When(
                new BookSeats(Client1, Screening1, Seats("A", "B")));

            ThenExpect(
                new SeatsWereBooked(Client1, Screening1, Seats("A", "B")));
        }

        [Fact]
        public void Fail_to_book_seats_not_reserved()
        {
            Given(
                new ScreeningWasInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C")));

            When(
                new BookSeats(Client1, Screening1, Seats("A", "B")));

            ThenExpect(
                new SeatsBookingHasFailed(Client1, Screening1, Seats("A", "B")));
        }

        [Fact]
        public void Fail_to_book_seats_already_booked()
        {
            Given(
                new ScreeningWasInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C")),
                new SeatsWereReserved(Client1, Screening1, Seats("A", "B")),
                new SeatsWereBooked(Client1, Screening1, Seats("A", "B")));

            When(
                new BookSeats(Client1, Screening1, Seats("A", "B")));

            ThenExpect(
                new SeatsBookingHasFailed(Client1, Screening1, Seats("A", "B")));
        }
    }
}