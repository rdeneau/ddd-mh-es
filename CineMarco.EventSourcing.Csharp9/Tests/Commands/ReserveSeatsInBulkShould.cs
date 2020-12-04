using CineMarco.EventSourcing.Csharp9.Tests.Utils;
using Xunit;
using static CineMarco.EventSourcing.Csharp9.Tests.Utils.ScreeningData;

namespace CineMarco.EventSourcing.Csharp9.Tests.Commands
{
    public class ReserveSeatsInBulkShould : TestBase
    {
        [Fact]
        public void Reserve_first_seat()
        {
            var screening = ScreeningWithSeats("A", "B");

            Given(screening.IsInitialized());

            When(screening.ReserveSeatsInBulk(numberOfSeats: 1));

            ThenExpect(screening.HasSeatsReserved("A"));
        }

        [Fact]
        public void Reserve_second_seat()
        {
            var screening = ScreeningWithSeats("A", "B");

            Given(screening.IsInitialized(),
                  screening.HasSeatsReserved("A"));

            When(screening.ReserveSeatsInBulk(numberOfSeats: 1));

            ThenExpect(screening.HasSeatsReserved("B"));
        }

        [Fact]
        public void Reserve_two_seats_given_two_seats_are_already_reserved()
        {
            var screening = ScreeningWithSeats("A", "B", "C", "D");

            Given(screening.IsInitialized(),
                  screening.HasSeatsReserved("A", "C"));

            When(screening.ReserveSeatsInBulk(numberOfSeats: 2));

            ThenExpect(screening.HasSeatsReserved("B", "D"));
        }

        [Fact]
        public void Fail_to_reserve_a_seat_not_available()
        {
            var screening = ScreeningWithSeats("A");

            Given(screening.IsInitialized(),
                  screening.HasSeatsReserved("A"));

            When(screening.ReserveSeatsInBulk(numberOfSeats: 1));

            ThenExpect(screening.HasFailedToReserveSeats(numberOfSeats: 1));
        }

        // [Fact]
        // public void Reserve_Too_Much_Seats_Should_Fail()
        // {
        //     Given(ScreeningInitialized.Generate(screening1Id, new NumberOfSeats(10)));
        //
        //     When(new ReserveSeats(screening1Id, new NumberOfSeats(20)));
        //
        //     ThenExpect(new SeatsNotReserved(screening1Id, new NumberOfSeats(20)));
        // }
        //
        // [Fact]
        // public void Reserve_Should_Fail_Given_All_seats_are_reserved()
        // {
        //     Given(ScreeningInitialized.Generate(screening1Id, new NumberOfSeats(10)),
        //           new SeatsReserved(screening1Id, new NumberOfSeats(10)));
        //
        //     When(new ReserveSeats(screening1Id, new NumberOfSeats(1)));
        //
        //     ThenExpect(new SeatsNotReserved(screening1Id, new NumberOfSeats(1)));
        // }
    }
}