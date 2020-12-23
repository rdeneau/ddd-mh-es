module Infrastructure

type EventStore<'Event> =
  {
    Append : 'Event list -> unit
    Get :  unit -> 'Event list
  }

type Msg<'Event> =
  | Get of AsyncReplyChannel<'Event list>
  | Append of 'Event list

let eventStoreOf<'Event> () : EventStore<'Event> =
  let mailbox =
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

  {
    Append = fun events -> mailbox.Post (Append events)
    Get = fun () ->  mailbox.PostAndReply(Get)
  }

