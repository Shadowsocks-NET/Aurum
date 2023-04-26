module Aurum.Configuration.V2fly.Inbound

open System.Text.Json.Serialization

type Protocols =
    | [<JsonName("http")>] HTTP
    | [<JsonName("socks")>] Socks
    | [<JsonName("dokodemo-door")>] DokodemoDoor

type SocksAuth =
    | [<JsonName("noauth")>] NoAuth
    | [<JsonName("password")>] Password

type DestinationOverride =
    | [<JsonName("http")>] HTTP
    | [<JsonName("tls")>] TLS
    | [<JsonName("fakedns")>] FakeDNS
    | [<JsonName("fakedns+others")>] FakeDNSOthers

type AccountObject = { User: string; Pass: string }

type InboundConfigurationObject =
    { Accounts: AccountObject list option
      UserLevel: int option
      Timeout: int option
      (*HTTP specific*)
      AllowTransparent: bool option
      (*SOCKS specific*)
      Auth: SocksAuth option
      UDP: bool option
      IP: string option
      (*Dokodemo-door specific*)
      Address: string option
      Port: int option
      Network: string option
      FollowRedirect: bool option }

// stub object (reserved)
type SocksInboundConfigurationObject =
    { Accounts: AccountObject list option
      UserLevel: int option
      Timeout: int option
      Auth: SocksAuth option
      UDP: bool option
      IP: string option }

// stub object (reserved)
type DokodemoDoorInboundConfigurationObject =
    { Accounts: AccountObject list option
      UserLevel: int option
      Timeout: int option
      Address: string option
      Port: int option
      Network: string option
      FollowRedirect: bool option }

type SniffingObject =
    { Enabled: bool
      DestOverride: DestinationOverride list
      MetadataOnly: bool }

type InboundObject =
    { Listen: string
      Port: int
      Protocol: Protocols
      Settings: InboundConfigurationObject
      Tag: string
      Sniffing: SniffingObject }

let createSocksAccount (user, pass) =
    { AccountObject.User = user
      Pass = pass }

let createSocksInboundObject (auth, accounts, udp, ip, userLevel) =
    { InboundConfigurationObject.Auth = auth
      Accounts = accounts
      UserLevel = userLevel
      UDP = udp
      IP = ip
      Timeout = None
      AllowTransparent = None
      Address = None
      Port = None
      Network = None
      FollowRedirect = None }

let createHttpInoundObject (timeout, accounts, allowTransparent, userLevel) =
    { InboundConfigurationObject.Auth = None
      Accounts = accounts
      UserLevel = userLevel
      UDP = None
      IP = None
      Timeout = timeout
      AllowTransparent = allowTransparent
      Address = None
      Port = None
      Network = None
      FollowRedirect = None }

let createDokodemoDoorInboundObject (address, port, network, timeout, followRedirect, userLevel) =
    { InboundConfigurationObject.Auth = None
      Accounts = None
      UserLevel = userLevel
      UDP = None
      IP = None
      Timeout = timeout
      AllowTransparent = None
      Address = address
      Port = port
      Network = network
      FollowRedirect = followRedirect }

let createSniffingObject (enabled, destinationOverride, metadataOnly) =
    { SniffingObject.Enabled = enabled
      DestOverride = destinationOverride
      MetadataOnly = metadataOnly }

let createInboundObject (allowLocalConnection, port, protocol, settings, sniffing, tag) =
    { InboundObject.Listen =
          if allowLocalConnection then
              "0.0.0.0"
          else
              "127.0.0.1"
      Port = port
      Protocol = protocol
      Settings = settings
      Sniffing = sniffing
      Tag = Option.defaultValue "Default" tag }
