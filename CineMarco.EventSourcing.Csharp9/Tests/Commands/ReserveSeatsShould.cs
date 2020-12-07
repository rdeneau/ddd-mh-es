using System;
using CineMarco.EventSourcing.Csharp9.Tests.Utils;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.Fixtures;
using Xunit;

namespace CineMarco.EventSourcing.Csharp9.Tests.Commands
{
    public class ReserveSeatsShould : SemanticTest
    {
        private readonly ScreeningReservationFixture screening = new();

        [Fact]
        public void Reserve_first_seat()
        {
            Given(
                screening.IsInitialized(Seats.Number("A", "B")));

            When(
                screening.ReserveSeats("A"));

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
                screening.ReserveSeats("B"));

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
                screening.ReserveSeats("B", "D"));

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
                screening.ReserveSeats("A"));

            ThenExpect(
                screening.HasFailedToReserveSeats("A"));
        }

        [Fact]
        public void Fail_to_reserve_a_seat_not_known()
        {
            Given(
                screening.IsInitialized(Seats.Number("A")),
                screening.HasSeatsReserved("A"));

            When(
                screening.ReserveSeats("X"));

            ThenExpect(
                screening.HasFailedToReserveSeatsUnknown("X"));
        }

        [Theory]
        [InlineData(15)]
        [InlineData(20)]
        public void Reserve_a_seat_enough_minutes_before_the_screening(int minutesBeforeScreening)
        {
            Given(
                screening.IsInitialized(Seats.Number("A", "B"), Occurring.Later(minutesBeforeScreening)));

            When(
                screening.ReserveSeats("A"));

            ThenExpect(
                screening.HasSeatsReserved("A"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(14)]
        public void Fail_to_reserve_a_seat_less_than_15_minutes_before_the_screening(int minutesBeforeScreening)
        {
            Given(
                screening.IsInitialized(Seats.Number("A", "B"), Occurring.Later(minutesBeforeScreening)));

            When(
                screening.ReserveSeats("A"));

            ThenExpect(
                screening.HasFailedToReserveSeatsTooClosedToScreeningTime("A"));
        }

        [Fact]
        public void Reserve_a_seat_given_previous_reservation_expired()
        {
            Given(
                screening.IsInitialized(Seats.Number("A", "B")),
                screening.HasSeatsReserved("A") with { At = Occurring.Sooner(minutesAgo: 12) },
                screening.HasSeatsReservationExpired("A"));

            When(
                screening.ReserveSeats("A"));

            ThenExpect(
                screening.HasSeatsReserved("A"));
        }

        [Fact]
        public void Schedule_reservation_expiration_check_after_12_minutes()
        {
            Given(
                screening.IsInitialized(Seats.Number("A", "B", "C")));

            When(
                screening.ReserveSeats("A", "B"));

            ThenExpectSchedule(
                screening.CheckSeatsReservationExpiration("A", "B"), TimeSpan.Parse("00:12:00"));
        }
    }
}