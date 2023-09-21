module Aurum.Configuration.Shared.V2fly

open System.Text.Json.Serialization
open Aurum
open System.Collections.Generic
open FSharpPlus.Lens

type HttpVersion =
  | [<JsonName "http/1.1">] HTTP1
  | [<JsonName "h2">] HTTP2
  | [<JsonName "h3">] HTTP3

type TransportProtocol =
  | KCP of KcpObject
  | WebSocket of WebSocketObject
  | HTTP of HttpObject
  | QUIC
  | GRPC of GrpcObject

and WebSocketObject =
  { Path: string
    MaxEarlyData: int
    BrowserForwarding: bool
    EarlyDataHeader: string
    Headers: Dictionary<string, string> option }

and KcpObject =
  { MTU: int
    TTI: int
    UplinkCapacity: int
    DownlinkCapacity: int
    Congestion: bool
    ReadBufferSize: int
    WriteBufferSize: int
    Seed: string option }

and HttpObject =
  { Host: string list
    Path: string
    Version: HttpVersion
    Headers: Dictionary<string, string list> option }

and GrpcObject = { ServiceName: string }

module WebSocketObject =
  let inline _Path f (p: WebSocketObject) =
    f p.Path <&> fun x -> { p with Path = x }

  let inline _MaxEarlyData f (p: WebSocketObject) =
    f p.MaxEarlyData <&> fun x -> { p with MaxEarlyData = x }

  let inline _BrowserForwarding f (p: WebSocketObject) =
    f p.BrowserForwarding <&> fun x -> { p with BrowserForwarding = x }

  let inline _EarlyDataHeader f (p: WebSocketObject) =
    f p.EarlyDataHeader <&> fun x -> { p with EarlyDataHeader = x }

  let inline _Headers f (p: WebSocketObject) =
    f p.Headers <&> fun x -> { p with Headers = x }

module KcpObject =
  let inline _MTU f (p: KcpObject) = f p.MTU <&> fun x -> { p with MTU = x }

  let inline _TTI f (p: KcpObject) = f p.TTI <&> fun x -> { p with TTI = x }

  let inline _UplinkCapacity f (p: KcpObject) =
    f p.UplinkCapacity <&> fun x -> { p with UplinkCapacity = x }

  let inline _DownlinkCapacity f (p: KcpObject) =
    f p.DownlinkCapacity <&> fun x -> { p with DownlinkCapacity = x }

  let inline _Congestion f (p: KcpObject) =
    f p.Congestion <&> fun x -> { p with Congestion = x }

  let inline _ReadBufferSize f (p: KcpObject) =
    f p.ReadBufferSize <&> fun x -> { p with ReadBufferSize = x }

  let inline _WriteBufferSize f (p: KcpObject) =
    f p.WriteBufferSize <&> fun x -> { p with WriteBufferSize = x }

  let inline _Seed f (p: KcpObject) =
    f p.Seed <&> fun x -> { p with Seed = x }

module HttpObject =
  let inline _Host f (p: HttpObject) =
    f p.Host <&> fun x -> { p with Host = x }

  let inline _Path f (p: HttpObject) =
    f p.Path <&> fun x -> { p with Path = x }

  let inline _Headers f (p: HttpObject) =
    f p.Headers <&> fun x -> { p with Headers = x }

module GrpcObject =
  let inline _ServiceName f (p: GrpcObject) =
    f p.ServiceName <&> fun x -> { p with ServiceName = x }

module TransportProtocol =
  let inline _WebSocket f =
    prism'
      WebSocket
      (fun x ->
        match x with
        | WebSocket x -> Some(WebSocket x)
        | _ -> None)
      f

  let inline _KCP f =
    prism'
      KCP
      (fun x ->
        match x with
        | KCP x -> Some(KCP x)
        | _ -> None)
      f

  let inline _HTTP f =
    prism'
      HTTP
      (fun x ->
        match x with
        | HTTP x -> Some(HTTP x)
        | _ -> None)
      f

  let inline _QUIC f =
    prism'
      (fun _ -> QUIC)
      (fun x ->
        match x with
        | QUIC -> Some QUIC
        | _ -> None)
      f

  let inline _GRPC f =
    prism'
      GRPC
      (fun x ->
        match x with
        | GRPC x -> Some(GRPC x)
        | _ -> None)
      f

