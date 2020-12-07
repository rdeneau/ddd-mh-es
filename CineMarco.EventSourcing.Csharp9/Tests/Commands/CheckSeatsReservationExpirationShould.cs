using CineMarco.EventSourcing.Csharp9.Tests.Utils;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.Fixtures;
using Xunit;

namespace CineMarco.EventSourcing.Csharp9.Tests.Commands
{
    public class CheckSeatsReservationExpirationShould : SemanticTest
    {
        private readonly ScreeningReservationFixture screening = new();

        [Theory]
        [InlineData(12)]
        [InlineData(15)]
        public void Remove_reservation_expired_after(int minutesAgo)
        {
            Given(
                screening.IsInitialized(Seats.Number("A", "B")),
                screening.HasSeatsReserved("A") with { At = Occurring.Sooner(minutesAgo) });

            When(
                screening.CheckSeatsReservationExpiration("A"));

            ThenExpect(
                screening.HasSeatsReservationExpired("A"));
        }

        [Fact]
        public void Remove_reservation_expired_only_for_given_seats()
        {
            Given(
                screening.IsInitialized(Seats.Number("A", "B")),
                screening.HasSeatsReserved("A") with { At = Occurring.Sooner(minutesAgo: 15) },
                screening.HasSeatsReserved("B") with { At = Occurring.Sooner(minutesAgo: 10) });

            When(
                screening.CheckSeatsReservationExpiration("A", "B"));

            ThenExpect(
                screening.HasSeatsReservationExpired("A"));
        }
    }
}