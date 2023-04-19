module Aurum.Configuration.Sing.Outbound

open System.Text.Json.Serialization

type DirectRecord =
  { tag: string

    overrideAddress: string
    overridePort: int16
    proxyProtocol: int

    detour: string option Skippable
    bindInterface: string option Skippable
    bindAddress: string option Skippable
    routingMark: int option Skippable
    reuseAddr: bool option Skippable
    connectTimeout: string option Skippable
    tcpFastOpen: bool option Skippable
    domainStrategy: string option Skippable
    fallbackDelay: string option Skippable }

type BlockRecord = { tag: string }

type ShadowsocksRecord =
  { tag: string

    server: string
    serverPort: int16
    method: string
    password: string
    network: string option Skippable
    udpOverTcp: bool option Skippable

    detour: string option Skippable
    bindInterface: string option Skippable
    bindAddress: string option Skippable
    routingMark: int option Skippable
    reuseAddr: bool option Skippable
    connectTimeout: string option Skippable
    tcpFastOpen: bool option Skippable
    domainStrategy: string option Skippable
    fallbackDelay: string option Skippable }

type VMessRecord =
  { tag: string

    server: string
    serverPort: int16
    uuid: string
    password: string
    network: string option Skippable
    udpOverTcp: bool option Skippable

    detour: string option Skippable
    bindInterface: string option Skippable
    bindAddress: string option Skippable
    routingMark: int option Skippable
    reuseAddr: bool option Skippable
    connectTimeout: string option Skippable
    tcpFastOpen: bool option Skippable
    domainStrategy: string option Skippable
    fallbackDelay: string option Skippable }

type SelectorRecord =
  { tag: string

    outbounds: string list
    [<JsonPropertyName("default")>]
    defaultOutbound: string option Skippable }

type Outbounds =
  | Direct of DirectRecord
  | Block of BlockRecord
  | SOCKS of obj // SOCKSRecord
  | HTTP of obj //  HTTPRecord
  | Shadowsocks of ShadowsocksRecord
  | Selector of SelectorRecord
