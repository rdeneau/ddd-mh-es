using CineMarco.EventSourcing.Csharp9.Tests.Utils;
using Xunit;

namespace CineMarco.EventSourcing.Csharp9.Tests.Commands
{
    public class ReserveSeatsShould : TestBase
    {
        public ReserveSeatsShould()
        {
            IgnoreEventTimestamp = true;
        }

        [Fact]
        public void Reserve_first_seat()
        {
            var screening = ScreeningData.WithSeats("A", "B");

            Given(screening.IsInitialized());

            When(screening.ReserveSeats("A"));

            ThenExpect(screening.HasSeatsReserved("A"));
        }

        [Fact]
        public void Reserve_second_seat()
        {
            var screening = ScreeningData.WithSeats("A", "B");

            Given(screening.IsInitialized(),
                  screening.HasSeatsReserved("A"));

            When(screening.ReserveSeats("B"));

            ThenExpect(screening.HasSeatsReserved("B"));
        }

        [Fact]
        public void Reserve_two_seats_given_two_seats_are_already_reserved()
        {
            var screening = ScreeningData.WithSeats("A", "B", "C", "D");

            Given(screening.IsInitialized(),
                  screening.HasSeatsReserved("A", "C"));

            When(screening.ReserveSeats("B", "D"));

            ThenExpect(screening.HasSeatsReserved("B", "D"));
        }

        [Fact]
        public void Fail_to_reserve_a_seat_not_available()
        {
            var screening = ScreeningData.WithSeats("A");

            Given(screening.IsInitialized(),
                  screening.HasSeatsReserved("A"));

            When(screening.ReserveSeats("A"));

            ThenExpect(screening.HasFailedToReserveSeats("A"));
        }

        [Fact]
        public void Fail_to_reserve_a_seat_not_known()
        {
            var screening = ScreeningData.WithSeats("A");

            Given(screening.IsInitialized(),
                  screening.HasSeatsReserved("A"));

            When(screening.ReserveSeats("X"));

            ThenExpect(screening.HasFailedToReserveSeatsUnknown("X"));
        }
    }
}