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
    public class ReserveSeatsInBulkShould : SemanticTest
    {
        [Fact]
        public void Reserve_first_seat()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B")));

            When(
                new ReserveSeatsInBulk(Client1, Screening1, new NumberOfSeats(1)));

            ThenExpect(
                new SeatsAreReserved(Client1, Screening1, Seats("A")));
        }

        [Fact]
        public void Reserve_second_seat()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B")),
                new SeatsAreReserved(Client1, Screening1, Seats("A")));

            When(
                new ReserveSeatsInBulk(Client1, Screening1, new NumberOfSeats(1)));

            ThenExpect(
                new SeatsAreReserved(Client1, Screening1, Seats("B")));
        }

        [Fact]
        public void Reserve_two_seats_given_two_seats_are_already_reserved()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C", "D")),
                new SeatsAreReserved(Client1, Screening1, Seats("A", "C")));

            When(
                new ReserveSeatsInBulk(Client1, Screening1, new NumberOfSeats(2)));

            ThenExpect(
                new SeatsAreReserved(Client1, Screening1, Seats("B", "D")));
        }

        [Fact]
        public void Fail_to_reserve_a_seat_not_available()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A")),
                new SeatsAreReserved(Client1, Screening1, Seats("A")));

            When(
                new ReserveSeatsInBulk(Client1, Screening1, new NumberOfSeats(1)));

            ThenExpect(
                new SeatsBulkReservationFailed(Client1, Screening1, NumberOfSeats: 1));
        }

        [Fact]
        public void Fail_to_reserve_too_much_seat_for_the_screening()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A")));

            When(
                new ReserveSeatsInBulk(Client1, Screening1, new NumberOfSeats(2)));

            ThenExpect(
                new SeatsBulkReservationFailed(Client1, Screening1, NumberOfSeats: 2));
        }
    }
}