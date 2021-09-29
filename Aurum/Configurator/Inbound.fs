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

type AccountObject = { User: string; Pass: string }

type InboundConfigurationObject =
    { Accounts: AccountObject list
      UserLevel: int
      Timeout: int
      (*HTTP specific*)
      AllowTransparent: bool
      (*SOCKS specific*)
      Auth: SocksAuth
      UDP: bool
      IP: string
      (*Dokodemo-door specific*)
      Address: string option
      Port: int
      Network: string
      FollowRedirect: bool }

type SniffingObject =
    { Enabled: bool
      DestOverride: string list
      MetadataOnly: bool }

type InboundObject =
    { Listen: string
      Port: int
      Protocol: string
      Settings: InboundConfigurationObject
      Tag: string
      Sniffing: SniffingObject }
