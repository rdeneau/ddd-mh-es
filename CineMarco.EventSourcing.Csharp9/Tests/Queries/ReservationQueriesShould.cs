using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Tests.Utils;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers;
using Xunit;
using static CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers.Clients;
using static CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers.Screenings;
using static CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers.SeatNumbers;

namespace CineMarco.EventSourcing.Csharp9.Tests.Queries
{
    public class ReservationQueriesShould : SemanticTest
    {
        [Fact]
        public void Indicate_available_seats_after_initialisation()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C", "D")));

            WhenQuery(
                new ScreeningAvailableSeats(Screening1));

            ThenExpect(
                new ScreeningAvailableSeatsResponse(Screening1, Seats("A", "B", "C", "D")));
        }

        [Fact]
        public void Indicate_available_seats_after_reservation()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C", "D")),
                new SeatsAreReserved(Client1, Screening1, Seats("A", "B")));

            WhenQuery(
                new ScreeningAvailableSeats(Screening1));

            ThenExpect(
                new ScreeningAvailableSeatsResponse(Screening1, Seats("C", "D")));
        }

        [Fact]
        public void Indicate_available_seats_after_reservation_expired()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C", "D")),
                new SeatsAreReserved(Client1, Screening1, Seats("A")),
                new SeatsAreReserved(Client1, Screening1, Seats("B")),
                new SeatReservationHasExpired(Screening1, Seats("A")));

            WhenQuery(
                new ScreeningAvailableSeats(Screening1));

            ThenExpect(
                new ScreeningAvailableSeatsResponse(Screening1, Seats("A", "C", "D")));
        }

        [Fact]
        public void Indicate_client_seats_reserved()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C", "D")),
                new SeatsAreReserved(Client2, Screening1, Seats("A", "C")));

            WhenQuery(
                new ClientSeatReservation(Client2, Screening1));

            ThenExpect(
                new ClientSeatReservationResponse(Client2, Screening1, Seats("A", "C")));
        }
    }
}