module Aurum.Configuration.Shared.Routing

open Aurum

type DomainStrategy =
  | AsIs
  | IPIfNonMatch
  | IPOnDemand

type RuleMatchProtocol =
  | HTTP
  | TLS
  | BitTorrent

type RuleMatchNetwork =
  | TCP
  | UDP
  | TCPAndUDP

type DomainType =
  | Plain of string
  | Regex of string
  | RootDomain of string
  | Full of string
  | Geosite of string

type IpType =
  | IP of string
  | Geoip of string

type RuleObject =
  { Domain: DomainType list option
    Ip: IpType list option
    Port: string option
    Networks: RuleMatchNetwork option
    Protocol: RuleMatchProtocol option
    Tag: string option }

type DomainRuleList = DomainType list
type IpRuleList = IpType list

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

let constructDirect (domainRule, ipRule) =
  { Domain = domainRule
    Ip = ipRule
    Port = None
    Networks = Some TCPAndUDP
    Protocol = None
    Tag = Some "freedom" }

let constructProxy (domainRule, ipRule) proxyTag =
  { Domain = domainRule
    Ip = ipRule
    Port = None
    Networks = Some TCPAndUDP
    Protocol = None
    Tag = proxyTag }

let constructBlock (domainRule, ipRule) =
  { Domain = domainRule
    Ip = ipRule
    Port = None
    Networks = Some TCPAndUDP
    Protocol = None
    Tag = Some "blackhole" }

let constructSingleRule rule proxyTag =
  match rule with
  | Direct(domainRule, ipRule) -> Some(constructDirect (domainRule, ipRule))
  | Proxy(domainRule, ipRule) -> Some(constructProxy (domainRule, ipRule) proxyTag)
  | Block(domainRule, ipRule) -> Some(constructBlock (domainRule, ipRule))
  | Skip -> None

let constructRuleSet ruleSet proxyTag =
  List.map (fun rule -> constructSingleRule rule proxyTag) ruleSet
  |> List.filter Option.isSome
  |> List.map Option.get

let mergeRules rule1 rule2 =
  match rule1, rule2 with
  | Direct(xDomain, xIP), Direct(yDomain, yIP) -> Direct(mergeOptionList xDomain yDomain, mergeOptionList xIP yIP)
  | Proxy(xDomain, xIP), Proxy(yDomain, yIP) -> Proxy(mergeOptionList xDomain yDomain, mergeOptionList xIP yIP)
  | Block(xDomain, xIP), Block(yDomain, yIP) -> Block(mergeOptionList xDomain yDomain, mergeOptionList xIP yIP)
  | _ -> raise RuleTypeNotMatchException

let constructPreset constructionStrategy =

  let blockRule =
    if constructionStrategy.BlockAds then
      Block(Some [ Geosite "category-ads-all" ], None)
    else
      Skip

  blockRule

let generateRoutingRules (domainList, constructionStrategy, userDomainRules: DomainStringListObject, userIpRules) =
  match domainList with
  | BypassMainland ->
    let blockPreset = constructPreset constructionStrategy

    let userProxyRule = Proxy(Some userDomainRules.Proxy, Some userIpRules.Proxy)

    let userDirectRule = Direct(Some userDomainRules.Direct, Some userIpRules.Direct)

    let userBlockRule = Block(Some userDomainRules.Block, Some userIpRules.Block)

    let geolocationProxy = Proxy(Some [ Geosite "geolocation-!cn" ], None)

    let scholarProxy = Proxy(Some [ Geosite "google-scholar" ], None)

    let scholarDirect =
      Direct(Some [ Geosite "category-scholar-!cn"; Geosite "category-scholar-cn" ], None)

    let siteCNDirect = Direct(Some [ Geosite "cn" ], None)

    let sarProxy = Proxy(None, Some [ Geoip "hk"; Geoip "mo" ])

    let ipDirect = Direct(None, Some [ Geoip "private"; Geoip "cn" ])

    constructRuleSet
      [ mergeRules userBlockRule blockPreset
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

    let userBlockRule = Block(Some userDomainRules.Block, Some userIpRules.Block)

    let userDirectRule = Block(Some userDomainRules.Direct, Some userIpRules.Direct)

    let userProxyRule = Proxy(Some userDomainRules.Proxy, Some userIpRules.Proxy)

    let proxyRule = Proxy(Some(domainList |> List.map (fun a -> Plain a)), None)

    constructRuleSet
      [ mergeRules userBlockRule blockPreset
        userDirectRule
        mergeRules userProxyRule proxyRule
        Proxy(None, None) ]
  | Loyalsoldier ->
    let userBlockRule = Block(Some userDomainRules.Block, Some userIpRules.Block)

    let userDirectRule = Block(Some userDomainRules.Direct, Some userIpRules.Direct)

    let userProxyRule = Proxy(Some userDomainRules.Proxy, Some userIpRules.Proxy)

    let blockRule = Block(Some([ Geosite "category-ads-all" ]), None)

    let directRule1 =
      Direct(
        Some(
          [ Geosite "private"
            Geosite "apple-cn"
            Geosite "google-cn"
            Geosite "tld-cn"
            Geosite "category-games@cn" ]
        ),
        None
      )

    let proxyRule = Proxy(Some([ Geosite"geolocation-!cn" ]), None)

    let directRule2 = Direct(Some([ Geosite"cn" ]), None)

    constructRuleSet
      [ mergeRules userBlockRule blockRule
        userDirectRule
        userProxyRule
        directRule1
        proxyRule
        directRule2
        Proxy(None, None) ]
  | Empty ->
    let blockPreset = constructPreset constructionStrategy

    constructRuleSet
      [ mergeRules blockPreset (Block(Some userDomainRules.Block, Some userIpRules.Block))
        Direct(Some userDomainRules.Direct, Some userIpRules.Direct)
        Proxy(Some userDomainRules.Proxy, Some userIpRules.Proxy) ]
