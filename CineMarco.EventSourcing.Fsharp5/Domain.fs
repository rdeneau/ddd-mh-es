module Domain

open System

/// Generate an instance of a single case union wrapping a random `Guid`
let generate idCtor = idCtor (Guid.NewGuid())

type ClientId = ClientId of Guid
type ScreeningId = ScreeningId of Guid

type SeatNumber = SeatNumber of string
type SeatNumbers = SeatNumber list
type SeatsReservation = ClientId * ScreeningId * SeatNumbers

type ScreeningDate = DateTime

type ReservationFailure =
  | NotEnoughSeatsAvailable
  | SomeSeatsAreUnknown of SeatNumbers
  | TooClosedToScreeningTime

type DomainEvent =
  | ScreeningWasInitialized of ScreeningId * ScreeningDate * SeatNumbers
  | SeatsWereReserved of SeatsReservation
  | SeatsReservationHasFailed of SeatsReservation * ReservationFailure
  | SeatReservationHasExpired of SeatsReservation
  | SeatsWereBooked of SeatsReservation
  | SeatsBookingHasFailed of SeatsReservation
