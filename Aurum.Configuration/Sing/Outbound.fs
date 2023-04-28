module Aurum.Configuration.Sing.Outbound

open System.Text.Json.Serialization

type DirectRecord =
  { tag: string

    overrideAddress: string
    overridePort: int16
    proxyProtocol: int

    detour: string option
    bindInterface: string option
    bindAddress: string option
    routingMark: int option
    reuseAddr: bool option
    connectTimeout: string option
    tcpFastOpen: bool option
    domainStrategy: string option
    fallbackDelay: string option }

type BlockRecord = { tag: string }

type ShadowsocksRecord =
  { tag: string

    server: string
    serverPort: int16
    method: string
    password: string
    network: string option
    udpOverTcp: bool option

    detour: string option
    bindInterface: string option
    bindAddress: string option
    routingMark: int option
    reuseAddr: bool option
    connectTimeout: string option
    tcpFastOpen: bool option
    domainStrategy: string option
    fallbackDelay: string option }

type VMessRecord =
  { tag: string

    server: string
    serverPort: int16
    security: string option
    uuid: string
    network: string option
    udpOverTcp: bool option

    detour: string option
    bindInterface: string option
    bindAddress: string option
    routingMark: int option
    reuseAddr: bool option
    connectTimeout: string option
    tcpFastOpen: bool option
    domainStrategy: string option
    fallbackDelay: string option }

type SelectorRecord =
  { tag: string

    outbounds: string list
    [<JsonPropertyName("default")>]
    defaultOutbound: string option }

type Outbounds =
  | Direct of DirectRecord
  | Block of BlockRecord
  | SOCKS of obj // SOCKSRecord
  | HTTP of obj //  HTTPRecord
  | Shadowsocks of ShadowsocksRecord
  | Selector of SelectorRecord
