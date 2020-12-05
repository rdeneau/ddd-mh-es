using CineMarco.EventSourcing.Csharp9.Tests.Utils;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.Fixtures;
using Xunit;

namespace CineMarco.EventSourcing.Csharp9.Tests.Queries
{
    public class ReservationQueriesShould : SemanticTest
    {
        private readonly ScreeningReservationFixture screening = new();

        [Fact]
        public void Indicate_available_seats()
        {
            Given(
                screening.IsInitialized(Occurring.Tomorrow, Seats.Number("A", "B", "C", "D")),
                screening.HasSeatsReserved("A", "B"));

            WhenQuery(
                screening.AvailableSeats());

            ThenExpect(
                screening.AvailableSeatsResponse("C", "D"));
        }
    }
}