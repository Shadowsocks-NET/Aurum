module Aurum.Configuration.Outbound

open Aurum.Configuration.V2fly.Transport
open Aurum

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

[<RequireQualifiedAccess>]
// VLESS is removed in v2ray-go, so this is subject to removal too (may happen in any time).
type VLESSEncryption = | [<JsonName("none")>] None

type Protocols =
    | [<JsonName("vless")>] VLESS // subject to removal.
    | [<JsonName("vmess")>] VMess
    | [<JsonName("shadowsocks")>] Shadowsocks
    | [<JsonName("trojan")>] Trojan

[<RequireQualifiedAccess>]
type VMessEncryption =
    | [<JsonName("none")>] None
    | [<JsonName("zero")>] Zero
    | [<JsonName("auto")>] Auto
    | [<JsonName("aes-128-gcm")>] AES
    | [<JsonName("chacha20-poly1305")>] ChaCha20

type UserObject =
    { ID: string
      Encryption: VLESSEncryption option
      Level: int option
      AlterId: int
      Security: VMessEncryption option }

    static member ID_ = (fun a -> a.ID), (fun b a -> { a with ID = b })

    static member Encryption_ =
        (fun a -> a.Encryption), (fun b a -> { a with Encryption = Some b })

    static member Level_ = (fun a -> a.Level), (fun b a -> { a with Level = Some b })

    static member Security_ =
        (fun a -> a.Security), (fun b a -> { a with UserObject.Security = Some b })

// v2ray-go specific implementation, removed VLESS components.
type GoUserObject =
    { ID: string
      Level: int option
      Security: VMessEncryption option }

    static member ID_ = (fun a -> a.ID), (fun b a -> { a with GoUserObject.ID = b })

    static member Level_ = (fun a -> a.Level), (fun b a -> { a with GoUserObject.Level = Some b })

    static member Security_ =
        (fun a -> a.Security), (fun b a -> { a with GoUserObject.Security = Some b })

type ServerObject =
    { Address: string
      Port: int
      Password: string option
      Email: string option
      Level: int option
      Method: ShadowsocksEncryption option
      IvCheck: bool option
      Users: UserObject list option }

    static member Address_ = (fun a -> a.Address), (fun b a -> { a with Address = b })

    static member Port_ = (fun a -> a.Port), (fun b a -> { a with Port = b })

    static member Password_ = (fun a -> a.Password), (fun b a -> { a with Password = Some b })

    static member Email_ = (fun a -> a.Email), (fun b a -> { a with Email = Some b })

    static member Level_ = (fun a -> a.Level), (fun b a -> { a with ServerObject.Level = Some b })

    static member Method_ =
        (fun a -> a.Method), (fun b a -> { a with ServerObject.Method = Some b })

    static member IvCheck_ = (fun a -> a.IvCheck), (fun b a -> { a with IvCheck = Some b })

    static member Users_ = (fun a -> a.Users), (fun b a -> { a with Users = Some b })

type OutboundConfigurationObject =
    { Vnext: ServerObject list option
      Servers: ServerObject list option }

    member this.GetServerInfo() =
        let server =
            ((Option.orElse this.Vnext this.Servers)
                  |> Option.get)[0]

        server.Address, server.Port

    static member Vnext_ = (fun a -> a.Vnext), (fun b a -> { a with Vnext = Some b })

    static member Servers_ = (fun a -> a.Servers), (fun b a -> { a with Servers = Some b })

// v2ray-go specific implementation, removed vnext layer.
type GoOutboundConfigurationObject =
    { Address: string option
      Port: int option
      Users: GoUserObject list option
      Servers: ServerObject list option }

    member this.GetServerInfo() =
        match this.Servers with
        | Some x -> x[0].Address, x[0].Port
        | None -> Option.get this.Address, Option.get this.Port

    static member Address_ =
        (fun a -> a.Address), (fun b a -> { a with GoOutboundConfigurationObject.Address = Some b })

    static member Port_ =
        (fun a -> a.Port), (fun b a -> { a with GoOutboundConfigurationObject.Port = Some b })

    static member Users_ =
        (fun a -> a.Users), (fun b a -> { a with GoOutboundConfigurationObject.Users = Some b })

    static member Servers_ =
        (fun a -> a.Servers), (fun b a -> { a with GoOutboundConfigurationObject.Servers = Some b })

