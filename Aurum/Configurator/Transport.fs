namespace Aurum.Configurator

open System.Collections.Generic
open Aurum
open FSharp.Json

module Transport =
    [<RequireQualifiedAccess>]
    type Networks =
        | WS
        | GRPC
        | TCP
        | KCP
        | DomainSocket
        | HTTP
        | QUIC

    [<RequireQualifiedAccess>]
    type Security =
        | None
        | TLS
        | XTLS

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
          Request: HttpRequestObject
          Response: HttpResponseObject }

    type TcpObject = { Header: TcpHeaderObject }

    [<RequireQualifiedAccess>]
    type UdpHeaders =
        | None
        | SRTP
        | UTP
        | WechatVideo
        | DTLS
        | WireGuard

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
          Seed: string }

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
        { ServerName: string
          AllowInsecure: bool
          Alpn: string list
          DisableSystemRoot: bool }

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
          Tls: TLSObject option
          Tcp: TcpObject option
          Kcp: KcpObject option
          Ws: WebSocketObject option
          Http: HttpObject option
          Quic: QuicObject option
          Grpc: GrpcObject option
          Sockopt: SockoptObject option }

    let createWebSocketObject path maxEarlyData browserForwarding earlyDataHeader host headers =
        let constructedHeaders =
            match headers with
            | Some header -> header
            | None -> Dictionary<string, string>()

        match host with
        | Some host -> constructedHeaders.Add("Host", host)
        | None -> ()

        let config =
            { WebSocketObject.Path =
                  match path with
                  | None -> "/"
                  | Some path -> path
              MaxEarlyData =
                  match maxEarlyData with
                  | None -> 0
                  | Some maxEarlyData -> maxEarlyData
              BrowserForwarding =
                  match browserForwarding with
                  | None -> false
                  | Some browserForwarding -> browserForwarding
              EarlyDataHeader =
                  match earlyDataHeader with
                  | None -> ""
                  | Some earlyDataHeader -> earlyDataHeader
              Headers = Some(constructedHeaders) }

        WebSocket config

    let createGrpcObject serviceName =
        let config =
            { ServiceName = serviceName
              Mode = "gun" }

        GRPC config

    let createHttpObject path (host: string option) headers =
        let parsedHost =
            match host with
            | Some host -> host.Split "," |> Seq.toList
            | None -> []

        let config =
            { HttpObject.Path =
                  match path with
                  | None -> "/"
                  | Some path -> path
              Headers = headers
              Host = parsedHost
              Method = HTTPMethod.PUT }

        HTTP config

    let createQUICObject security key headerType =
        match security with
        | Some "none"
        | None -> ()
        | Some "aes-128-gcm"
        | Some "chacha20-poly1305" ->
            match key with
            | Some ""
            | None -> raise (Exceptions.ConfigurationParameterError "no QUIC key specified")
            | _ -> ()
        | _ -> raise (Exceptions.ConfigurationParameterError "unknown QUIC security type")

        let header =
            { UdpHeaderObject.HeaderType =
                  match headerType with
                  | None -> "none"
                  | Some headerType -> headerType }

        let config =
            { QuicObject.Security =
                  match security with
                  | None -> "none"
                  | Some security -> security
              Key = key
              Header = header }

        QUIC config
