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

  type Repository<'k, 'v> when 'k : comparison =
    private Repository of Map<'k, 'v>

  module Repository =
    let empty = Map.empty |> Repository

    let private innerMap repository =
      let (Repository map) = repository
      map

    let find key repository =
      repository
      |> innerMap
      |> Map.find key

    let add key value repository =
      repository
      |> innerMap
      |> Map.add key value
      |> Repository

    let mapValue (key: 'k) (mapping: 'v -> 'v) (repository: Repository<'k, 'v>) =
      let mappedValue =
        repository
        |> find key
        |> mapping

      repository
      |> add key mappedValue

  let availableSeatsProjection: Projection<Repository<ScreeningId, SeatNumbers>, DomainEvent> =
    {
      Initial = Repository.empty
      Update =
        fun repo event ->
          match event with
          | ScreeningWasInitialized (screeningId, _, seats) ->
            repo |> Repository.add screeningId seats

          | SeatsWereReserved (_, screeningId, reservedSeats) ->
            repo |> Repository.mapValue screeningId (List.except reservedSeats)

          | SeatReservationHasExpired (_, screeningId, releasedSeats) ->
            repo |> Repository.mapValue screeningId (List.append releasedSeats)

          | _ -> repo
    }
