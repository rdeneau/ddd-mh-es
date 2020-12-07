using CineMarco.EventSourcing.Csharp9.Tests.Utils;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.Fixtures;
using Xunit;

namespace CineMarco.EventSourcing.Csharp9.Tests.Commands
{
    public class ReserveSeatsInBulkShould : SemanticTest
    {
        private readonly ScreeningReservationFixture screening = new();

        [Fact]
        public void Reserve_first_seat()
        {
            Given(
                screening.IsInitialized(Seats.Number("A", "B")));

            When(
                screening.ReserveSeatsInBulk(numberOfSeats: 1));

            ThenExpect(
                screening.HasSeatsReserved("A"));
        }

        [Fact]
        public void Reserve_second_seat()
        {
            Given(
                screening.IsInitialized(Seats.Number("A", "B")),
                screening.HasSeatsReserved("A"));

            When(
                screening.ReserveSeatsInBulk(numberOfSeats: 1));

            ThenExpect(
                screening.HasSeatsReserved("B"));
        }

        [Fact]
        public void Reserve_two_seats_given_two_seats_are_already_reserved()
        {
            Given(
                screening.IsInitialized(Seats.Number("A", "B", "C", "D")),
                screening.HasSeatsReserved("A", "C"));

            When(
                screening.ReserveSeatsInBulk(numberOfSeats: 2));

            ThenExpect(
                screening.HasSeatsReserved("B", "D"));
        }

        [Fact]
        public void Fail_to_reserve_a_seat_not_available()
        {
            Given(
                screening.IsInitialized(Seats.Number("A")),
                screening.HasSeatsReserved("A"));

            When(
                screening.ReserveSeatsInBulk(numberOfSeats: 1));

            ThenExpect(
                screening.HasFailedToBulkReserveSeats(numberOfSeats: 1));
        }

        [Fact]
        public void Fail_to_reserve_too_much_seat_for_the_screening()
        {
            Given(
                screening.IsInitialized(Seats.Number("A")));

            When(
                screening.ReserveSeatsInBulk(numberOfSeats: 2));

            ThenExpect(
                screening.HasFailedToBulkReserveSeats(numberOfSeats: 2));
        }
    }
}