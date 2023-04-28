module Aurum.Configuration.Shared.Transport

open System.Text.Json.Serialization
open System.Collections.Generic

[<RequireQualifiedAccess>]
type TransportNetworks =
    | [<JsonName("ws")>] WS
    | [<JsonName("grpc")>] GRPC
    | [<JsonName("tcp")>] TCP // merged into http for sing-box
    | [<JsonName("kcp")>] KCP
    | [<JsonName("domainsocket")>] DomainSocket
    | [<JsonName("http")>] HTTP
    | [<JsonName("quic")>] QUIC

[<RequireQualifiedAccess>]
type TransportSecurity =
    | [<JsonName("none")>] None
    | [<JsonName("tls")>] TLS
    // only for reservation. V2fly and Sing backend does not have XTLS support.
    | [<JsonName("xtls")>] XTLS

[<RequireQualifiedAccess>]
type Security =
    | [<JsonName("none")>] None
    | [<JsonName("tls")>] TLS
    | [<JsonName("xtls")>] XTLS

type WebSocketObject =
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

type HttpRequestObject =
    { Version: string
      Method: string
      Path: string list
      Headers: Dictionary<string, string list> }

    static member Version_ = (fun a -> a.Version), (fun b a -> { a with Version = b })

    static member Method_ = (fun a -> a.Method), (fun b a -> { a with Method = b })

    static member Path_ = (fun a -> a.Path), (fun b a -> { a with Path = b })

    static member Headers_ = (fun a -> a.Headers), (fun b a -> { a with Headers = b })

type HttpResponseObject =
    { Version: string
      Status: string
      Reason: string
      Headers: Dictionary<string, string list> }

    static member Version_ = (fun a -> a.Version), (fun b a -> { a with Version = b })

    static member Status_ = (fun a -> a.Status), (fun b a -> { a with Status = b })

    static member Reason_ = (fun a -> a.Reason), (fun b a -> { a with Reason = b })

    static member Headers_ = (fun a -> a.Headers), (fun b a -> { a with Headers = b })

type TcpHeaderObject =
    { [<JsonPropertyName("type")>]
      HeaderType: string
      Request: HttpRequestObject option
      Response: HttpResponseObject option }

    static member HeaderType_ = (fun a -> a.HeaderType), (fun b a -> { a with HeaderType = b })

    static member Request_ = (fun a -> a.Request), (fun b a -> { a with Request = b })

    static member Response_ = (fun a -> a.Response), (fun b a -> { a with Response = b })

type TcpObject =
    { Header: TcpHeaderObject }

    static member Header_ = (fun a -> a.Header), (fun b a -> { a with Header = b })

[<RequireQualifiedAccess>]
type UdpHeaders =
    | [<JsonName("none")>] None
    | [<JsonName("srtp")>] SRTP
    | [<JsonName("utp")>] UTP
    | [<JsonName("wechat-video")>] WechatVideo
    | [<JsonName("dtls")>] DTLS
    | [<JsonName("wireguard")>] WireGuard

type UdpHeaderObject =
    { [<JsonPropertyName("type")>]
      HeaderType: string }

    static member HeaderType_ = (fun a -> a.HeaderType), (fun b a -> { a with HeaderType = b })

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

    static member Header_ = (fun a -> a.Header), (fun b a -> { a with KcpObject.Header = b })

    static member Seed_ = (fun a -> a.Seed), (fun b a -> { a with Seed = b })

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

    static member Path_ = (fun a -> a.Path), (fun b a -> { a with Path = b })

    static member Host_ = (fun a -> a.Host), (fun b a -> { a with Host = b })

    static member Method_ = (fun a -> a.Method), (fun b a -> { a with Method = b })

    static member Headers_ = (fun a -> a.Headers), (fun b a -> { a with Headers = b })

[<RequireQualifiedAccess>]
type QuicSecurity =
    | None
    | [<JsonName("aes-128-gcm")>] AES
    | [<JsonName("chacha20-poly1305")>] ChaCha20

type QuicObject =
    { Security: QuicSecurity
      Key: string option
      Header: UdpHeaderObject }

    static member Security_ = (fun a -> a.Security), (fun b a -> { a with Security = b })

    static member Key_ = (fun a -> a.Key), (fun b a -> { a with Key = b })

    static member Header_ = (fun a -> a.Header), (fun b a -> { a with QuicObject.Header = b })

type GrpcObject =
    { ServiceName: string
      Mode: string }

    static member ServiceName_ = (fun a -> a.ServiceName), (fun b a -> { a with ServiceName = b })

    static member Mode_ = (fun a -> a.Mode), (fun b a -> { a with Mode = b })

