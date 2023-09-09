module Aurum.Configuration.Sing.Outbound

open System.Text.Json.Serialization
open FSharpPlus.Lens

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


module DirectRecord =
  let inline _tag f p = f p.tag <&> fun x -> { p with tag = x }

  let inline _overrideAddress f p =
    f p.overrideAddress <&> fun x -> { p with overrideAddress = x }

  let inline _overridePort f p =
    f p.overridePort <&> fun x -> { p with overridePort = x }

  let inline _proxyProtocol f p =
    f p.proxyProtocol <&> fun x -> { p with proxyProtocol = x }

  let inline _detour f p =
    f p.detour <&> fun x -> { p with detour = x }

  let inline _bindInterface f p =
    f p.bindInterface <&> fun x -> { p with bindInterface = x }

  let inline _bindAddress f p =
    f p.bindAddress <&> fun x -> { p with bindAddress = x }

  let inline _routingMark f p =
    f p.routingMark <&> fun x -> { p with routingMark = x }

  let inline _reuseAddr f p =
    f p.reuseAddr <&> fun x -> { p with reuseAddr = x }

  let inline _connectTimeout f p =
    f p.connectTimeout <&> fun x -> { p with connectTimeout = x }

  let inline _tcpFastOpen f p =
    f p.tcpFastOpen <&> fun x -> { p with tcpFastOpen = x }

  let inline _domainStrategy f p =
    f p.domainStrategy <&> fun x -> { p with domainStrategy = x }

  let inline _fallbackDelay f p =
    f p.fallbackDelay <&> fun x -> { p with fallbackDelay = x }

type BlockRecord = { tag: string }

module BlockRecord =
  let inline _tag f p = f p.tag <&> fun x -> { p with tag = x }

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

module ShadowsocksRecord =
  let inline _tag f p = f p.tag <&> fun x -> { p with tag = x }

  let inline _server f p =
    f p.server <&> fun x -> { p with server = x }

  let inline _serverPort f p =
    f p.serverPort <&> fun x -> { p with serverPort = x }

  let inline _method f p =
    f p.method <&> fun x -> { p with method = x }

  let inline _password f p =
    f p.password <&> fun x -> { p with password = x }

  let inline _network f p =
    f p.network <&> fun x -> { p with network = x }

  let inline _udpOverTcp f p =
    f p.udpOverTcp <&> fun x -> { p with udpOverTcp = x }

  let inline _detour f p =
    f p.detour <&> fun x -> { p with detour = x }

  let inline _bindInterface f p =
    f p.bindInterface <&> fun x -> { p with bindInterface = x }

  let inline _bindAddress f p =
    f p.bindAddress <&> fun x -> { p with bindAddress = x }

  let inline _routingMark f p =
    f p.routingMark <&> fun x -> { p with routingMark = x }

  let inline _reuseAddr f p =
    f p.reuseAddr <&> fun x -> { p with reuseAddr = x }

  let inline _connectTimeout f p =
    f p.connectTimeout <&> fun x -> { p with connectTimeout = x }

  let inline _tcpFastOpen f p =
    f p.tcpFastOpen <&> fun x -> { p with tcpFastOpen = x }

  let inline _domainStrategy f p =
    f p.domainStrategy <&> fun x -> { p with domainStrategy = x }

  let inline _fallbackDelay f p =
    f p.fallbackDelay <&> fun x -> { p with fallbackDelay = x }

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

module VMessRecord =
  let inline _tag f p = f p.tag <&> fun x -> { p with tag = x }

  let inline _server f p =
    f p.server <&> fun x -> { p with server = x }

  let inline _serverPort f p =
    f p.serverPort <&> fun x -> { p with serverPort = x }

  let inline _security f p =
    f p.security <&> fun x -> { p with security = x }

  let inline _uuid f p =
    f p.uuid <&> fun x -> { p with uuid = x }

  let inline _network f p =
    f p.network <&> fun x -> { p with network = x }

  let inline _udpOverTcp f p =
    f p.udpOverTcp <&> fun x -> { p with udpOverTcp = x }

type SelectorRecord =
  { tag: string

    outbounds: string list
    [<JsonPropertyName("default")>]
    defaultOutbound: string option }

module SelectorRecord =
  let inline _tag f p = f p.tag <&> fun x -> { p with tag = x }

  let inline _outbounds f p =
    f p.outbounds <&> fun x -> { p with outbounds = x }

  let inline _defaultOutbound f p =
    f p.defaultOutbound <&> fun x -> { p with defaultOutbound = x }

type Outbounds =
  | Direct of DirectRecord
  | Block of BlockRecord
  | SOCKS of obj // SOCKSRecord
  | HTTP of obj //  HTTPRecord
  | Shadowsocks of ShadowsocksRecord
  | VMess of VMessRecord
  | Selector of SelectorRecord
