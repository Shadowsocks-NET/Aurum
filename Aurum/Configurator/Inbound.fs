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

    let createSocksAccount user pass =
        { AccountObject.User = user
          Pass = pass }
