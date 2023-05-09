module Aurum.Configuration.Shared.Shadowsocks

open System.Text.Json.Serialization

// ShadowsocksR and legacy stream cipher are not supported, since they have serious security issues. If users really wish to use them, they could supply custom JSON config provided their backend has proper support.
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

type ShadowsocksObject =
  { Host: string
    Port: string
    Encryption: ShadowsocksEncryption
    Password: string }

let createShadowsocksObject (host, port, encryption, password) =
  { Host = host
    Port = port
    Encryption = encryption
    Password = password }
