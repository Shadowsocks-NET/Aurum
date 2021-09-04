namespace Aurum.Configurator.Inbound

open Aurum.Configurator.Transport

// reserved for future annotations.
type Protocols =
    | HTTP
    | Socks
    | DokodemoDoor

type SocksAuth =
    | NoAuth
    | Password

type AccountObject = { user: string; pass: string }

type InboundConfigurationObject =
    { accounts: AccountObject list
      userLevel: int
      timeout: int
      (*HTTP specific*)
      allowTransparent: bool
      (*SOCKS specific*)
      auth: SocksAuth
      udp: bool
      ip: string
      (*Dokodemo-door specific*)
      address: string option
      port: int
      network: string
      followRedirect: bool }

type SniffingObject =
    { enabled: bool
      destOverride: string list
      metadataOnly: bool }

type InboundObject =
    { listen: string
      port: int
      protocol: string
      settings: InboundConfigurationObject
      tag: string
      sniffing: SniffingObject }
