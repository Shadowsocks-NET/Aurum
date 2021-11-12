module Aurum.Configuration.Routing

open Aurum
open Aurum.Configuration
open FSharp.Json

type DomainStrategy =
    | [<JsonUnionCase("AsIs")>] AsIs
    | [<JsonUnionCase("IPIfNonMatch")>] IPIfNonMatch
    | [<JsonUnionCase("IPOnDemand")>] IPOnDemand

type DomainMatcher =
    | [<JsonUnionCase("mph")>] MinimalPerfectHash
    | [<JsonUnionCase("linear")>] Original

type RuleObjectType = | [<JsonUnionCase("field")>] Field

type RuleMatchProtocol =
    | [<JsonUnionCase("http")>] HTTP
    | [<JsonUnionCase("tls")>] TLS
    | [<JsonUnionCase("bittorrent")>] BitTorrent

type RuleObject =
    { DomainMatcher: DomainMatcher option
      Type: RuleObjectType
      Domains: string list option
      IP: string list option
      Port: string option
      SourcePort: string option
      Network: string option
      Source: string list option
      User: string list option
      Protocol: RuleMatchProtocol option
      Attrs: string option
      OutboundTag: string option
      BalancerTag: string option }

[<JsonUnion(Mode = UnionMode.CaseKeyAsFieldValue, CaseKeyField = "type")>]
type BalancerStrategy =
    | [<JsonUnionCase("random")>] Random
    | [<JsonUnionCase("leastPing")>] LeastPing

type BalancerObject = { Tag: string; Selector: string list; Strategy: BalancerStrategy }
