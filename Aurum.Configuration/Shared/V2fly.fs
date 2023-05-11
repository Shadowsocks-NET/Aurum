module Aurum.Configuration.Shared.V2fly

open System.Text.Json.Serialization
open Aurum
open System.Collections.Generic

// obj is a stub object, so that the generated JSON will be an empty object instead of null or undefined or have an absent field
type TransportProtocol =
  | TCP of obj
  | KCP of KcpObject
  | WebSocket of WebSocketObject
  | HTTP of HttpObject
  | QUIC of obj
  | GRPC of GrpcObject

and WebSocketObject =
  { Path: string
    MaxEarlyData: int
    BrowserForwarding: bool
    EarlyDataHeader: string
    Headers: Dictionary<string, string> option }

  static member Path_ = (fun a -> a.Path), (fun b a -> { a with Path = b })

  static member MaxEarlyData_ =
    (fun a -> a.MaxEarlyData), (fun b a -> { a with MaxEarlyData = b })

  static member BrowserForwarding_ =
    (fun a -> a.BrowserForwarding), (fun b a -> { a with BrowserForwarding = b })

  static member EarlyDataHeader_ =
    (fun a -> a.EarlyDataHeader), (fun b a -> { a with EarlyDataHeader = b })

  static member Headers_ = (fun a -> a.Headers), (fun b a -> { a with Headers = b })

and KcpObject =
  { MTU: int
    TTI: int
    UplinkCapacity: int
    DownlinkCapacity: int
    Congestion: bool
    ReadBufferSize: int
    WriteBufferSize: int
    Seed: string option }
  static member MTU_ = (fun a -> a.MTU), (fun b a -> { a with MTU = b })

  static member TTI_ = (fun a -> a.TTI), (fun b a -> { a with TTI = b })

  static member UplinkCapacity_ =
    (fun a -> a.UplinkCapacity), (fun b a -> { a with UplinkCapacity = b })

  static member DownlinkCapacity_ =
    (fun a -> a.DownlinkCapacity), (fun b a -> { a with DownlinkCapacity = b })

  static member Congestion_ = (fun a -> a.Congestion), (fun b a -> { a with Congestion = b })

  static member ReadBufferSize_ =
    (fun a -> a.ReadBufferSize), (fun b a -> { a with ReadBufferSize = b })

  static member WriteBufferSize_ =
    (fun a -> a.WriteBufferSize), (fun b a -> { a with WriteBufferSize = b })

  static member Seed_ = (fun a -> a.Seed), (fun b a -> { a with Seed = b })

and HttpObject =
  { Host: string list
    Path: string
    Headers: Dictionary<string, string list> }

  static member Path_ = (fun a -> a.Path), (fun b a -> { a with Path = b })

  static member Host_ = (fun a -> a.Host), (fun b a -> { a with Host = b })

  static member Headers_ = (fun a -> a.Headers), (fun b a -> { a with Headers = b })

and GrpcObject =
  { ServiceName: string }

  static member ServiceName_ = (fun a -> a.ServiceName), (fun b a -> { a with ServiceName = b })

type TLSObject =
  { ServerName: string option
    AllowInsecure: bool option
    ALPN: string list option }

  static member ServerName_ = (fun a -> a.ServerName), (fun b a -> { a with ServerName = b })

  static member AllowInsecure_ =
    (fun a -> a.AllowInsecure), (fun b a -> { a with AllowInsecure = b })

  static member ALPN_ = (fun a -> a.ALPN), (fun b a -> { a with ALPN = b })

[<RequireQualifiedAccess>]
type TransportSecurity =
  | [<JsonName "none">] None
  | [<JsonName "tls">] TLS of TLSObject
  // only for reservation. V2fly and Sing backend does not have XTLS support.
  | [<JsonName "xtls">] XTLS

type StreamSettings =
  { TransportSettings: TransportProtocol
    SecuritySettings: TransportSecurity }


[<RequireQualifiedAccess>]
// VLESS deprecated and subject to removal in v2fly/v2ray-core v5
// It is not supported in sing-box
type VLESSEncryption = | [<JsonName("none")>] None

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

  static member Enabled_ = (fun a -> a.Enabled), (fun b a -> { a with Enabled = b })

  static member Concurrency_ =
    (fun a -> a.Concurrency), (fun b a -> { a with Concurrency = Some b })

type VMessObject =
  { Address: string
    Port: int
    UUID: string
    Security: VMessSecurity }

  static member Address_ = (fun a -> a.Address), (fun b a -> { a with Address = b })

  static member Port_ = (fun a -> a.Port), (fun b a -> { a with Port = b })

  static member UUID_ = (fun a -> a.UUID), (fun b a -> { a with UUID = b })

  static member Security_ = (fun a -> a.Security), (fun b a -> { a with Security = b })

  member this.GetServerInfo() = this.Address, this.Port

type Protocols =
  | [<JsonName("vless")>] VLESS
  | [<JsonName("vmess")>] VMess of VMessObject

type V2flyObject =
  { Protocol: Protocols
    StreamSettings: StreamSettings }

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

let createQuicObject () = QUIC {|  |}

let createTCPObject () = TCP {|  |}

let createTLSObject (serverName, alpn, disableSystemRoot) =
  TransportSecurity.TLS
    { TLSObject.ServerName = serverName
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
  | "none" -> VMessSecurity.None
  | "zero" -> VMessSecurity.Zero
  | "auto" -> VMessSecurity.Auto
  | "aes-128-gcm" -> VMessSecurity.AES
  | "chacha20-poly1305" -> VMessSecurity.ChaCha20
  | _ -> raise (ConfigurationParameterException "unknown security type")

let createV2flyObject protocol transportSettings securitySettings =
  { Protocol = protocol
    StreamSettings =
      { TransportSettings = transportSettings
        SecuritySettings = securitySettings } }
