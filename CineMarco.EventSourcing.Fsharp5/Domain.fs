module Domain

open System

/// Generate an instance of a single case union wrapping a random `Guid`
let generate idCtor = idCtor (Guid.NewGuid())

type ClientId = ClientId of Guid
type ScreeningId = ScreeningId of Guid

type SeatNumber = SeatNumber of string
type SeatNumbers = SeatNumber list

type ScreeningDate = DateTime

type ReservationFailure =
  | NotEnoughSeatsAvailable
  | SomeSeatsAreUnknown of SeatNumbers
  | TooClosedToScreeningTime

type DomainEvent =
  | ScreeningWasInitialized of ScreeningId * ScreeningDate * SeatNumbers
  | SeatsWereReserved of ClientId * ScreeningId * SeatNumbers
  | SeatsReservationHasFailed of ClientId * ScreeningId * SeatNumbers * ReservationFailure
  | SeatReservationHasExpired of ClientId * ScreeningId * SeatNumbers
  | SeatsWereBooked of ClientId * ScreeningId * SeatNumbers
  | SeatsBookingHasFailed of ClientId * ScreeningId * SeatNumbers
