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
        public void Remove_reservation_expired(int minutesAgo)
        {
            Given(
                screening.IsInitialized(Occurring.Tomorrow, Seats.Number("A", "B")),
                screening.HasSeatsReserved("A") with { At = Occurring.Sooner(minutesAgo) });

            When(
                screening.CheckSeatsReservationExpiration("A"));

            ThenExpect(
                screening.HasSeatsReservationExpired("A"));
        }
    }
}