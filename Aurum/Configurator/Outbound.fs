namespace Aurum.Configurator.Outbound

open FSharp.Json
open Aurum.Configurator.Transport

[<RequireQualifiedAccessAttribute>]
type ShadowsocksEncryption =
    | [<JsonUnionCase("none")>] None
    | [<JsonUnionCase("plain")>] Plain
    | [<JsonUnionCase("chacha20-poly1305")>] ChaCha20
    | [<JsonUnionCase("chacha20-ietf-poly1305")>] ChaCha20IETF
    | [<JsonUnionCase("aes-128-gcm")>] AES128
    | [<JsonUnionCase("aes-256-gcm")>] AES256

[<RequireQualifiedAccessAttribute>]
// VLESS is removed in v2ray-go, so this is subject to removal too (may happen in any time).
type VLESSEncryption = | [<JsonUnionCase("none")>] None

type Protocols =
    | [<JsonUnionCase("vless")>] VLESS // subject to removal.
    | [<JsonUnionCase("vmess")>] VMess
    | [<JsonUnionCase("shadowsocks")>] Shadowsocks
    | [<JsonUnionCase("trojan")>] Trojan

[<RequireQualifiedAccessAttribute>]
type VMessEncryption =
    | [<JsonUnionCase("none")>] None
    | [<JsonUnionCase("zero")>] Zero
    | [<JsonUnionCase("auto")>] Auto
    | [<JsonUnionCase("aes-128-gcm")>] AES
    | [<JsonUnionCase("chacha20-poly1305")>] ChaCha20

type UserObject =
    { ID: string
      Encryption: VLESSEncryption
      Level: int
      AlterId: int
      Security: VMessEncryption }

// v2ray-go specific implementation, removed VLESS components.
type GoUserObject =
    { ID: string
      Level: int
      Security: VMessEncryption }

type ServerObject =
    { Address: string
      Port: int
      Password: string
      Email: string option
      Level: int
      Method: ShadowsocksEncryption
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

