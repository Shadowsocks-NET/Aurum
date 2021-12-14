module Aurum.Configuration.TopLevel


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

type SerializedOutbound = string
type SerializedInbound = string
type SerializedDNSConf = string
type SerializedRoutingConf = string

type TopLevelObject =
    { Log: LogObject
      DNS: SerializedDNSConf
      Routing: SerializedRoutingConf
      Inbounds: SerializedInbound list
      Outbounds: SerializedOutbound list }
