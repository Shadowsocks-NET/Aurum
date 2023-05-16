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
    | [<JsonName("bittorrent")>] BitTorrent

type RuleMatchNetwork =
    | [<JsonName("tcp")>] TCP
    | [<JsonName("udp")>] UDP
    | [<JsonName("tcp,udp")>] TCPAndUDP

type RuleObject =
    { DomainMatcher: DomainMatcher option
      Domain: string list option
      GeoDomain: string list option
      Ip: string list option
      Port: string option
      SourcePort: string option
      Network: RuleMatchNetwork option
      Source: string list option
      User: string list option
      Protocol: RuleMatchProtocol option
      Attrs: string option
      OutboundTag: string option
      BalancerTag: string option }

[<JsonFSharpConverter(BaseUnionEncoding = JsonUnionEncoding.AdjacentTag, UnionTagName = "type")>]
type BalancerStrategy =
    | [<JsonName("random")>] Random
    | [<JsonName("leastPing")>] LeastPing

type BalancerObject =
    { Tag: string
      Selector: string list
      Strategy: BalancerStrategy }

type RoutingObject =
    { DomainStrategy: DomainStrategy option
      DomainMatcher: DomainMatcher option
      Rules: RuleObject list option
      Balancers: BalancerObject list option }

type DomainRuleList = string list
type IpRuleList = string list

type DomainStringListObject =
    { Direct: DomainRuleList
      Proxy: DomainRuleList
      Block: DomainRuleList }

type IpStringListObject =
    { Direct: IpRuleList
      Proxy: IpRuleList
      Block: IpRuleList }

type RulePresets =
    | BypassMainland // adopted from v2rayA's mainland whitelist mode
    | GFWList of string list // uses Loyalsoldier/v2ray-rules-dat's gfw.txt
    | Greatfire of string list // uses Loyalsoldier/v2ray-rules-dat's greatfire.txt
    | Loyalsoldier // uses Loyalsoldier/v2ray-rules-dat's recommended v2ray setting in README
    | Empty // no preset rules

// when using full domain list or the recommended setting, these options will be ignored
type RuleConstructionStrategy = { BlockAds: bool }

type ProxyTag =
    | Outbound of string
    | Balancer of string

type RuleType =
    | Direct of DomainRuleList option * IpRuleList option
    | Proxy of DomainRuleList option * IpRuleList option
    | Block of DomainRuleList option * IpRuleList option
    | Skip

let createRoutingObject rules balancers domainStrategy =
    { RoutingObject.Balancers = balancers
      Rules = rules
      DomainMatcher = Some MinimalPerfectHash
      DomainStrategy = domainStrategy }