type MuxObject =
    { Enabled: bool
      Concurrency: int option }

    static member Enabled_ = (fun a -> a.Enabled), (fun b a -> { a with Enabled = b })

    static member Concurrency_ =
        (fun a -> a.Concurrency), (fun b a -> { a with Concurrency = Some b })

type GenericOutboundObject<'T> =
    { SendThrough: string option
      Protocol: Protocols
      Settings: 'T
      Tag: string
      StreamSettings: StreamSettingsObject option
      Mux: MuxObject }

    static member SendThrough_ =
        (fun a -> a.SendThrough), (fun b a -> { a with SendThrough = Some b })

    static member Protocol_ = (fun a -> a.Protocol), (fun b a -> { a with Protocol = b })

    static member Settings_ = (fun a -> a.Settings), (fun b a -> { a with Settings = b })

    static member Tag_ = (fun a -> a.Tag), (fun b a -> { a with Tag = b })

    static member StreamSettings_ =
        (fun a -> a.StreamSettings), (fun b a -> { a with StreamSettings = Some b })

    static member Mux_ = (fun a -> a.Mux), (fun b a -> { a with Mux = b })

    member this.GetConnectionType() =
        let protocol =
            match this.Protocol with
            | VMess -> "VMess"
            | VLESS -> "VLESS"
            | Shadowsocks -> "Shadowsocks"
            | Trojan -> "Trojan"

        let network =
            Option.map
                (fun streamSetting ->
                    match streamSetting.Network with
                    | Networks.GRPC -> "+gRPC"
                    | Networks.HTTP -> "+HTTP2"
                    | Networks.KCP -> "+mKCP"
                    | Networks.QUIC -> "+QUIC"
                    | Networks.TCP -> "+TCP"
                    | Networks.WS -> "+WS"
                    | Networks.DomainSocket -> "+DomainSocket")
                this.StreamSettings
            |> Option.defaultValue ""

        let tlsFlag =
            Option.map
                (fun (streamSetting: StreamSettingsObject) ->
                    match streamSetting.Security with
                    | Security.None -> ""
                    | Security.TLS -> "+TLS"
                    | Security.XTLS -> "+XTLS")
                this.StreamSettings
            |> Option.defaultValue ""

        protocol + network + tlsFlag

type OutboundObject = GenericOutboundObject<OutboundConfigurationObject>
// v2ray-go specific implementation, removed vnext and VLESS.
type GoOutboundObject = GenericOutboundObject<GoOutboundConfigurationObject>


let createVMessUserObject (uuid, security, level, alterId) =
    { UserObject.ID = uuid
      Security = Some(security)
      Level = level
      AlterId = Option.defaultValue 0 alterId
      Encryption = None }

let createVMessServerObject (host, port, users) =
    { ServerObject.Address = host
      Port = port
      Users = Some users
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

let createV2flyOutboundObject (sendThrough, protocol, setting, streamSetting, tag, mux, vnext) : OutboundObject =
    { OutboundObject.SendThrough = sendThrough
      Protocol = protocol
      Settings =
        if vnext then
            { OutboundConfigurationObject.Vnext = Some [ setting ]
              Servers = None }
        else
            { OutboundConfigurationObject.Vnext = None
              Servers = Some [ setting ] }
      StreamSettings = streamSetting
      Mux =
        Option.defaultValue
            { MuxObject.Enabled = false
              Concurrency = Some 1 }
            mux
      Tag = tag }

let createShadowsocksServerObject (email, address, port, method, password, level, ivCheck) =
    { ServerObject.Email = email
      Address = address
      Port = port
      Method = method
      Password = password
      Level = level
      IvCheck = ivCheck
      Users = None }
