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
