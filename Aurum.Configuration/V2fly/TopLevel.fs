﻿module Aurum.Configuration.V2Fly.TopLevel

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

    static member Access_ =
        (fun a -> a.Access), (fun b a -> { a with Access = b })

    static member Error_ =
        (fun a -> a.Error), (fun b a -> { a with Error = b })

    static member Loglevel_ =
        (fun a -> a.Loglevel), (fun b a -> { a with Loglevel = b })

type SerializedOutbound = string
type SerializedInbound = string
type SerializedDNSConf = string
type SerializedRoutingConf = string

type TopLevelObject =
    { Log: LogObject
      DNS: DNSObject
      Routing: RoutingObject
      Inbounds: InboundObject list
      Outbounds: OutboundObject list }