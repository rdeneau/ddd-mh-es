using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Tests.Helpers;
using Xunit;
using static CineMarco.EventSourcing.Csharp9.Tests.Helpers.ScreeningData;

namespace CineMarco.EventSourcing.Csharp9.Tests
{
    public class ReserveSeatShould : TestBase
    {
        [Fact]
        public void Reserve_first_seat_of_the_screening()
        {
            var screening = ScreeningWith(numberOfSeats: 4);

            Given(new ScreeningInitialized(screening.Id, screening.Seats));

            When(new ReserveSeats(screening.Id, new NumberOfSeats(1)));

            ThenExpect(new SeatsReserved(screening.Id, screening.SeatsWithNumber(1)));
        }

        [Fact]
        public void Reserve_second_seat_of_the_screening()
        {
            var screening = ScreeningWith(numberOfSeats: 4);

            Given(new ScreeningInitialized(screening.Id, screening.Seats),
                  new SeatsReserved(screening.Id, screening.SeatsWithNumber(1).WithEach(x => x.Reserve())));

            When(new ReserveSeats(screening.Id, new NumberOfSeats(1)));

            ThenExpect(new SeatsReserved(screening.Id, screening.SeatsWithNumber(2)));
        }

        [Fact]
        public void Reserve_two_seats_twice()
        {
            var screening = ScreeningWith(numberOfSeats: 4);

            Given(new ScreeningInitialized(screening.Id, screening.Seats),
                  new SeatsReserved(screening.Id, screening.SeatsWithNumber(1, 2).WithEach(x => x.Reserve())));

            When(new ReserveSeats(screening.Id, new NumberOfSeats(2)));

            ThenExpect(new SeatsReserved(screening.Id, screening.SeatsWithNumber(3, 4)));
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