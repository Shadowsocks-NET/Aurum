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

type RuleMatchNetwork =
    | [<JsonUnionCase("tcp")>] TCP
    | [<JsonUnionCase("udp")>] UDP
    | [<JsonUnionCase("tcp,udp")>] TCPAndUDP

type RuleObject =
    { DomainMatcher: DomainMatcher option
      Type: RuleObjectType
      Domains: string list option
      IP: string list option
      Port: string option
      SourcePort: string option
      Network: RuleMatchNetwork option
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

type BalancerObject =
    { Tag: string
      Selector: string list
      Strategy: BalancerStrategy }

type RoutingObject =
    { DomainStrategy: DomainStrategy option
      DomainMatcher: DomainMatcher option
      Rules: RuleObject list option
      Balancers: BalancerObject list option }

type FullDomainListObject =
    { Direct: string list
      Proxy: string list
      Block: string list }

type DomainList =
    | Full of FullDomainListObject
    | GFWList of string list
    | Greatfire of string list

type RuleConstructionStrategy = // when using full domain list, these options will be ignored
    { BypassMainland: bool
      BlockAds: bool }

type ProxyTag =
    | Outbound of string
    | Balancer of string

let constructDomainEntry domain = $"domain:{domain}"

let generateRoutingWithDomainList (domainList, constructionStrategy, proxyTag) =
    match domainList with
    | Full domainList ->
        let proxyDomain =
            domainList.Proxy |> List.map constructDomainEntry

        let directDomain =
            domainList.Direct |> List.map constructDomainEntry

        let blockDomain =
            domainList.Block |> List.map constructDomainEntry

        let proxyRule =
            { DomainMatcher = None
              Type = Field
              Domains = Some proxyDomain
              IP = None
              Port = None
              SourcePort = None
              Network = Some TCPAndUDP
              Source = None
              User = None
              Protocol = None
              Attrs = None
              OutboundTag =
                match proxyTag with
                | Outbound tag -> Some tag
                | Balancer _ -> None
              BalancerTag =
                match proxyTag with
                | Balancer tag -> Some tag
                | Outbound _ -> None }

        let directRule =
            { DomainMatcher = None
              Type = Field
              Domains = Some directDomain
              IP = None
              Port = None
              SourcePort = None
              Network = Some TCPAndUDP
              Source = None
              User = None
              Protocol = None
              Attrs = None
              OutboundTag = Some "freedom"
              BalancerTag = None }

        let blockRule =
            { DomainMatcher = None
              Type = Field
              Domains = Some blockDomain
              IP = None
              Port = None
              SourcePort = None
              Network = Some TCPAndUDP
              Source = None
              User = None
              Protocol = None
              Attrs = None
              OutboundTag = Some "blackhole"
              BalancerTag = None }

        [ proxyRule; directRule; blockRule ]
    | GFWList domainList ->
        []
