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
            var screening = BuildScreening.WithSeats("A", "B")
                                          .PlannedTomorrow();

            Given(screening.IsInitialized());

            When(screening.ReserveSeats("A"));

            ThenExpect(screening.HasSeatsReserved("A"));
        }

        [Fact]
        public void Reserve_second_seat()
        {
            var screening = BuildScreening.WithSeats("A", "B")
                                          .PlannedTomorrow();

            Given(screening.IsInitialized(),
                  screening.HasSeatsReserved("A"));

            When(screening.ReserveSeats("B"));

            ThenExpect(screening.HasSeatsReserved("B"));
        }

        [Fact]
        public void Reserve_two_seats_given_two_seats_are_already_reserved()
        {
            var screening = BuildScreening.WithSeats("A", "B", "C", "D")
                                          .PlannedTomorrow();

            Given(screening.IsInitialized(),
                  screening.HasSeatsReserved("A", "C"));

            When(screening.ReserveSeats("B", "D"));

            ThenExpect(screening.HasSeatsReserved("B", "D"));
        }

        [Fact]
        public void Fail_to_reserve_a_seat_not_available()
        {
            var screening = BuildScreening.WithSeats("A")
                                          .PlannedTomorrow();

            Given(screening.IsInitialized(),
                  screening.HasSeatsReserved("A"));

            When(screening.ReserveSeats("A"));

            ThenExpect(screening.HasFailedToReserveSeats("A"));
        }

        [Fact]
        public void Fail_to_reserve_a_seat_not_known()
        {
            // TODO: screening field + initialized in IsInitialized() method
            var screening = BuildScreening.WithSeats("A")
                                          .PlannedTomorrow();

            Given(screening.IsInitialized(),
                  screening.HasSeatsReserved("A"));

            When(screening.ReserveSeats("X"));

            ThenExpect(screening.HasFailedToReserveSeatsUnknown("X"));
        }

        [Theory]
        [InlineData(15)]
        [InlineData(20)]
        public void Reserve_a_seat_enough_minutes_before_the_screening(int minutesBeforeScreening)
        {
            var screening = BuildScreening.WithSeats("A", "B")
                                          .PlannedLater(minutesBeforeScreening);

            Given(screening.IsInitialized());

            When(screening.ReserveSeats("A"));

            ThenExpect(screening.HasSeatsReserved("A"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(14)]
        public void Fail_to_reserve_a_seat_less_than_15_minutes_before_the_screening(int minutesBeforeScreening)
        {
            var screening = BuildScreening.WithSeats("A")
                                          .PlannedLater(minutesBeforeScreening);

            Given(screening.IsInitialized());

            When(screening.ReserveSeats("A"));

            ThenExpect(screening.HasFailedToReserveSeatsTooClosedToScreeningTime("A"));
        }
    }
}