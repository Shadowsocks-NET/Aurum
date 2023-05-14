module Aurum.Configuration.Shared.Share

open Aurum.Configuration.SubscriptionAdapter.OOCv1

type Subscriptions =
  | Base64 of url: string
  | Clash of url: string // WIP
  | OocV1 of apiToken: OocApiToken // WIP
  | SingOutbound of url: OocApiToken
