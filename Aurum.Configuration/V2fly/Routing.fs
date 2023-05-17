module Aurum.Configuration.V2fly.Routing

open Aurum
open System.Text.Json.Serialization

type DomainStrategy =
  | [<JsonName("AsIs")>] AsIs
  | [<JsonName("IPIfNonMatch")>] IPIfNonMatch
  | [<JsonName("IPOnDemand")>] IPOnDemand

type DomainMatcher =
  | [<JsonName("mph")>] MinimalPerfectHash
  | [<JsonName("linear")>] Original

type RuleMatchProtocol =
  | [<JsonName("http")>] HTTP
  | [<JsonName("tls")>] TLS

type RuleMatchNetwork =
  | [<JsonName("tcp")>] TCP
  | [<JsonName("udp")>] UDP
  | [<JsonName("tcp,udp")>] TCPAndUDP

[<JsonFSharpConverter(BaseUnionEncoding = JsonUnionEncoding.AdjacentTag)>]
type DomainObject =
  | Plain of string
  | Regex of string
  | RootDomain of string
  | Full of string

type GeoDataObject =
  { InverseMatch: bool option
    Code: string option
    FilePath: string option }

type CidrObject = { IpAddr: string; Prefix: int }

type RuleObject =
  { Tag: string
    Domain: DomainObject list option
    GeoDomain: GeoDataObject list option
    Ip: CidrObject list option
    GeoIp: GeoDataObject list option
    Networks: RuleMatchNetwork
    PortList: string option
    InboundTag: string option }

type RoutingObject =
  { DomainStrategy: DomainStrategy option
    DomainMatcher: DomainMatcher option
    Rules: RuleObject list option }

let createRoutingObject rules domainStrategy =
  { RoutingObject.Rules = rules
    DomainMatcher = Some MinimalPerfectHash
    DomainStrategy = domainStrategy }

