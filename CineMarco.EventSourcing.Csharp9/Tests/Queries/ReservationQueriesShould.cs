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
        public void Indicate_available_seats_after_initialisation()
        {
            Given(
                screening.IsInitialized(Occurring.Tomorrow, Seats.Number("A", "B", "C", "D")));

            WhenQuery(
                screening.AvailableSeats());

            ThenExpect(
                screening.AvailableSeatsResponse("A", "B", "C", "D"));
        }

        [Fact]
        public void Indicate_available_seats_after_reservation()
        {
            Given(
                screening.IsInitialized(Occurring.Tomorrow, Seats.Number("A", "B", "C", "D")),
                screening.HasSeatsReserved("A", "B"));

            WhenQuery(
                screening.AvailableSeats());

            ThenExpect(
                screening.AvailableSeatsResponse("C", "D"));
        }

        [Fact]
        public void Indicate_available_seats_after_reservation_expired()
        {
            Given(
                screening.IsInitialized(Occurring.Tomorrow, Seats.Number("A", "B", "C", "D")),
                screening.HasSeatsReserved("A"),
                screening.HasSeatsReserved("B"),
                screening.HasSeatsReservationExpired("A"));

            WhenQuery(
                screening.AvailableSeats());

            ThenExpect(
                screening.AvailableSeatsResponse("A", "C", "D"));
        }
    }
}