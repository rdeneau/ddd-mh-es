using System;
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
        private readonly DateTimeOffset _tenMinutesAgo     = Occurring.Sooner(minutesAgo: 10);
        private readonly DateTimeOffset _fifteenMinutesAgo = Occurring.Sooner(minutesAgo: 15);

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
        public void Indicate_screening_not_found()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C", "D")));

            WhenQuery(
                new ScreeningAvailableSeats(Screening2));

            ThenExpect(
                ScreeningAvailableSeatsResponse.NotFound(Screening2));
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
                new SeatsAreReserved(Client2, Screening1, Seats("B")),
                new SeatReservationHasExpired(Client1, Screening1, Seats("A")));

            WhenQuery(
                new ScreeningAvailableSeats(Screening1));

            ThenExpect(
                new ScreeningAvailableSeatsResponse(Screening1, Seats("A", "C", "D")));
        }

        [Fact]
        public void Indicate_client_seats_reservation_expired()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C", "D")),
                new SeatsAreReserved(Client1, Screening1, Seats("A")) { At = _fifteenMinutesAgo },
                new SeatsAreReserved(Client1, Screening1, Seats("B")) { At = _tenMinutesAgo },
                new SeatReservationHasExpired(Client1, Screening1, Seats("A")));

            WhenQuery(
                new ClientScreeningReservations(Client1, Screening1));

            ThenExpect(
                new ClientScreeningReservationResponse(Client1, Screening1, Seats("B").Reserved(_tenMinutesAgo)));
        }

        [Fact]
        public void Indicate_client_seats_reserved()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C", "D")),
                new SeatsAreReserved(Client1, Screening1, Seats("A")),
                new SeatsAreReserved(Client2, Screening1, Seats("B", "C")) { At = _tenMinutesAgo },
                new SeatsAreReserved(Client3, Screening1, Seats("D")));

            WhenQuery(
                new ClientScreeningReservations(Client2, Screening1));

            ThenExpect(
                new ClientScreeningReservationResponse(Client2, Screening1, Seats("B", "C").Reserved(_tenMinutesAgo)));
        }

        [Fact]
        public void Indicate_client_no_reservation_for_other_screening()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C", "D")),
                new ScreeningIsInitialized(Screening2, Occurring.Tomorrow, Seats("X", "Y", "Z")),
                new SeatsAreReserved(Client1, Screening1, Seats("A")));

            WhenQuery(
                new ClientScreeningReservations(Client1, Screening2));

            ThenExpect(
                ClientScreeningReservationResponse.NotFound(Client1, Screening2));
        }

        [Fact]
        public void Indicate_client_no_reservation_at_all()
        {
            Given(
                new ScreeningIsInitialized(Screening1, Occurring.Tomorrow, Seats("A", "B", "C", "D")));

            WhenQuery(
                new ClientScreeningReservations(Client1, Screening1));

            ThenExpect(
                ClientScreeningReservationResponse.NotFound(Client1, Screening1));
        }
    }
}