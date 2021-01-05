module Business

module ListHelper =
  let intersect list1 list2 =
    Set.intersect (Set.ofList list1) (Set.ofList list2)
    |> Set.toList

module MapHelper =
  let changeMany keys mapper table =
    let mapEntry key =
      let item =
        table
        |> Map.find key
        |> mapper
      (key, item)

    keys
    |> List.map mapEntry
    |> Map.ofList

open Domain
open Projections

type Seat =
  | AvailableSeat of SeatNumber
  | ReservedSeat of SeatNumber * ClientId
  | BookedSeat of SeatNumber * ClientId

let screeningProjection: Projection<Repository<ScreeningId, Map<SeatNumber, Seat>>, DomainEvent> =
  {
    Initial = Repository.empty
    Update =
      fun repo event ->
        match event with
        | ScreeningWasInitialized (screeningId, _, seatNumbers) ->
          let seats: Map<SeatNumber, Seat> =
            seatNumbers
            |> List.map (fun num -> (num, AvailableSeat num))
            |> Map.ofList

          repo |> Repository.add screeningId seats

        | SeatsWereReserved (clientId, screeningId, reservedSeats) ->
          let reserve seat =
            match seat with
            | AvailableSeat seatNumber -> ReservedSeat (seatNumber, clientId)
            | _ -> seat // ignore silently because should not happen

          repo |> Repository.change screeningId (MapHelper.changeMany reservedSeats reserve)

        | SeatReservationHasExpired (_, screeningId, releasedSeats) ->
          let release seat =
            match seat with
            | ReservedSeat (seatNumber, _) -> AvailableSeat seatNumber
            | _ -> seat // ignore silently because should not happen

          repo |> Repository.change screeningId (MapHelper.changeMany releasedSeats release)

        | _ -> repo
  }

let reserveSeats clientId screeningId seatNumbers events =
  let screenings =
    events
    |> project screeningProjection

  let availableSeatNumbers =
    screenings
    |> Repository.find screeningId
    |> Map.toList
    |> List.filter (fun (_, seat) ->
                    match seat with
                    | AvailableSeat _ -> true
                    | _ -> false)
    |> List.map fst

  let seatsToReserved = ListHelper.intersect availableSeatNumbers seatNumbers
  if seatsToReserved.Length = seatNumbers.Length then
    [SeatsWereReserved (clientId, screeningId, seatNumbers)]
  else
    [SeatsReservationHasFailed (clientId, screeningId, seatNumbers, NotEnoughSeatsAvailable)]
