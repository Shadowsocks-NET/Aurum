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

type DomainPresets =
    | Full of DomainStringListObject // uses Loyalsoldier/v2ray-rules-dat's {direct,proxy,block}-list.txt
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

type RuleType =
    | Direct of DomainRuleList option * IpRuleList option
    | Proxy of DomainRuleList option * IpRuleList option
    | Block of DomainRuleList option * IpRuleList option
    | Skip

let constructDomainEntry domain = $"domain:{domain}"

let constructSingleRule rule proxyTag : RuleObject option =
    match rule with
    | Direct (domainRule, ipRule) ->
        Some(
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
        )
    | Proxy (domainRule, ipRule) ->
        Some(
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
        )
    | Block (domainRule, ipRule) ->
        Some(
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
        )
    | Skip -> None

let constructRuleSet ruleSet proxyTag =
    List.map (fun rule -> constructSingleRule rule proxyTag) ruleSet
    |> List.filter Option.isSome
    |> List.map Option.get

let constructPreset constructionStrategy =

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
            Skip

    let blockRule =
        if constructionStrategy.BlockAds then
            Block(Some [ "geosite:category-ads-all" ], None)
        else
            Skip

    constructRuleSet [ directRule
                       blockRule ]

let generateRoutingWithDomainList (domainList, constructionStrategy, extraDomainRules, extraIpRules) =
    match domainList with
    | Full domainList ->
        let proxyRule =
            Proxy(
                Some(
                    (domainList.Proxy |> List.map constructDomainEntry)
                    @ extraDomainRules.Proxy
                ),
                Some extraIpRules.Proxy
            )

        let directRule =
            Direct(
                Some(
                    (domainList.Direct |> List.map constructDomainEntry)
                    @ extraDomainRules.Direct
                ),
                Some extraIpRules.Direct
            )

        let blockRule =
            Block(
                Some(
                    (domainList.Block |> List.map constructDomainEntry)
                    @ extraDomainRules.Block
                ),
                Some extraIpRules.Block
            )

        constructRuleSet [ blockRule
                           directRule
                           proxyRule
                           Direct(None, None) ]
    | GFWList domainList
    | Greatfire domainList ->
        let proxyRule =
            Proxy(
                Some(
                    (domainList |> List.map constructDomainEntry)
                    @ extraDomainRules.Proxy
                ),
                Some extraIpRules.Proxy
            )

        let directRule =
            Direct(Some extraDomainRules.Direct, Some extraIpRules.Direct)

        let blockRule =
            Block(Some extraDomainRules.Block, Some extraIpRules.Block)

        fun proxyTag ->
            constructPreset constructionStrategy proxyTag
            @ constructRuleSet
                [ blockRule
                  directRule
                  proxyRule
                  Direct(None, None) ]
                proxyTag
    | Loyalsoldier ->
        let blockRule =
            Block(
                Some(
                    [ "geosite:category-ads-all" ]
                    @ extraDomainRules.Block
                ),
                Some extraIpRules.Block
            )

        let directRule1 =
            Direct(
                Some(
                    [ "geosite:private"
                      "geosite:apple-cn"
                      "geosite:google-cn"
                      "geosite:tld-cn"
                      "geosite:category-games@cn" ]
                    @ extraDomainRules.Direct
                ),
                Some extraIpRules.Direct
            )

        let proxyRule =
            Proxy(
                Some(
                    [ "geosite:geolocation-!cn" ]
                    @ extraDomainRules.Proxy
                ),
                Some extraIpRules.Proxy
            )

        let directRule2 = Direct(Some([ "geosite:cn" ]), None)

        constructRuleSet [ blockRule
                           directRule1
                           proxyRule
                           directRule2
                           Proxy(None, None) ]
