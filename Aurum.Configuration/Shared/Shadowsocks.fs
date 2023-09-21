module Aurum.Configuration.Shared.Shadowsocks

open System.Text.Json.Serialization
open FSharpPlus.Lens
open FSharpPlus.Data

// ShadowsocksR and legacy stream cipher are not supported, since they have serious security issues. If users really wish to use them, they could supply custom JSON config provided their backend has proper support.
// Password / PSKs are embedded in this DU's fields
[<RequireQualifiedAccess>]
type ShadowsocksEncryption =
  | [<JsonName("none")>] None
  | [<JsonName("plain")>] Plain
  | [<JsonName("chacha20-poly1305")>] ChaCha20 of Password: string
  | [<JsonName("chacha20-ietf-poly1305")>] ChaCha20Ietf of Password: string
  | [<JsonName("aes-128-gcm")>] AES128 of Password: string
  | [<JsonName("aes-256-gcm")>] AES256 of Password: string
  | [<JsonName("2022-blake3-aes-128-gcm")>] AES128_2022 of PSKs: string list
  | [<JsonName("2022-blake3-aes-256-gcm")>] AES256_2022 of PSKs: string list
  | [<JsonName("2022-blake3-chacha20-poly1305")>] ChaCha20_2022 of PSKs: string list
  | [<JsonName("2022-blake3-chacha8-poly1305")>] ChaCha8_2022 of PSKs: string list

module ShadowsocksEncryption =
  let inline _None f p =
    prism'
      (fun _ -> ShadowsocksEncryption.None)
      (fun x ->
        match x with
        | ShadowsocksEncryption.None -> Some ShadowsocksEncryption.None
        | _ -> None)
      f
      p

  let inline _Plain f p =
    prism'
      (fun _ -> ShadowsocksEncryption.Plain)
      (fun x ->
        match x with
        | ShadowsocksEncryption.Plain -> Some ShadowsocksEncryption.Plain
        | _ -> None)
      f
      p

  let inline _ChaCha20 f p =
    prism'
      ShadowsocksEncryption.ChaCha20
      (fun x ->
        match x with
        | ShadowsocksEncryption.ChaCha20 x -> Some(ShadowsocksEncryption.ChaCha20 x)
        | _ -> None)
      f
      p

  let inline _ChaCha20Ietf f p =
    prism'
      ShadowsocksEncryption.ChaCha20Ietf
      (fun x ->
        match x with
        | ShadowsocksEncryption.ChaCha20Ietf x -> Some(ShadowsocksEncryption.ChaCha20Ietf x)
        | _ -> None)
      f
      p

  let inline _AES128 f p =
    prism'
      ShadowsocksEncryption.AES128
      (fun x ->
        match x with
        | ShadowsocksEncryption.AES128 x -> Some(ShadowsocksEncryption.AES128 x)
        | _ -> None)
      f
      p

  let inline _AES256 f p =
    prism'
      ShadowsocksEncryption.AES256
      (fun x ->
        match x with
        | ShadowsocksEncryption.AES256 x -> Some(ShadowsocksEncryption.AES256 x)
        | _ -> None)
      f
      p

  let inline _AES128_2022 f p =
    prism'
      ShadowsocksEncryption.AES128_2022
      (fun x ->
        match x with
        | ShadowsocksEncryption.AES128_2022 x -> Some(ShadowsocksEncryption.AES128_2022 x)
        | _ -> None)
      f
      p

  let inline _AES256_2022 f p =
    prism'
      ShadowsocksEncryption.AES256_2022
      (fun x ->
        match x with
        | ShadowsocksEncryption.AES256_2022 x -> Some(ShadowsocksEncryption.AES256_2022 x)
        | _ -> None)
      f
      p

  let inline _ChaCha20_2022 f p =
    prism'
      ShadowsocksEncryption.ChaCha20_2022
      (fun x ->
        match x with
        | ShadowsocksEncryption.ChaCha20_2022 x -> Some(ShadowsocksEncryption.ChaCha20_2022 x)
        | _ -> None)
      f
      p

  let inline _ChaCha8_2022 f p =
    prism'
      ShadowsocksEncryption.ChaCha8_2022
      (fun x ->
        match x with
        | ShadowsocksEncryption.ChaCha8_2022 x -> Some(ShadowsocksEncryption.ChaCha8_2022 x)
        | _ -> None)
      f
      p

type ShadowsocksPlugin =
  | SimpleObfs of option: string
  | V2ray of option: string

module ShadowsocksPlugin =
  let inline _SimpleObfs f p =
    prism'
      ShadowsocksPlugin.SimpleObfs
      (fun x ->
        match x with
        | ShadowsocksPlugin.SimpleObfs x -> Some(ShadowsocksPlugin.SimpleObfs x)
        | _ -> None)
      f
      p

  let inline _V2ray f p =
    prism'
      ShadowsocksPlugin.V2ray
      (fun x ->
        match x with
        | ShadowsocksPlugin.V2ray x -> Some(ShadowsocksPlugin.V2ray x)
        | _ -> None)
      f
      p

type ShadowsocksObject =
  { Host: string
    Port: int
    Encryption: ShadowsocksEncryption
    Plugin: ShadowsocksPlugin option }

module ShadowsocksObject =
  let inline _Host f p =
    f p.Host <&> fun x -> { p with Host = x }

  let inline _Port f p =
    f p.Port <&> fun x -> { p with Port = x }

  let inline _Encryption f p =
    f p.Encryption <&> fun x -> { p with Encryption = x }

  let inline _Plugin f p =
    f p.Plugin <&> fun x -> { p with Plugin = x }

let createShadowsocksObject host port encryption plugin =
  { Host = host
    Port = port
    Encryption = encryption
    Plugin = plugin }
