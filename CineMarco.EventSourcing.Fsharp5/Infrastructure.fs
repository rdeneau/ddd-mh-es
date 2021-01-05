module Infrastructure

/// Wrap a business behaviour
type EventProducer<'Event> =
  'Event list -> 'Event list

type EventStore<'Event> =
  {
    Get    :  unit -> 'Event list
    Append : 'Event list -> unit

    /// Combination of `Get () |> Behaviour |> Append`
    Evolve : EventProducer<'Event> -> unit
  }

type Msg<'Event> =
  | Get of AsyncReplyChannel<'Event list>
  | Append of 'Event list

let eventStoreOf<'Event> () : EventStore<'Event> =
  let agent =
    MailboxProcessor.Start(fun inbox ->
      let rec loop events =
          async {
            let! msg = inbox.Receive()

            match msg with
            | Append newEvents ->
                return! loop (List.concat [ events ; newEvents ])

            | Get channel ->
                channel.Reply events
                return! loop events
          }

      loop []
    )

  let getEvents () =
    agent.PostAndReply(Get)

  let append events =
    agent.Post (Append events)

  let evolve behaviour =
    getEvents ()
    |> behaviour
    |> append

  {
    Get    = getEvents
    Append = append
    Evolve = evolve
  }

