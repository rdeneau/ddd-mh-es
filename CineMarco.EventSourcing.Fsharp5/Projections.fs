module Projections

  open Domain

  type Projection<'State, 'Event> =
    {
      Initial : 'State
      Update  : 'State -> 'Event -> 'State
    }

  let project (projection: Projection<_,_>) events =
    events
    |> List.fold projection.Update projection.Initial

  let availableSeatsByScreening: Projection<Map<ScreeningId, SeatNumbers>, DomainEvent> =
    {
      Initial = Map.empty
      Update  =
        fun state event ->
          let get screeningId =
            state
            |> Map.find screeningId

          let set screeningId seats =
            state
            |> Map.add screeningId seats

          match event with
          | ScreeningWasInitialized (screeningId, _, seats) ->
            seats
            |> set screeningId

          | SeatsWereReserved (_, screeningId, reservedSeats) ->
            get screeningId
            |> List.except reservedSeats
            |> set screeningId

          | SeatReservationHasExpired (_, screeningId, releasedSeats) ->
            get screeningId
            |> List.append releasedSeats
            |> set screeningId

          | _ -> state
    }