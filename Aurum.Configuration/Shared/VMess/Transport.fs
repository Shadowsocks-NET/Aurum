module Aurum.Configuration.Shared.VMess.Transports

open System.Text.Json.Serialization

[<RequireQualifiedAccess>]
type Networks =
    | [<JsonName("ws")>] WS
    | [<JsonName("grpc")>] GRPC
    | [<JsonName("tcp")>] TCP  // merged into http for sing-box
    | [<JsonName("kcp")>] KCP
    | [<JsonName("domainsocket")>] DomainSocket
    | [<JsonName("http")>] HTTP
    | [<JsonName("quic")>] QUIC

[<RequireQualifiedAccess>]
type Security =
    | [<JsonName("none")>] None
    | [<JsonName("tls")>] TLS
    | [<JsonName("xtls")>] XTLS  // merely a reservation. V2fly and Sing backend does not have XTLS support.
