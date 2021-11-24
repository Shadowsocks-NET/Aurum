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

type DomainPresets =
    | Full of FullDomainListObject // uses Loyalsoldier/v2ray-rules-dat's {direct,proxy,block}-list.txt
    | GFWList of string list // uses Loyalsoldier/v2ray-rules-dat's gfw.txt
    | Greatfire of string list // uses Loyalsoldier/v2ray-rules-dat's greatfire.txt
    | Loyalsoldier // uses Loyalsoldier/v2ray-rules-dat's recommended v2ray setting in README

// when using full domain list or the recommended setting, these options will be ignored
type RuleConstructionStrategy =
    { BypassMainland: bool
      BlockAds: bool }

type ProxyTag =
    | Outbound of string
    | Balancer of string

type DomainRuleList = string list
type IpRuleList = string list

type RuleType =
    | Direct of DomainRuleList option * IpRuleList option
    | Proxy of DomainRuleList option * IpRuleList option
    | Block of DomainRuleList option * IpRuleList option

let constructDomainEntry domain = $"domain:{domain}"

let constructSingleRule rule proxyTag =
    match rule with
    | Direct (domainRule, ipRule) ->
        { DomainMatcher = None
          Type = Field
          Domains = domainRule
          IP = ipRule
          Port = None
          SourcePort = None
          Network = Some TCPAndUDP
          Source = None
          User = None
          Protocol = None
          Attrs = None
          OutboundTag = Some "freedom"
          BalancerTag = None }
    | Proxy (domainRule, ipRule) ->
        { DomainMatcher = None
          Type = Field
          Domains = domainRule
          IP = ipRule
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
    | Block (domainRule, ipRule) ->
        { DomainMatcher = None
          Type = Field
          Domains = domainRule
          IP = ipRule
          Port = None
          SourcePort = None
          Network = Some TCPAndUDP
          Source = None
          User = None
          Protocol = None
          Attrs = None
          OutboundTag = Some "blackhole"
          BalancerTag = None }

let constructRuleSet ruleSet proxyTag =
    List.map (fun rule -> constructSingleRule rule proxyTag) ruleSet

let constructPreset constructionStrategy =
    let directDomain =
        if constructionStrategy.BypassMainland then
            Some [ "geosite:cn" ]
        else
            None

    let directIp =
        if constructionStrategy.BypassMainland then
            Some [ "223.5.5.5/32"
                   "119.29.29.29/32"
                   "180.76.76.76/32"
                   "114.114.114.114/32"
                   "geoip:cn"
                   "geoip:private" ]
        else
            None

    let directRule =
        if constructionStrategy.BypassMainland then
            Direct(
                Some [ "geosite:cn" ],
                Some [ "223.5.5.5/32"
                       "119.29.29.29/32"
                       "180.76.76.76/32"
                       "114.114.114.114/32"
                       "geoip:cn"
                       "geoip:private" ]
            )
        else
            Direct(None, None)

    let blockDomain =
        if constructionStrategy.BlockAds then
            Some [ "geosite:category-ads-all" ]
        else
            None

    let blockRule =
        if constructionStrategy.BlockAds then
            Block(Some [ "geosite:category-ads-all" ], None)
        else
            Block(None, None)

    constructRuleSet [ directRule
                       blockRule ]

let generateRoutingWithDomainList (domainList, constructionStrategy) proxyTag =
    match domainList with
    | Full domainList ->
        let proxyRule =
            Proxy(Some(domainList.Proxy |> List.map constructDomainEntry), None)

        let directRule =
            Direct(Some(domainList.Direct |> List.map constructDomainEntry), None)

        let blockRule =
            Block(Some(domainList.Block |> List.map constructDomainEntry), None)

        constructRuleSet [ blockRule; directRule; proxyRule ] proxyTag
    | GFWList domainList
    | Greatfire domainList ->
        let proxyRules =
            Proxy(Some(domainList |> List.map constructDomainEntry), None)

        let proxyRuleObjects = constructRuleSet [ proxyRules ] proxyTag

        let presetRuleObjects =
            constructPreset constructionStrategy proxyTag

        presetRuleObjects @ proxyRuleObjects
