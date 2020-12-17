using CineMarco.EventSourcing.Csharp9.Application;
using CineMarco.EventSourcing.Csharp9.Domain;
using CineMarco.EventSourcing.Csharp9.Tests.Utils;
using CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers;
using Xunit;
using static CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers.Clients;
using static CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers.Screenings;
using static CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers.SeatNumbers;

namespace CineMarco.EventSourcing.Csharp9.Tests.Scenarios
{
    public class ReservationScenariosShould : SemanticTest
    {
        [Fact]
        public void Indicate_client_seats_reserved()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C", "D")));

            When(
                new ReserveSeats(Client1, Screening1, Seats("B", "C"), TimeStamp));

            WhenQuery(
                new ClientScreeningReservations(Client1, Screening1));

            ThenExpect(
                new ClientScreeningReservationResponse(Client1, Screening1, Seats("B", "C").Reserved(at: TimeStamp)));
        }
    }
}