module Aurum.Configuration.Transport

open System.Collections.Generic
open Aurum
open FSharp.Json

[<RequireQualifiedAccess>]
type Networks =
    | [<JsonUnionCase("ws")>] WS
    | [<JsonUnionCase("grpc")>] GRPC
    | [<JsonUnionCase("tcp")>] TCP
    | [<JsonUnionCase("kcp")>] KCP
    | [<JsonUnionCase("domainsocket")>] DomainSocket
    | [<JsonUnionCase("http")>] HTTP
    | [<JsonUnionCase("quic")>] QUIC

[<RequireQualifiedAccess>]
type Security =
    | [<JsonUnionCase("none")>] None
    | [<JsonUnionCase("tls")>] TLS
    | [<JsonUnionCase("xtls")>] XTLS

type WebSocketObject =
    { Path: string
      MaxEarlyData: int
      BrowserForwarding: bool
      EarlyDataHeader: string
      Headers: Dictionary<string, string> option }

    static member Path_ =
        (fun a -> a.Path), (fun b a -> { a with Path = b })

    static member MaxEarlyData_ =
        (fun a -> a.MaxEarlyData), (fun b a -> { a with MaxEarlyData = b })

    static member BrowserForwarding_ =
        (fun a -> a.BrowserForwarding), (fun b a -> { a with BrowserForwarding = b })

    static member EarlyDataHeader_ =
        (fun a -> a.EarlyDataHeader), (fun b a -> { a with EarlyDataHeader = b })

    static member Headers_ =
        (fun a -> a.Headers), (fun b a -> { a with Headers = b })

type HttpRequestObject =
    { Version: string
      Method: string
      Path: string list
      Headers: Dictionary<string, string list> }

    static member Version_ =
        (fun a -> a.Version), (fun b a -> { a with Version = b })

    static member Method_ =
        (fun a -> a.Method), (fun b a -> { a with Method = b })

    static member Path_ =
        (fun a -> a.Path), (fun b a -> { a with Path = b })

    static member Headers_ =
        (fun a -> a.Headers), (fun b a -> { a with Headers = b })

type HttpResponseObject =
    { Version: string
      Status: string
      Reason: string
      Headers: Dictionary<string, string list> }

    static member Version_ =
        (fun a -> a.Version), (fun b a -> { a with Version = b })

    static member Status_ =
        (fun a -> a.Status), (fun b a -> { a with Status = b })

    static member Reason_ =
        (fun a -> a.Reason), (fun b a -> { a with Reason = b })

    static member Headers_ =
        (fun a -> a.Headers), (fun b a -> { a with Headers = b })

type TcpHeaderObject =
    { [<JsonField("type")>]
      HeaderType: string
      Request: HttpRequestObject option
      Response: HttpResponseObject option }

    static member HeaderType_ =
        (fun a -> a.HeaderType), (fun b a -> { a with HeaderType = b })

    static member Request_ =
        (fun a -> a.Request), (fun b a -> { a with Request = b })

    static member Response_ =
        (fun a -> a.Response), (fun b a -> { a with Response = b })

type TcpObject =
    { Header: TcpHeaderObject }

    static member Header_ =
        (fun a -> a.Header), (fun b a -> { a with Header = b })

[<RequireQualifiedAccess>]
type UdpHeaders =
    | [<JsonUnionCase("none")>] None
    | [<JsonUnionCase("srtp")>] SRTP
    | [<JsonUnionCase("utp")>] UTP
    | [<JsonUnionCase("wechat-video")>] WechatVideo
    | [<JsonUnionCase("dtls")>] DTLS
    | [<JsonUnionCase("wireguard")>] WireGuard

type UdpHeaderObject =
    { [<JsonField("type")>]
      HeaderType: string }

    static member HeaderType_ =
        (fun a -> a.HeaderType), (fun b a -> { a with HeaderType = b })

type KcpObject =
    { MTU: int
      TTI: int
      UplinkCapacity: int
      DownlinkCapacity: int
      Congestion: bool
      ReadBufferSize: int
      WriteBufferSize: int
      Header: UdpHeaderObject
      Seed: string option }
    static member MTU_ =
        (fun a -> a.MTU), (fun b a -> { a with MTU = b })

    static member TTI_ =
        (fun a -> a.TTI), (fun b a -> { a with TTI = b })

    static member UplinkCapacity_ =
        (fun a -> a.UplinkCapacity), (fun b a -> { a with UplinkCapacity = b })

    static member DownlinkCapacity_ =
        (fun a -> a.DownlinkCapacity), (fun b a -> { a with DownlinkCapacity = b })

    static member Congestion_ =
        (fun a -> a.Congestion), (fun b a -> { a with Congestion = b })

    static member ReadBufferSize_ =
        (fun a -> a.ReadBufferSize), (fun b a -> { a with ReadBufferSize = b })

    static member WriteBufferSize_ =
        (fun a -> a.WriteBufferSize), (fun b a -> { a with WriteBufferSize = b })

    static member Header_ =
        (fun a -> a.Header), (fun b a -> { a with KcpObject.Header = b })

    static member Seed_ =
        (fun a -> a.Seed), (fun b a -> { a with Seed = b })

[<RequireQualifiedAccess>]
type HTTPMethod =
    | GET
    | HEAD
    | POST
    | PUT
    | DELETE
    | CONNECT
    | OPTIONS
    | TRACE
    | PATCH

type HttpObject =
    { Host: string list
      Path: string
      Method: HTTPMethod
      Headers: Dictionary<string, string list> }

