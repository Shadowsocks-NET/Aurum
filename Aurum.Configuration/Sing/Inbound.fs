module Aurum.Configuration.Sing.Inbound

open System.Text.Json.Serialization
open Aurum.Configuration.Sing.Shared

type SOCKSRecord =
  { tag: string

    listen: string
    listenPort: int16 option
    tcpFastOpen: bool option
    udpFragment: bool option
    sniff: bool option
    sniffOverrideDestination: bool option
    domainStrategy: string option
    udpTimeout: int option
    proxyProtocol: bool option
    proxyProtocolAcceptNoHeader: bool option
    detour: string option

    users: obj list }

type HTTPRecord =
  { tag: string

    listen: string
    listenPort: int16 option
    tcpFastOpen: bool option
    udpFragment: bool option
    sniff: bool option
    sniffOverrideDestination: bool option
    domainStrategy: string option
    udpTimeout: int option
    proxyProtocol: bool option
    proxyProtocolAcceptNoHeader: bool option
    detour: string option

    users: obj list option
    tls: TLSInbound option
    setSystemProxy: bool option }

type MixedRecord =
  { tag: string

    listen: string
    listenPort: int16 option
    tcpFastOpen: bool option
    udpFragment: bool option
    sniff: bool option
    sniffOverrideDestination: bool option
    domainStrategy: string option
    udpTimeout: int option
    proxyProtocol: bool option
    proxyProtocolAcceptNoHeader: bool option
    detour: string option

    users: obj list option
    setSystemProxy: bool option}

type TunStack =
  | System
  | GVisor // not included by default
  | Mixed
  | Lwip // upstream archived

type TunRecord =
  { tag: string
    interfaceName: string option
    inet4Address: string
    inet6Address: string option
    mtu: int option
    autoRoute: bool option
    strictRoute: bool option
    inet4RouteAddress: string list option
    inet6RouteAddress: string list option
    endpointIndependentNat: bool option
    udpTimeout: int option
    stack: TunStack option
    includeInterface: string list option
    excludeInterface: string list option
    includeUid: int list option
    includeUidRange: string list option
    excludeUid: int list option
    excludeUidRange: string list option
    includeAndroidUser: int list option
    includePackage: string list option
    excludePackage: string list option
    platform: obj option

    listen: string
    listenPort: int16 option
    tcpFastOpen: bool option
    udpFragment: bool option
    sniff: bool option
    sniffOverrideDestination: bool option
    domainStrategy: string option
    proxyProtocol: bool option
    proxyProtocolAcceptNoHeader: bool option
    detour: string option }
