module Infrastructure

type EventProducer<'Event> =
  'Event list -> 'Event list

type EventStore<'Event> =
  {
    Get    :  unit -> 'Event list
    Append : 'Event list -> unit
    Evolve : EventProducer<'Event> -> unit
  }

type Msg<'Event> =
  | Get of AsyncReplyChannel<'Event list>
  | Append of 'Event list
  | Evolve of EventProducer<'Event>

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

            | Evolve eventProducer ->
                let newEvents = eventProducer events
                return! loop (List.concat [ events ; newEvents ])
          }

      loop []
    )

  {
    Get    = fun () -> agent.PostAndReply(Get)
    Append = fun events -> agent.Post (Append events)
    Evolve = fun eventProducer -> agent.Post (Evolve eventProducer)
  }