// reserved for future annotations.
[<RequireQualifiedAccess>]
type QuicSecurity =
    | None
    | [<JsonField("aes-128-gcm")>] AES
    | [<JsonField("chacha20-poly1305")>] ChaCha20

type QuicObject =
    { Security: string
      Key: string option
      Header: UdpHeaderObject }

type GrpcObject = { ServiceName: string; Mode: string }

type TLSObject =
    { ServerName: string option
      AllowInsecure: bool option
      Alpn: string list option
      DisableSystemRoot: bool option }

[<RequireQualifiedAccess>]
type TProxyType =
    | Redirect
    | TProxy
    | Off

type SockoptObject =
    { Mark: int
      TcpFastOpen: bool option
      Tproxy: TProxyType }

type TransportConfigurationTypes =
    | TCP of TcpObject
    | KCP of KcpObject
    | WebSocket of WebSocketObject
    | HTTP of HttpObject
    | QUIC of QuicObject
    | GRPC of GrpcObject

type StreamSettingsObject =
    { Network: Networks
      Security: Security
      Tls: TLSObject option
      Tcp: TcpObject option
      Kcp: KcpObject option
      Ws: WebSocketObject option
      Http: HttpObject option
      Quic: QuicObject option
      Grpc: GrpcObject option
      Sockopt: SockoptObject option }

let createWebSocketObject (path, maxEarlyData, browserForwarding, earlyDataHeader, host, headers) =
    let constructedHeaders =
        Option.defaultValue (Dictionary()) headers

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
    let config =
        { ServiceName = serviceName
          Mode = "gun" }

    GRPC config

let createHttpObject (path, host: string option, headers) =
    let parsedHost =
        Option.map (fun (host: string) -> host.Split "," |> Seq.toList) host
        |> Option.defaultValue []

    let config =
        { HttpObject.Path = Option.defaultValue "/" path
          Headers = headers
          Host = parsedHost
          Method = HTTPMethod.PUT }

    HTTP config

let createQUICObject (security, key, headerType) =
    match security with
    | Some "none"
    | None -> ()
    | Some "aes-128-gcm"
    | Some "chacha20-poly1305" ->
        match key with
        | Some ""
        | None -> raise (ConfigurationParameterException "no QUIC key specified")
        | _ -> ()
    | _ -> raise (ConfigurationParameterException "unknown QUIC security type")

    let header =
        { UdpHeaderObject.HeaderType = Option.defaultValue "none" headerType }

    let config =
        { QuicObject.Security = Option.defaultValue "none" security
          Key = key
          Header = header }

    QUIC config

let createKCPObject
    (
        mtu,
        tti,
        uplinkCapacity,
        downlinkCapacity,
        congestion,
        readBufferSize,
        writeBufferSize,
        seed,
        headerType
    ) =
    let header =
        { UdpHeaderObject.HeaderType = Option.defaultValue "none" headerType }

    let config =
        { KcpObject.MTU = Option.defaultValue 1350 mtu
          TTI = Option.defaultValue 20 tti
          UplinkCapacity = Option.defaultValue 5 uplinkCapacity
          DownlinkCapacity = Option.defaultValue 20 downlinkCapacity
          Congestion = Option.defaultValue false congestion
          ReadBufferSize = Option.defaultValue 2 readBufferSize
          WriteBufferSize = Option.defaultValue 2 writeBufferSize
          Seed = seed
          Header = header }

    KCP config

let createTCPObject headerObject =
    let tcpHeader =
        Option.defaultValue
            { TcpHeaderObject.HeaderType = "none"
              Request = None
              Response = None }
            headerObject

    let config = { TcpObject.Header = tcpHeader }

    TCP config

let createTLSObject (serverName, alpn, disableSystemRoot) =
    { TLSObject.ServerName = serverName
      Alpn = alpn
      AllowInsecure = Some false
      DisableSystemRoot = disableSystemRoot }

let createStreamSettingsObject (transport, security, tls) =
    match transport with
    | TCP transport ->
        { StreamSettingsObject.Network = Networks.TCP
          Tcp = Some transport
          Tls = tls
          Security = security
          Kcp = None
          Quic = None
          Ws = None
          Http = None
          Grpc = None
          Sockopt = None }
    | GRPC transport ->
        { StreamSettingsObject.Network = Networks.GRPC
          Tcp = None
          Tls = tls
          Security = security
          Kcp = None
          Quic = None
          Ws = None
          Http = None
          Grpc = Some transport
          Sockopt = None }
    | HTTP transport ->
        { StreamSettingsObject.Network = Networks.HTTP
          Tcp = None
          Tls = tls
          Security = security
          Kcp = None
          Quic = None
          Ws = None
          Http = Some transport
          Grpc = None
          Sockopt = None }
    | KCP transport ->
        { StreamSettingsObject.Network = Networks.KCP
          Tcp = None
          Tls = tls
          Security = security
          Kcp = Some transport
          Quic = None
          Ws = None
          Http = None
          Grpc = None
          Sockopt = None }
    | QUIC transport ->
        { StreamSettingsObject.Network = Networks.QUIC
          Tcp = None
          Tls = tls
          Security = security
          Kcp = None
          Quic = Some transport
          Ws = None
          Http = None
          Grpc = None
          Sockopt = None }
    | WebSocket transport ->
        { StreamSettingsObject.Network = Networks.WS
          Tcp = None
          Tls = tls
          Security = security
          Kcp = None
          Quic = None
          Ws = Some transport
          Http = None
          Grpc = None
          Sockopt = None }
