namespace Aurum.Configurator

open FSharp.Json
open Aurum.Configurator.Transport

module Outbound =
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

    type Protocols =
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

    // v2ray-go specific implementation, removed VLESS components.
    type GoUserObject =
        { id: string
          level: int
          alterId: int
          security: string }

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

    // v2ray-go specific implementation, removed vnext layer.
    type GoOutboundConfigurationObject =
        { address: string
          port: int
          users: GoUserObject list
          servers: ServerObject list }

    type MuxObject = { enabled: bool; concurrency: int }

    type GenericOutboundObject<'T> =
        { sendThrough: string
          protocol: Protocols
          settings: 'T
          tag: string
          streamSettings: StreamSettingsObject
          mux: MuxObject }

    type OutboundObject = GenericOutboundObject<OutboundConfigurationObject>
    // v2ray-go specific implementation, removed vnext and VLESS.
    type GoOutboundObject = GenericOutboundObject<GoOutboundConfigurationObject>

    let createVMessOutboundObject = ()
