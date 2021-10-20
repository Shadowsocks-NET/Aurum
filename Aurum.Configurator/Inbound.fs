namespace Aurum.Configurator

open FSharp.Json

module Inbound =
    type Protocols =
        | [<JsonUnionCase("http")>] HTTP
        | [<JsonUnionCase("socks")>] Socks
        | [<JsonUnionCase("dokodemo-door")>] DokodemoDoor

    type SocksAuth =
        | [<JsonUnionCase("noauth")>] NoAuth
        | [<JsonUnionCase("password")>] Password

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
          DestOverride: string list
          MetadataOnly: bool }

    type InboundObject =
        { Listen: string
          Port: int
          Protocol: string
          Settings: InboundConfigurationObject
          Tag: string
          Sniffing: SniffingObject }

    let createSocksAccount user pass =
        { AccountObject.User = user
          Pass = pass }