type TlsObject =
  { ServerName: string option
    AllowInsecure: bool option
    ALPN: string list option }

module TlsObject =
  let inline _ServerName f (p: TlsObject) =
    f p.ServerName <&> fun x -> { p with ServerName = x }

  let inline _AllowInsecure f (p: TlsObject) =
    f p.AllowInsecure <&> fun x -> { p with AllowInsecure = x }

  let inline _ALPN f (p: TlsObject) =
    f p.ALPN <&> fun x -> { p with ALPN = x }

[<RequireQualifiedAccess>]
type TransportSecurity =
  | [<JsonName "none">] None
  | [<JsonName "tls">] TLS of TlsObject
  // only for reservation. V2fly and Sing backend does not have XTLS support.
  | [<JsonName "xtls">] XTLS

module TransportSecurity =
  let inline _None f =
    prism'
      (fun _ -> TransportSecurity.None)
      (fun x ->
        match x with
        | TransportSecurity.None -> Some TransportSecurity.None
        | _ -> None)
      f

  let inline _TLS f =
    prism'
      TransportSecurity.TLS
      (fun x ->
        match x with
        | TransportSecurity.TLS x -> Some(TransportSecurity.TLS x)
        | _ -> None)
      f

  let inline _XTLS f =
    prism'
      (fun _ -> TransportSecurity.XTLS)
      (fun x ->
        match x with
        | TransportSecurity.XTLS -> Some TransportSecurity.XTLS
        | _ -> None)
      f

type StreamSettings =
  { TransportSettings: TransportProtocol
    SecuritySettings: TransportSecurity }

module StreamSettings =
  let inline _TransportSettings f (p: StreamSettings) =
    f p.TransportSettings <&> fun x -> { p with TransportSettings = x }

  let inline _SecuritySettings f (p: StreamSettings) =
    f p.SecuritySettings <&> fun x -> { p with SecuritySettings = x }

[<RequireQualifiedAccess>]
// VLESS deprecated and subject to removal in v2fly/v2ray-core v5
// It is not supported in sing-box
type VlessEncryption = | [<JsonName("none")>] None

module VlessEncryption =
  let inline _None f =
    prism'
      (fun _ -> VlessEncryption.None)
      (fun x ->
        match x with
        | VlessEncryption.None -> Some VlessEncryption.None
        | _ -> None)
      f

// V2fly v5 config does not support it yet
// The core retains its support, though, and sing-box also supports it
[<RequireQualifiedAccess>]
type VMessSecurity =
  | [<JsonName("none")>] None
  | [<JsonName("zero")>] Zero
  | [<JsonName("auto")>] Auto
  | [<JsonName("aes-128-gcm")>] AES
  | [<JsonName("chacha20-poly1305")>] ChaCha20

type MuxObject =
  { Enabled: bool
    Concurrency: int option }

module MuxObject =
  let inline _Enabled f (p: MuxObject) =
    f p.Enabled <&> fun x -> { p with Enabled = x }

  let inline _Concurrency f (p: MuxObject) =
    f p.Concurrency <&> fun x -> { p with Concurrency = x }

type VMessObject =
  { Address: string
    Port: int
    UUID: string
    Security: VMessSecurity }

  member this.GetServerInfo() = this.Address, this.Port

module VMessObject =
  let inline _Address f (p: VMessObject) =
    f p.Address <&> fun x -> { p with Address = x }

  let inline _Port f (p: VMessObject) =
    f p.Port <&> fun x -> { p with Port = x }

  let inline _UUID f (p: VMessObject) =
    f p.UUID <&> fun x -> { p with UUID = x }

  let inline _Security f (p: VMessObject) =
    f p.Security <&> fun x -> { p with Security = x }

