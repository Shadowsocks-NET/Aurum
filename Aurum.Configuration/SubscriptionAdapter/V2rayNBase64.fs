module Aurum.Configuration.SubscriptionAdapter.V2rayNBase64

open Aurum

let extractShareLinks content =
  let decodedBase64 = decodeBase64 content

  decodedBase64.Split "\n"
  |> List.ofArray
  |> List.map (fun link -> link.Substring 8)
