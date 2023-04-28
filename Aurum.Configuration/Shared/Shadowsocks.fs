module Aurum.Configuration.Shared.Shadowsocks

open System.Text.Json.Serialization

[<RequireQualifiedAccess>]
type ShadowsocksEncryption =
    | [<JsonName("none")>] None
    | [<JsonName("plain")>] Plain
    | [<JsonName("chacha20-poly1305")>] ChaCha20
    | [<JsonName("chacha20-ietf-poly1305")>] ChaCha20IETF
    | [<JsonName("aes-128-gcm")>] AES128
    | [<JsonName("aes-256-gcm")>] AES256
    | [<JsonName("2022-blake3-aes-128-gcm")>] AES128_2022
    | [<JsonName("2022-blake3-aes-256-gcm")>] AES256_2022
    | [<JsonName("2022-blake3-chacha20-poly1305")>] ChaCha20_2022
    | [<JsonName("2022-blake3-chacha8-poly1305")>] ChaCha8_2022
