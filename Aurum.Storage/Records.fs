module Aurum.Storage.Records

type SubscriptionType =
    | None = 0
    | Base64 = 1 // not suggested
    | SIP008 = 2
    | OOCv1 = 3

type GroupObject =
    { Id: string
      Name: string
      Subscription: SubscriptionType
      SubscriptionSource: string
      Connections: string list (* stores id of connections belong to this group *)  }

    static member Name_ =
        (fun a -> a.Name), (fun b a -> { a with Name = b })

    static member Subscription_ =
        (fun a -> a.Subscription), (fun b a -> { a with Subscription = b })

    static member SubscriptionSource_ =
        (fun a -> a.SubscriptionSource), (fun b a -> { a with SubscriptionSource = b })
