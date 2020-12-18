using System;
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
    public class ReserveSeatsShould : SemanticTest
    {
        [Fact]
        public void Reserve_first_seat()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B")));

            When(
                new ReserveSeats(Client1, Screening1, Seats("A")));

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
                new ReserveSeats(Client1, Screening1, Seats("B")));

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
                new ReserveSeats(Client1, Screening1, Seats("B", "D")));

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
                new ReserveSeats(Client1, Screening1, Seats("A")));

            ThenExpect(
                new SeatsReservationFailed(Client1, Screening1, Seats("A")));
        }

        [Fact]
        public void Fail_to_reserve_a_seat_not_known()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A")),
                new SeatsAreReserved(Client1, Screening1, Seats("A")));

            When(
                new ReserveSeats(Client1, Screening1, Seats("X")));

            ThenExpect(
                new SeatsReservationFailed(Client1, Screening1, Seats("X"), ReservationFailure.SomeSeatsAreUnknown));
        }

        [Theory]
        [InlineData(15)]
        [InlineData(20)]
        public void Reserve_a_seat_enough_minutes_before_the_screening(int minutesBeforeScreening)
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Later(minutesBeforeScreening), Seats("A", "B")));

            When(
                new ReserveSeats(Client1, Screening1, Seats("A")));

            ThenExpect(
                new SeatsAreReserved(Client1, Screening1, Seats("A")));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(14)]
        public void Fail_to_reserve_a_seat_less_than_15_minutes_before_the_screening(int minutesBeforeScreening)
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Later(minutesBeforeScreening), Seats("A", "B")));

            When(
                new ReserveSeats(Client1, Screening1, Seats("A")));

            ThenExpect(
                new SeatsReservationFailed(Client1, Screening1, Seats("A"), ReservationFailure.TooClosedToScreeningTime));
        }

        [Fact]
        public void Reserve_a_seat_given_previous_reservation_expired()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B")),
                new SeatsAreReserved(Client1, Screening1, Seats("A")) { At = Occurring.Sooner(minutesAgo: 12) },
                new SeatReservationHasExpired(Client1, Screening1, Seats("A")));

            When(
                new ReserveSeats(Client1, Screening1, Seats("A")));

            ThenExpect(
                new SeatsAreReserved(Client1, Screening1, Seats("A")));
        }

        [Fact]
        public void Schedule_reservation_expiration_check_after_12_minutes()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C")));

            When(
                new ReserveSeats(Client1, Screening1, Seats("A", "B")) { At = FixedTimeStamp });

            ThenExpectSchedule(
                new CheckSeatsReservationExpiration(Client1, Screening1, Seats("A", "B")) { At = FixedTimeStamp },
                delay: TimeSpan.Parse("00:12:00"));
        }
    }
}