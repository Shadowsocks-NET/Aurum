namespace Aurum.Configurator.Outbound

open FSharp.Json
open Aurum.Configurator.Transport

// reserved for future annotations.
type ShadowsocksEncryption =
    | None
    | Plain
    | [<JsonField("chacha20-poly1305")>] ChaCha20
    | [<JsonField("chacha20-ietf-poly1305")>] ChaCha20IETF
    | [<JsonField("aes-128-gcm")>] AES128
    | [<JsonField("aes-256-gcm")>] AES256

// VLESS is removed in v2ray-go, so this is subject to removal too (may happen in any time).
type VLESSEncryption = | None

type Protocol =
    | VLESS // subject to removal.
    | VMess
    | Shadowsocks
    | Trojan

// reserved for future annotations.
type VMessEncryption =
    | None
    | Zero
    | Auto
    | [<JsonField("aes-128-gcm")>] AES
    | [<JsonField("chacha20-poly1305")>] ChaCha20

type UserObject =
    { id: string
      encryption: VLESSEncryption
      level: int
      alterId: int
      security: string (* should be VMessEncryption but not supported by the Serializer/Deserializer library *)  }

type ServerObject =
    { address: string
      port: int
      password: string
      email: string option
      level: int
      method: string (* should be ShadowsocksEncryption but not supported by the Serializer/Deserializer library *)
      ivCheck: bool
      users: UserObject list }

type OutboundConfigurationObject =
    { vnext: ServerObject list
      servers: ServerObject list }

type MuxObject = { enabled: bool; concurrency: int }

type OutboundObject =
    { sendThrough: string
      protocol: Protocol
      settings: OutboundConfigurationObject
      tag: string
      streamSettings: StreamSettingsObject
      mux: MuxObject }