type Protocols =
  | [<JsonName("vless")>] VLESS
  | [<JsonName("vmess")>] VMess of VMessObject

module Protocols =
  let inline _VLESS f =
    prism'
      (fun _ -> Protocols.VLESS)
      (fun x ->
        match x with
        | Protocols.VLESS -> Some Protocols.VLESS
        | _ -> None)
      f

  let inline _VMess f =
    prism'
      Protocols.VMess
      (fun x ->
        match x with
        | Protocols.VMess x -> Some(Protocols.VMess x)
        | _ -> None)
      f

type V2flyObject =
  { Protocol: Protocols
    StreamSettings: StreamSettings }

module V2flyObject =
  let inline _Protocol f (p: V2flyObject) =
    f p.Protocol <&> fun x -> { p with Protocol = x }

  let inline _StreamSettings f (p: V2flyObject) =
    f p.StreamSettings <&> fun x -> { p with StreamSettings = x }

let createWebSocketObject (path, maxEarlyData, browserForwarding, earlyDataHeader, host, headers) =
  let constructedHeaders = Option.defaultValue (Dictionary()) headers

  match host with
  | Some host -> constructedHeaders.Add("Host", host)
  | None -> ()

  let config =
    { WebSocketObject.Path = Option.defaultValue "/" path
      MaxEarlyData = Option.defaultValue 0 maxEarlyData
      BrowserForwarding = Option.defaultValue false browserForwarding
      EarlyDataHeader = Option.defaultValue "" earlyDataHeader
      Headers = Some(constructedHeaders) }

  WebSocket config

let createGrpcObject serviceName =
  let config = { ServiceName = serviceName }

  GRPC config

let createHttpObject (path, host: string option, headers) =
  let parsedHost =
    Option.map (fun (host: string) -> host.Split "," |> Seq.toList) host
    |> Option.defaultValue []

  let config =
    { HttpObject.Path = Option.defaultValue "/" path
      Headers = headers
      Host = parsedHost }

  HTTP config

let createKCPObject (mtu, tti, uplinkCapacity, downlinkCapacity, congestion, readBufferSize, writeBufferSize, seed) =

  let config =
    { KcpObject.MTU = Option.defaultValue 1350 mtu
      TTI = Option.defaultValue 20 tti
      UplinkCapacity = Option.defaultValue 5 uplinkCapacity
      DownlinkCapacity = Option.defaultValue 20 downlinkCapacity
      Congestion = Option.defaultValue false congestion
      ReadBufferSize = Option.defaultValue 2 readBufferSize
      WriteBufferSize = Option.defaultValue 2 writeBufferSize
      Seed = seed }

  KCP config

let createQuicObject () = QUIC

let createTLSObject (serverName, alpn) =
  TransportSecurity.TLS
    { TlsObject.ServerName = serverName
      ALPN = alpn
      AllowInsecure = Some false }

let createVMessObject (host, port, uuid, security) =
  VMess
    { VMessObject.Address = host
      Port = port
      UUID = uuid
      Security = security }

let parseVMessSecurity security =
  let security = Option.defaultValue "auto" security

  match security with
  | "none" -> Ok VMessSecurity.None
  | "zero" -> Ok VMessSecurity.Zero
  | "auto" -> Ok VMessSecurity.Auto
  | "aes-128-gcm" -> Ok VMessSecurity.AES
  | "chacha20-poly1305" -> Ok VMessSecurity.ChaCha20
  | _ -> Error(ConfigurationParameterException "unknown security type")

let createV2flyObject protocol transportSettings securitySettings =
  { Protocol = protocol
    StreamSettings =
      { TransportSettings = transportSettings
        SecuritySettings = securitySettings } }
