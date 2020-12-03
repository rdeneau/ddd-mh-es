using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Tests.Helpers;
using Xunit;

namespace CineMarco.EventSourcing.Csharp9
{
    public class ReserveSeatTests : TestBase
    {
        private readonly ScreeningId screening1 = ScreeningId.Generate();

        [Fact]
        public void Reserve_First_Seats_Should_Succeed()
        {
            Given(new ScreeningInitialized(screening1, new NumberOfSeats(10)));

            When(new ReserveSeats(screening1, new NumberOfSeats(2)));

            ThenExpect(new SeatsReserved(screening1, new NumberOfSeats(2)));
        }

        [Fact]
        public void Reserve_Too_Much_Seats_Should_Fail()
        {
            Given(new ScreeningInitialized(screening1, new NumberOfSeats(10)));

            When(new ReserveSeats(screening1, new NumberOfSeats(20)));

            ThenExpect(new SeatsNotReserved(screening1, new NumberOfSeats(20)));
        }

        [Fact]
        public void Reserve_Should_Fail_Given_All_seats_are_reserved()
        {
            Given(new ScreeningInitialized(screening1, new NumberOfSeats(10)),
                  new SeatsReserved(screening1, new NumberOfSeats(10)));

            When(new ReserveSeats(screening1, new NumberOfSeats(1)));

            ThenExpect(new SeatsNotReserved(screening1, new NumberOfSeats(1)));
        }
    }
}