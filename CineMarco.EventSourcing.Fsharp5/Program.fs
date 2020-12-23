open System

open Domain
open Infrastructure

[<EntryPoint>]
let main _ =
  let eventStore = eventStoreOf<DomainEvent> ()

  let seats numbers =
    numbers
    |> List.map SeatNumber

  let clientId = generate ClientId
  let screeningId = generate ScreeningId
  let tomorrowAt10 = DateTime.Today.Add(TimeSpan(1, 10, 0, 0))

  eventStore.Append
    [
      ScreeningWasInitialized (screeningId, tomorrowAt10, seats ["A";"B"])
      SeatsWereReserved (clientId, screeningId, seats ["A"])
    ]

  let printEvents events =
    events
    |> List.length
    |> printfn "History (%i)"

    events
    |> List.iteri (fun i item -> printfn $" %i{i}: {item}")

  eventStore.Get()
  |> printEvents

  let exitCode = 0
  exitCode
