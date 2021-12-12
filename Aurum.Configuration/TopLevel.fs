module Aurum.Configuration.TopLevel

open Aurum.Configuration.DNS
open Aurum.Configuration.Inbound
open Aurum.Configuration.Outbound
open Aurum.Configuration.Routing

[<RequireQualifiedAccess>]
type LogLevel =
    | Debug
    | Info
    | Warning
    | Error
    | None

type LogObject =
    { Access: string option
      Error: string option
      Loglevel: LogLevel }

type TopLevelObject =
    { Log: LogObject
      DNS: DNSObject
      Routing: RoutingObject
      Inbounds: InboundObject list
      Outbounds: OutboundObject list }
