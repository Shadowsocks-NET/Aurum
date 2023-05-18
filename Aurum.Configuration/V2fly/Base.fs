module Aurum.Configuration.V2fly.Base

open Aurum.Configuration.V2fly.DNS
open Aurum.Configuration.V2fly.Inbound
open Aurum.Configuration.V2fly.Outbound
open Aurum.Configuration.V2fly.Routing

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

  static member Access_ = (fun a -> a.Access), (fun b a -> { a with Access = b })

  static member Error_ = (fun a -> a.Error), (fun b a -> { a with Error = b })

  static member Loglevel_ = (fun a -> a.Loglevel), (fun b a -> { a with Loglevel = b })

type V2flyBaseConfiguration =
  { Log: LogObject
    DNS: DNSObject
    Routing: RoutingObject
    Inbounds: InboundObject list
    Outbounds: OutboundObject list }
