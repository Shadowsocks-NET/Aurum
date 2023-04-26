module Aurum.Configuration.Sing.Inbound

open System.Text.Json.Serialization

type SOCKSRecord =
  { tag: string

    listen: string
    listenPort: int16 option Skippable
    tcpFastOpen: bool option Skippable
    udpFragment: bool option Skippable
    sniff: bool option Skippable
    sniffOverrideDestination: bool option Skippable
    domainStrategy: string option Skippable
    udpTimeout: int option Skippable
    proxyProtocol: bool option Skippable
    proxyProtocolAcceptNoHeader: bool option Skippable
    detour: string option Skippable

    users: obj list Skippable }

type HTTPRecord =
  { tag: string
    listen: string
    listenPort: int16 option Skippable
    tcpFastOpen: bool option Skippable
    udpFragment: bool option Skippable
    sniff: bool option Skippable
    sniffOverrideDestination: bool option Skippable
    domainStrategy: string option Skippable
    udpTimeout: int option Skippable
    proxyProtocol: bool option Skippable
    proxyProtocolAcceptNoHeader: bool option Skippable
    detour: string option Skippable

    users: obj list option Skippable
    tls: TLSInbound option Skippable
    setSystemProxy: bool option Skippable }
