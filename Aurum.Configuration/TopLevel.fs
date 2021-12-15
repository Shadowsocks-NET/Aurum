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
      DNS: SerializedDNSConf
      Routing: SerializedRoutingConf
      Inbounds: SerializedInbound list
      Outbounds: SerializedOutbound list }
