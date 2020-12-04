using CineMarco.EventSourcing.Csharp9.Tests.Utils;
using Xunit;

namespace CineMarco.EventSourcing.Csharp9.Tests.Commands
{
    public class ReserveSeatsInBulkShould : TestBase
    {
        public ReserveSeatsInBulkShould()
        {
            IgnoreEventTimestamp = true;
        }

        [Fact]
        public void Reserve_first_seat()
        {
            var screening = ScreeningData.WithSeats("A", "B");

            Given(screening.IsInitialized());

            When(screening.ReserveSeatsInBulk(numberOfSeats: 1));

            ThenExpect(screening.HasSeatsReserved("A"));
        }

        [Fact]
        public void Reserve_second_seat()
        {
            var screening = ScreeningData.WithSeats("A", "B");

            Given(screening.IsInitialized(),
                  screening.HasSeatsReserved("A"));

            When(screening.ReserveSeatsInBulk(numberOfSeats: 1));

            ThenExpect(screening.HasSeatsReserved("B"));
        }

        [Fact]
        public void Reserve_two_seats_given_two_seats_are_already_reserved()
        {
            var screening = ScreeningData.WithSeats("A", "B", "C", "D");

            Given(screening.IsInitialized(),
                  screening.HasSeatsReserved("A", "C"));

            When(screening.ReserveSeatsInBulk(numberOfSeats: 2));

            ThenExpect(screening.HasSeatsReserved("B", "D"));
        }

        [Fact]
        public void Fail_to_reserve_a_seat_not_available()
        {
            var screening = ScreeningData.WithSeats("A");

            Given(screening.IsInitialized(),
                  screening.HasSeatsReserved("A"));

            When(screening.ReserveSeatsInBulk(numberOfSeats: 1));

            ThenExpect(screening.HasFailedToReserveSeats(numberOfSeats: 1));
        }

        [Fact]
        public void Fail_to_reserve_too_much_seat_for_the_screening()
        {
            var screening = ScreeningData.WithSeats("A");

            Given(screening.IsInitialized());

            When(screening.ReserveSeatsInBulk(numberOfSeats: 2));

            ThenExpect(screening.HasFailedToReserveSeats(numberOfSeats: 2));
        }
    }
}