type TLSObject =
    { ServerName: string option
      AllowInsecure: bool option
      ALPN: string list option
      DisableSystemRoot: bool option }

    static member ServerName_ = (fun a -> a.ServerName), (fun b a -> { a with ServerName = b })

    static member AllowInsecure_ =
        (fun a -> a.AllowInsecure), (fun b a -> { a with AllowInsecure = b })

    static member ALPN_ = (fun a -> a.ALPN), (fun b a -> { a with ALPN = b })

    static member DisableSystemRoot_ =
        (fun a -> a.DisableSystemRoot), (fun b a -> { a with DisableSystemRoot = b })

[<RequireQualifiedAccess>]
type TProxyType =
    | Redirect
    | TProxy
    | Off

type SockoptObject =
    { Mark: int
      TCPFastOpen: bool option
      Tproxy: TProxyType }

    static member Mark_ = (fun a -> a.Mark), (fun b a -> { a with Mark = b })

    static member TCPFastOpen_ = (fun a -> a.TCPFastOpen), (fun b a -> { a with TCPFastOpen = b })

    static member Tproxy_ = (fun a -> a.Tproxy), (fun b a -> { a with Tproxy = b })

type TransportConfigurationTypes =
    | TCP of TcpObject
    | KCP of KcpObject
    | WebSocket of WebSocketObject
    | HTTP of HttpObject
    | QUIC of QuicObject
    | GRPC of GrpcObject

type StreamSettingsObject =
    { Network: TransportNetworks
      Security: Security
      TLS: TLSObject option
      Tcp: TcpObject option
      Kcp: KcpObject option
      Ws: WebSocketObject option
      Http: HttpObject option
      Quic: QuicObject option
      Grpc: GrpcObject option
      Sockopt: SockoptObject option }

    static member Network_ = (fun a -> a.Network), (fun b a -> { a with Network = b })

    static member Security_ = (fun a -> a.Security), (fun b a -> { a with Security = b })

    static member TLS_ = (fun a -> a.TLS), (fun b a -> { a with TLS = b })

    static member Tcp_ = (fun a -> a.Tcp), (fun b a -> { a with Tcp = b })

    static member Kcp_ = (fun a -> a.Kcp), (fun b a -> { a with Kcp = b })

    static member Ws_ = (fun a -> a.Ws), (fun b a -> { a with Ws = b })

    static member Http_ = (fun a -> a.Http), (fun b a -> { a with Http = b })

    static member Grpc_ = (fun a -> a.Grpc), (fun b a -> { a with Grpc = b })

    static member Sockopt_ = (fun a -> a.Sockopt), (fun b a -> { a with Sockopt = b })

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
    let header = { UdpHeaderObject.HeaderType = Option.defaultValue "none" headerType }

    let config =
        { QuicObject.Security = Option.defaultValue QuicSecurity.None security
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
    let header = { UdpHeaderObject.HeaderType = Option.defaultValue "none" headerType }

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
      ALPN = alpn
      AllowInsecure = Some false
      DisableSystemRoot = disableSystemRoot }

let createStreamSettingsObject (transport, security, tls) =
    match transport with
    | TCP transport ->
        { StreamSettingsObject.Network = TransportNetworks.TCP
          Tcp = Some transport
          TLS = tls
          Security = security
          Kcp = None
          Quic = None
          Ws = None
          Http = None
          Grpc = None
          Sockopt = None }
    | GRPC transport ->
        { StreamSettingsObject.Network = TransportNetworks.GRPC
          Tcp = None
          TLS = tls
          Security = security
          Kcp = None
          Quic = None
          Ws = None
          Http = None
          Grpc = Some transport
          Sockopt = None }
    | HTTP transport ->
        { StreamSettingsObject.Network = TransportNetworks.HTTP
          Tcp = None
          TLS = tls
          Security = security
          Kcp = None
          Quic = None
          Ws = None
          Http = Some transport
          Grpc = None
          Sockopt = None }
    | KCP transport ->
        { StreamSettingsObject.Network = TransportNetworks.KCP
          Tcp = None
          TLS = tls
          Security = security
          Kcp = Some transport
          Quic = None
          Ws = None
          Http = None
          Grpc = None
          Sockopt = None }
    | QUIC transport ->
        { StreamSettingsObject.Network = TransportNetworks.QUIC
          Tcp = None
          TLS = tls
          Security = security
          Kcp = None
          Quic = Some transport
          Ws = None
          Http = None
          Grpc = None
          Sockopt = None }
    | WebSocket transport ->
        { StreamSettingsObject.Network = TransportNetworks.WS
          Tcp = None
          TLS = tls
          Security = security
          Kcp = None
          Quic = None
          Ws = Some transport
          Http = None
          Grpc = None
          Sockopt = None }
