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
    public class CheckSeatsReservationExpirationShould : SemanticTest
    {
        [Theory]
        [InlineData(12)]
        [InlineData(15)]
        public void Remove_reservation_expired_after(int minutesAgo)
        {
            Given(
                new ScreeningHasBeenInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B")),
                new SeatsHaveBeenReserved(Client1, Screening1, Seats("A")) { At = Occurring.Sooner(minutesAgo) });

            When(
                new CheckSeatsReservationExpiration(Client1, Screening1, Seats("A")));

            ThenExpect(
                new SeatReservationHasExpired(Client1, Screening1, Seats("A")));
        }

        [Fact]
        public void Remove_reservation_expired_only_for_given_seats()
        {
            Given(
                new ScreeningHasBeenInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B")),
                new SeatsHaveBeenReserved(Client1, Screening1, Seats("A")) { At = Occurring.Sooner(minutesAgo: 15) },
                new SeatsHaveBeenReserved(Client1, Screening1, Seats("B")) { At = Occurring.Sooner(minutesAgo: 10) });

            When(
                new CheckSeatsReservationExpiration(Client1, Screening1, Seats("A", "B")));

            ThenExpect(
                new SeatReservationHasExpired(Client1, Screening1, Seats("A")));
        }
    }
}