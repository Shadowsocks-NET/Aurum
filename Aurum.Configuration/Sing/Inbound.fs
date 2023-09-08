module Aurum.Configuration.Sing.Inbound

open Aurum.Configuration.Sing.Shared
open FSharpPlus.Lens

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

module SOCKSRecord =
  let inline _tag f p =
    f p.tag <&> fun x -> { p with tag = x }

  let inline _listen f p =
    f p.listen <&> fun x -> { p with listen = x}

  let inline _listenPort f p =
    f p.listenPort <&> fun x -> { p with listenPort = x }

  let inline _tcpFastOpen f p =
    f p.tcpFastOpen <&> fun x -> { p with tcpFastOpen = x }

  let inline _udpFragment f p =
    f p.udpFragment <&> fun x -> { p with udpFragment = x }

  let inline _sniff f p =
    f p.sniff <&> fun x -> { p with sniff = x }

  let inline _sniffOverrideDestination f p =
    f p.sniffOverrideDestination <&> fun x -> { p with sniffOverrideDestination = x }

  let inline _domainStrategy f p =
    f p.domainStrategy <&> fun x -> { p with domainStrategy = x }

  let inline _udpTimeout f p =
    f p.udpTimeout <&> fun x -> { p with udpTimeout = x }

  let inline _proxyProtocol f p =
    f p.proxyProtocol <&> fun x -> { p with proxyProtocol = x }

  let inline _proxyProtocolAcceptNoHeader f p =
    f p.proxyProtocolAcceptNoHeader <&> fun x -> { p with proxyProtocolAcceptNoHeader = x }

  let inline _detour f p =
    f p.detour <&> fun x -> { p with detour = x }

  let inline _users f p =
    f p.users <&> fun x -> { p with users = x }

type HttpRecord =
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

module HttpRecord =
  let inline _tag f p =
    f p.tag <&> fun x -> { p with tag = x }

  let inline _listen f p =
    f p.listen <&> fun x -> { p with listen = x }

  let inline _listenPort f p =
    f p.listenPort <&> fun x -> { p with listenPort = x }

  let inline _tcpFastOpen f p =
    f p.tcpFastOpen <&> fun x -> { p with tcpFastOpen = x }

  let inline _udpFragment f p =
    f p.udpFragment <&> fun x -> { p with udpFragment = x }

  let inline _sniff f p =
    f p.sniff <&> fun x -> { p with sniff = x }

  let inline _sniffOverrideDestination f p =
    f p.sniffOverrideDestination <&> fun x -> { p with sniffOverrideDestination = x }

  let inline _domainStrategy f p =
    f p.domainStrategy <&> fun x -> { p with domainStrategy = x }

  let inline _udpTimeout f p =
    f p.udpTimeout <&> fun x -> { p with udpTimeout = x }

  let inline _proxyProtocol f p =
    f p.proxyProtocol <&> fun x -> { p with proxyProtocol = x }

  let inline _proxyProtocolAcceptNoHeader f p =
    f p.proxyProtocolAcceptNoHeader <&> fun x -> { p with proxyProtocolAcceptNoHeader = x }

  let inline _detour f p =
    f p.detour <&> fun x -> { p with detour = x }

  let inline _users f p =
    f p.users <&> fun x -> { p with users = x }

  let inline _tls f p =
    f p.tls <&> fun x -> { p with tls = x }

  let inline _setSystemProxy f p =
    f p.setSystemProxy <&> fun x -> { p with setSystemProxy = x }

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

