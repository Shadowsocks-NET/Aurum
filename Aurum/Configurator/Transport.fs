namespace Aurum.Configurator.Transport

open System.Collections.Generic
open FSharp.Json

type Networks =
    | WS
    | GRPC
    | TCP
    | KCP
    | DomainSocket
    | HTTP
    | QUIC


type Security =
    | None
    | TLS
    | XTLS

type WebSocketObject =
    { path: string
      maxEarlyData: int
      browserForwarding: bool
      earlyDataHeader: string
      headers: Dictionary<string, string> }

type HttpRequestObject =
    { version: string
      method: string
      path: string list
      headers: Dictionary<string, string list> }

type HttpResponseObject =
    { version: string
      status: string
      reason: string
      headers: Dictionary<string, string list> }

type TcpHeaderObject =
    { [<JsonField("type")>]
      headerType: string
      request: HttpRequestObject
      response: HttpResponseObject }

type TcpObject = { header: TcpHeaderObject }

type KcpHeaders =
    | None
    | SRTP
    | UTP
    | WechatVideo
    | DTLS
    | WireGuard

type UdpHeaderObject =
    { [<JsonField("type")>]
      headerType: KcpHeaders }

type KcpObject =
    { mtu: int
      tti: int
      uplinkCapacity: int
      downlinkCapacity: int
      congestion: bool
      readBufferSize: int
      writeBufferSize: int
      header: UdpHeaderObject
      seed: string }

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
    { host: string list
      path: string
      method: HTTPMethod
      headers: Dictionary<string, string list> }

type QuicSecurity =
    | None
    | [<JsonField("aes-128-gcm")>] AES
    | [<JsonField("chacha20-poly1305")>] ChaCha20

type QuicObject =
    { security: QuicSecurity
      key: string
      header: UdpHeaderObject }

type GrpcObject = { serviceName: string }

type TLSObject =
    { serverName: string
      allowInsecure: bool
      alpn: string list
      disableSystemRoot: bool }

type TProxyType =
    | Redirect
    | TProxy
    | Off

type SockoptObject =
    { mark: int
      tcpFastOpen: bool option
      tproxy: TProxyType }

type StreamSettingsObject =
    { network: Networks
      tls: TLSObject option
      tcp: TcpObject option
      kcp: KcpObject option
      ws: WebSocketObject option
      http: HttpObject option
      quic: QuicObject option
      grpc: GrpcObject option
      sockopt: SockoptObject option }
