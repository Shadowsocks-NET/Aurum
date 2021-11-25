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

let constructDomainEntry domain = $"domain:{domain}"

let constructDirect (domainRule, ipRule) : RuleObject =
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

let constructProxy (domainRule, ipRule) proxyTag : RuleObject =
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

let constructBlock (domainRule, ipRule) : RuleObject =
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

let constructSingleRule rule proxyTag : RuleObject option =
    match rule with
    | Direct (domainRule, ipRule) -> Some(constructDirect (domainRule, ipRule))
    | Proxy (domainRule, ipRule) -> Some(constructProxy (domainRule, ipRule) proxyTag)
    | Block (domainRule, ipRule) -> Some(constructBlock (domainRule, ipRule))
    | Skip -> None

let constructRuleSet ruleSet proxyTag =
    List.map (fun rule -> constructSingleRule rule proxyTag) ruleSet
    |> List.filter Option.isSome
    |> List.map Option.get

let mergeRules rule1 rule2 =
    match rule1, rule2 with
    | Direct (xDomain, xIP), Direct (yDomain, yIP) -> Direct(mergeOptionList xDomain yDomain, mergeOptionList xIP yIP)
    | Proxy (xDomain, xIP), Proxy (yDomain, yIP) -> Proxy(mergeOptionList xDomain yDomain, mergeOptionList xIP yIP)
    | Block (xDomain, xIP), Block (yDomain, yIP) -> Block(mergeOptionList xDomain yDomain, mergeOptionList xIP yIP)
    | _ -> raise RuleTypeNotMatchException

let constructPreset constructionStrategy =

    let blockRule =
        if constructionStrategy.BlockAds then
            Block(Some [ "geosite:category-ads-all" ], None)
        else
            Skip

    blockRule

let generateRoutingRules (domainList, constructionStrategy, userDomainRules, userIpRules) =
    match domainList with
    | BypassMainland ->
        let blockPreset = constructPreset constructionStrategy

        let userProxyRule =
            Proxy(Some userDomainRules.Proxy, Some userIpRules.Proxy)

        let userDirectRule =
            Direct(Some userDomainRules.Direct, Some userIpRules.Direct)

        let userBlockRule =
            Block(Some userDomainRules.Block, Some userIpRules.Block)

        let geolocationProxy =
            Proxy(Some [ "geosite:geolocation-!cn" ], None)

        let scholarProxy =
            Proxy(Some [ "geosite:google-scholar" ], None)

        let scholarDirect =
            Direct(
                Some [ "geosite:category-scholar-!cn"
                       "geosite:category-scholar-cn" ],
                None
            )

        let siteCNDirect = Direct(Some [ "geosite:cn" ], None)

        let sarProxy =
            Proxy(None, Some [ "geoip:hk"; "geoip:mo" ])

        let ipDirect =
            Direct(None, Some [ "geoip:private"; "geoip:cn" ])

        constructRuleSet [ mergeRules userBlockRule blockPreset
                           userDirectRule
                           userProxyRule
                           geolocationProxy
                           scholarProxy
                           scholarDirect
                           siteCNDirect
                           sarProxy
                           ipDirect
                           Proxy(None, None) ]
    | GFWList domainList
    | Greatfire domainList ->
        let blockPreset = constructPreset constructionStrategy

        let userBlockRule =
            Block(Some userDomainRules.Block, Some userIpRules.Block)

        let userDirectRule =
            Block(Some userDomainRules.Direct, Some userIpRules.Direct)

        let userProxyRule =
            Proxy(Some userDomainRules.Proxy, Some userIpRules.Proxy)

        let proxyRule =
            Proxy(Some((domainList |> List.map constructDomainEntry)), None)

        constructRuleSet [ mergeRules userBlockRule blockPreset
                           userDirectRule
                           mergeRules userProxyRule proxyRule
                           Proxy(None, None) ]
    | Loyalsoldier ->
        let userBlockRule =
            Block(Some userDomainRules.Block, Some userIpRules.Block)

        let userDirectRule =
            Block(Some userDomainRules.Direct, Some userIpRules.Direct)

        let userProxyRule =
            Proxy(Some userDomainRules.Proxy, Some userIpRules.Proxy)

        let blockRule =
            Block(Some([ "geosite:category-ads-all" ]), None)

        let directRule1 =
            Direct(
                Some(
                    [ "geosite:private"
                      "geosite:apple-cn"
                      "geosite:google-cn"
                      "geosite:tld-cn"
                      "geosite:category-games@cn" ]
                ),
                None
            )

        let proxyRule =
            Proxy(Some([ "geosite:geolocation-!cn" ]), None)

        let directRule2 = Direct(Some([ "geosite:cn" ]), None)

        constructRuleSet [ mergeRules userBlockRule blockRule
                           userDirectRule
                           userProxyRule
                           directRule1
                           proxyRule
                           directRule2
                           Proxy(None, None) ]
    | Empty ->
        let blockPreset = constructPreset constructionStrategy

        constructRuleSet [ mergeRules blockPreset (Block(Some userDomainRules.Block, Some userIpRules.Block))
                           Direct(Some userDomainRules.Direct, Some userIpRules.Direct)
                           Proxy(Some userDomainRules.Proxy, Some userIpRules.Proxy) ]
