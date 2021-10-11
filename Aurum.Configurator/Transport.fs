namespace Aurum.Configurator

open System.Collections.Generic
open Aurum.Exceptions
open FSharp.Json

module Transport =
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

    type HttpRequestObject =
        { Version: string
          Method: string
          Path: string list
          Headers: Dictionary<string, string list> }

    type HttpResponseObject =
        { Version: string
          Status: string
          Reason: string
          Headers: Dictionary<string, string list> }

    type TcpHeaderObject =
        { [<JsonField("type")>]
          HeaderType: string
          Request: HttpRequestObject option
          Response: HttpResponseObject option }

    type TcpObject = { Header: TcpHeaderObject }

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

    let createGrpcObject (serviceName) =
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

    let createTCPObject (headerObject) =
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
