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
        { ID: string
          Encryption: VLESSEncryption
          Level: int
          AlterId: int
          Security: string (* should be VMessEncryption but not supported by the Serializer/Deserializer library *)  }

    // v2ray-go specific implementation, removed VLESS components.
    type GoUserObject =
        { ID: string
          Level: int
          AlterId: int
          Security: string }

    type ServerObject =
        { Address: string
          Port: int
          Password: string
          Email: string option
          Level: int
          Method: string (* should be ShadowsocksEncryption but not supported by the Serializer/Deserializer library *)
          IvCheck: bool
          Users: UserObject list }

    type OutboundConfigurationObject =
        { Vnext: ServerObject list
          Servers: ServerObject list }

    // v2ray-go specific implementation, removed vnext layer.
    type GoOutboundConfigurationObject =
        { Address: string
          Port: int
          Users: GoUserObject list
          Servers: ServerObject list }

    type MuxObject = { Enabled: bool; Concurrency: int }

    type GenericOutboundObject<'T> =
        { SendThrough: string
          Protocol: Protocols
          Settings: 'T
          Tag: string
          StreamSettings: StreamSettingsObject
          Mux: MuxObject }

    type OutboundObject = GenericOutboundObject<OutboundConfigurationObject>
    // v2ray-go specific implementation, removed vnext and VLESS.
    type GoOutboundObject = GenericOutboundObject<GoOutboundConfigurationObject>

    let createVMessOutboundObject = ()
