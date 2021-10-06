namespace Aurum.Configurator

open FSharp.Json
open Aurum.Configurator.Transport
open Aurum.Exceptions

module Outbound =
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
          Encryption: VLESSEncryption option
          Level: int option
          AlterId: int
          Security: VMessEncryption option }

    // v2ray-go specific implementation, removed VLESS components.
    type GoUserObject =
        { ID: string
          Level: int option
          Security: VMessEncryption }

    type ServerObject =
        { Address: string
          Port: int
          Password: string option
          Email: string option
          Level: int option
          Method: ShadowsocksEncryption option
          IvCheck: bool option
          Users: UserObject list }

    type OutboundConfigurationObject =
        { Vnext: ServerObject list option
          Servers: ServerObject list option }

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
          StreamSettings: StreamSettingsObject option
          Mux: MuxObject }

    type OutboundObject = GenericOutboundObject<OutboundConfigurationObject>
    // v2ray-go specific implementation, removed vnext and VLESS.
    type GoOutboundObject = GenericOutboundObject<GoOutboundConfigurationObject>


    let createVMessUserObject uuid security level alterId =
        { UserObject.ID = uuid
          Security = Some(security)
          Level = level
          AlterId = Option.defaultValue 0 alterId
          Encryption = None }

    let createVMessServerObject host port users =
        { ServerObject.Address = host
          Port = port
          Users = users
          Password = None
          Email = None
          Level = None
          Method = None
          IvCheck = None }

    let parseVMessSecurity security =
        let security = Option.defaultValue "auto" security
        match security with
        | "none" -> VMessEncryption.None
        | "zero" -> VMessEncryption.Zero
        | "auto" -> VMessEncryption.Auto
        | "aes-128-gcm" -> VMessEncryption.AES
        | "chacha20-poly1305" -> VMessEncryption.ChaCha20
        | _ -> raise (ConfigurationParameterException "unknown security type")

    let createV2flyOutboundObject sendThrough protocol setting streamSetting mux =
        ()
