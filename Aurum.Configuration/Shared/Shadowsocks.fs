module Aurum.Configuration.Shared.Shadowsocks

open System.Text.Json.Serialization

// ShadowsocksR and legacy stream cipher are not supported, since they have serious security issues. If users really wish to use them, they could supply custom JSON config provided their backend has proper support.
// Password / PSKs are embedded in this DU's fields
[<RequireQualifiedAccess>]
type ShadowsocksEncryption =
  | [<JsonName("none")>] None
  | [<JsonName("plain")>] Plain
  | [<JsonName("chacha20-poly1305")>] ChaCha20 of Password: string
  | [<JsonName("chacha20-ietf-poly1305")>] ChaCha20IETF of Password: string
  | [<JsonName("aes-128-gcm")>] AES128 of Password: string
  | [<JsonName("aes-256-gcm")>] AES256 of Password: string
  | [<JsonName("2022-blake3-aes-128-gcm")>] AES128_2022 of PSKs: string list
  | [<JsonName("2022-blake3-aes-256-gcm")>] AES256_2022 of PSKs: string list
  | [<JsonName("2022-blake3-chacha20-poly1305")>] ChaCha20_2022 of PSKs: string list
  | [<JsonName("2022-blake3-chacha8-poly1305")>] ChaCha8_2022 of PSKs: string list

type ShadowsocksObject =
  { Host: string
    Port: int
    Encryption: ShadowsocksEncryption }

let createShadowsocksObject (host, port, encryption) =
  { Host = host
    Port = port
    Encryption = encryption }