module MixedRecord =
  let inline tag_ f p =
    f p.tag <&> fun x -> { p with tag = x }

  let inline _listen f p =
    f p.listen <&> fun x -> { p with listen = x }

  let inline _listenPort f p =
    f p.listenPort <&> fun x -> { p with listenPort = x }

  let inline _tcpFastOpen f p =
    f p.tcpFastOpen <&> fun x -> { p with tcpFastOpen = x }

  let inline _udpFragment f p =
    f p.udpFragment <&> fun x -> { p with udpFragment = x }

  let inline _sniff f p =
    f p.sniff <&> fun x -> { p with sniff = x }

  let inline _sniffOverrideDestination f p =
    f p.sniffOverrideDestination <&> fun x -> { p with sniffOverrideDestination = x }

  let inline _domainStrategy f p =
    f p.domainStrategy <&> fun x -> { p with domainStrategy = x }
  let inline _udpTimeout f p =
    f p.udpTimeout <&> fun x -> { p with udpTimeout = x }

  let inline _proxyProtocol f p =
    f p.proxyProtocol <&> fun x -> { p with proxyProtocol = x }

  let inline _proxyProtocolAcceptNoHeader f p =
    f p.proxyProtocolAcceptNoHeader <&> fun x -> { p with proxyProtocolAcceptNoHeader = x }

  let inline _detour f p =
    f p.detour <&> fun x -> { p with detour = x }

  let inline _users f p =
    f p.users <&> fun x -> { p with users = x }

  let inline _setSystemProxy f p =
    f p.setSystemProxy <&> fun x -> { p with setSystemProxy = x }

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

module TunRecord =
  let inline _tag f p =
    f p.tag <&> fun x -> { p with tag = x }

  let inline _interfaceName f p =
    f p.interfaceName <&> fun x -> { p with interfaceName = x }

  let inline _endpointIndependentNat f p =
    f p.endpointIndependentNat <&> fun x -> { p with endpointIndependentNat = x }

  let inline _udpTimeout f p =
    f p.udpTimeout <&> fun x -> { p with udpTimeout = x }

  let inline _stack f p =
    f p.stack <&> fun x -> { p with stack = x }

  let inline _includeInterface f p =
    f p.includeInterface <&> fun x -> { p with includeInterface = x }

  let inline _excludeInterface f p =
    f p.excludeInterface <&> fun x -> { p with excludeInterface = x }

  let inline _includeUid f p =
    f p.includeUid <&> fun x -> { p with includeUid = x }

  let inline _includeUidRange f p =
    f p.includeUidRange <&> fun x -> { p with includeUidRange = x }

  let inline _excludeUid f p =
    f p.excludeUid <&> fun x -> { p with excludeUid = x }

  let inline _excludeUidRange f p =
    f p.excludeUidRange <&> fun x -> { p with excludeUidRange = x }

  let inline _includeAndroidUser f p =
    f p.includeAndroidUser <&> fun x -> { p with includeAndroidUser = x }

  let inline _includePackage f p =
    f p.includePackage <&> fun x -> { p with includePackage = x }

  let inline _excludePackage f p =
    f p.excludePackage <&> fun x -> { p with excludePackage = x }

  let inline _platform f p =
    f p.platform <&> fun x -> { p with platform = x }

  let inline _listen f p =
    f p.listen <&> fun x -> { p with listen = x }

  let inline _listenPort f p =
    f p.listenPort <&> fun x -> { p with listenPort = x }

  let inline _tcpFastOpen f p =
    f p.tcpFastOpen <&> fun x -> { p with tcpFastOpen = x }

  let inline _udpFragment f p =
    f p.udpFragment <&> fun x -> { p with udpFragment = x }

  let inline _sniff f p =
    f p.sniff <&> fun x -> { p with sniff = x }

  let inline _sniffOverrideDestination f p =
    f p.sniffOverrideDestination <&> fun x -> { p with sniffOverrideDestination = x }

  let inline _domainStrategy f p =
    f p.domainStrategy <&> fun x -> { p with domainStrategy = x }

  let inline _proxyProtocol f p =
    f p.proxyProtocol <&> fun x -> { p with proxyProtocol = x }

  let inline _proxyProtocolAcceptNoHeader f p =
    f p.proxyProtocolAcceptNoHeader <&> fun x -> { p with proxyProtocolAcceptNoHeader = x }

  let inline _detour f p =
    f p.detour <&> fun x -> { p with detour = x }
