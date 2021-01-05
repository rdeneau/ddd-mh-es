open System

open Business
open Domain
open Infrastructure

module PrintHelpers =
  open Projections

  let printEvents events =
    events
    |> List.length
    |> printfn "History (%i events)"

    events
    |> List.iteri (fun i item -> printfn $" {i}: {item}")

  let printAvailableSeatsOf screeningId events =
    printfn $"Available seats for {screeningId}: "

    events
    |> project availableSeatsProjection
    |> Repository.find screeningId
    |> printfn " %A"

open PrintHelpers

[<EntryPoint>]
let main _ =
  let eventStore = eventStoreOf<DomainEvent>()

  let seats numbers =
    numbers
    |> List.map SeatNumber

  let clientId = generate ClientId
  let screeningId = generate ScreeningId
  let tomorrowAt10 = DateTime.Today.Add(TimeSpan(1, 10, 0, 0))

  eventStore.Append
    [
      ScreeningWasInitialized (screeningId, tomorrowAt10, seats ["A";"B"])
      // SeatsWereReserved (clientId, screeningId, seats ["A"])
    ]

  reserveSeats clientId screeningId (seats ["A"])
  |> eventStore.Evolve

  let events = eventStore.Get()

  events
  |> printEvents

  events
  |> printAvailableSeatsOf screeningId

  //Exit
  0
