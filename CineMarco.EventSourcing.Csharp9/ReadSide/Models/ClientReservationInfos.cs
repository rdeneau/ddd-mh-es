using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.ReadSide.Models
{
    public enum ReservationStatus
    {
        Reserved,
        Expired,
        Booked,
    }

    public class ClientSeatReservationInfo
    {
        public ScreeningId ScreeningId { get; }

        public SeatNumber SeatNumber { get; }

        public DateTimeOffset ReservationDate { get; }

        public ReservationStatus ReservationStatus { get; set; } = ReservationStatus.Reserved;

        public ClientSeatReservationInfo(ScreeningId screeningId, SeatNumber seatNumber, DateTimeOffset reservationDate)
        {
            ScreeningId     = screeningId;
            SeatNumber      = seatNumber;
            ReservationDate = reservationDate;
        }
    }

    public class ClientReservationInfo
    {
        public ClientId ClientId { get; }

        public List<ClientSeatReservationInfo> Reservations { get; } = new();

        public ClientReservationInfo(ClientId clientId)
        {
            ClientId = clientId;
        }
    }

    public class ClientReservationInfos : KeyedCollection<ClientId, ClientReservationInfo>, IReadModel
    {
        protected override ClientId GetKeyForItem(ClientReservationInfo item) =>
            item.ClientId;

        public void Project(IDomainEvent @event) =>
            Apply((dynamic) @event);

        /// <summary>
        /// Fallback "Apply" method, compulsory to secure the dynamic dispatch in <see cref="Project"/>
        /// </summary>
        // ReSharper disable once UnusedParameter.Local // `_` argument
        private void Apply(IDomainEvent _) { }

        private void Apply(SeatsAreReserved @event)
        {
            if (!TryGetValue(@event.ClientId, out var clientReservationInfo))
            {
                clientReservationInfo = new ClientReservationInfo(@event.ClientId);
                Add(clientReservationInfo);
            }

            clientReservationInfo.Reservations.AddRange(
                @event.Seats.Select(
                    seatNumber => new ClientSeatReservationInfo(@event.ScreeningId, seatNumber, @event.At)));
        }

        // TODO
        // private void Apply(SeatReservationHasExpired @event)
        // {
        //     if (TryGetValue(@event.ClientId, out var screeningInfo))
        //         screeningInfo.FreeSeats(@event.Seats);
        // }
    }

}