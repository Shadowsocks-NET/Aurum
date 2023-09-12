module Aurum.Configuration.Shared.Share

open Aurum.Configuration.SubscriptionAdapter.OOCv1
open FSharpPlus.Lens

type Subscriptions =
  | Base64 of url: string
  | Clash of url: string // WIP
  | OocV1 of apiToken: OocApiToken // WIP
  | SingOutbound of url: OocApiToken

module Subscriptions =
  let inline _Base64 f =
    prism'
      Base64
      (fun x ->
        match x with
        | Base64 x -> Some x
        | _ -> None)
      f

  let inline _Clash f =
    prism'
      Clash
      (fun x ->
        match x with
        | Clash x -> Some x
        | _ -> None)
      f

  let inline _OocV1 f =
    prism'
      OocV1
      (fun x ->
        match x with
        | OocV1 x -> Some x
        | _ -> None)
      f

  let inline _SingOutbound f =
    prism'
      SingOutbound
      (fun x ->
        match x with
        | SingOutbound x -> Some x
        | _ -> None)
      f
