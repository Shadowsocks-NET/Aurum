module Aurum.Configuration.Shared.Routing

open System.Text.Json.Serialization
open Aurum
open FSharpPlus.Lens
open FSharpPlus.Data
open FSharpPlus

let inline traverseResultList (f: 'a -> Result<'b, 'e>) (xs: 'a list) : Result<'b list, 'e> = traverse f xs

type DomainStrategy =
  | AsIs
  | IPIfNonMatch
  | IPOnDemand

type RuleMatchProtocol =
  | HTTP
  | TLS

type RuleMatchNetwork =
  | [<JsonName "tcp">] TCP
  | [<JsonName "udp">] UDP
  | [<JsonName "tcp,udp">] TCPAndUDP

type DomainMatchingType =
  | Full of string
  | Regex of string
  | Suffix of string
  | Keyword of string

module DomainMatchingType =
  let inline _Full f p =
    prism'
      Full
      (fun x ->
        match x with
        | Full x -> Some x
        | _ -> None)
      f
      p

  let inline _Regex f p =
    prism'
      Regex
      (fun x ->
        match x with
        | Regex x -> Some x
        | _ -> None)
      f
      p

  let inline _Suffix f p =
    prism'
      Suffix
      (fun x ->
        match x with
        | Suffix x -> Some x
        | _ -> None)
      f
      p

  let inline _Keyword f p =
    prism'
      Keyword
      (fun x ->
        match x with
        | Keyword x -> Some x
        | _ -> None)
      f
      p

type DomainType =
  | Domain of DomainMatchingType
  | Geosite of string

module DomainType =
  let inline _Domain f p =
    prism'
      Domain
      (fun x ->
        match x with
        | Domain x -> Some x
        | _ -> None)
      f
      p

  let inline _Geosite f p =
    prism'
      Geosite
      (fun x ->
        match x with
        | Geosite x -> Some x
        | _ -> None)
      f
      p

type IpType =
  | Ip of string
  | Geoip of string

module IpType =
  let inline _Ip f p =
    prism'
      Ip
      (fun x ->
        match x with
        | Ip x -> Some x
        | _ -> None)
      f
      p

  let inline _Geoip f p =
    prism'
      Geoip
      (fun x ->
        match x with
        | Geoip x -> Some x
        | _ -> None)
      f
      p

type RuleObject =
  { Domain: DomainType list option
    Ip: IpType list option
    Port: string option
    Networks: RuleMatchNetwork option
    Protocol: RuleMatchProtocol option
    InboundTag: string list option
    Tag: string }

module RuleObject =
  let inline _Domain f p =
    f p.Domain <&> fun x -> { p with Domain = x }

  let inline _Ip f p = f p.Ip <&> fun x -> { p with Ip = x }

  let inline _Port f p =
    f p.Port <&> fun x -> { p with Port = x }

  let inline _Networks f p =
    f p.Networks <&> fun x -> { p with Networks = x }

  let inline _Protocol f p =
    f p.Protocol <&> fun x -> { p with Protocol = x }

  let inline _InboundTag f p =
    f p.InboundTag <&> fun x -> { p with InboundTag = x }

  let inline _Tag f p = f p.Tag <&> fun x -> { p with Tag = x }

type DomainRuleList = DomainType list
type IpRuleList = IpType list

type DomainStringListObject =
  { Direct: DomainRuleList
    Proxy: DomainRuleList
    Block: DomainRuleList }

module DomainStringListObject =
  let inline _Direct f p =
    f p.Direct <&> fun x -> { p with Direct = x }

  let inline _Proxy f p =
    f p.Proxy <&> fun x -> { p with Proxy = x }

  let inline _Block f p =
    f p.Block <&> fun x -> { p with Block = x }

type IpStringListObject =
  { Direct: IpRuleList
    Proxy: IpRuleList
    Block: IpRuleList }

module IpStringListObject =
  let inline _Direct f p =
    f p.Direct <&> fun x -> { p with Direct = x }

  let inline _Proxy f p =
    f p.Proxy <&> fun x -> { p with Proxy = x }

  let inline _Block f p =
    f p.Block <&> fun x -> { p with Block = x }

type RulePresets =
  | BypassMainland // adopted from v2rayA's mainland whitelist mode
  | GFWList of string list // uses Loyalsoldier/v2ray-rules-dat's gfw.txt
  | Loyalsoldier // uses Loyalsoldier/v2ray-rules-dat's recommended v2ray setting in README
  | Empty // no preset rules

// when using full domain list or the recommended setting, these options will be ignored
type RuleConstructionStrategy = { BlockAds: bool }

module RuleConstructionStrategy =
  let inline _BlockAds f p =
    f p.BlockAds <&> fun x -> { p with BlockAds = x }

type ProxyTag =
  | Outbound of string
  | Balancer of string

module ProxyTag =
  let inline _Outbound f p =
    prism'
      Outbound
      (fun x ->
        match x with
        | Outbound x -> Some x
        | _ -> None)
      f
      p

  let inline _Balancer f p =
    prism'
      Balancer
      (fun x ->
        match x with
        | Balancer x -> Some x
        | _ -> None)
      f
      p

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
    InboundTag = None
    Tag = "freedom" }

let constructProxy (domainRule, ipRule) proxyTag =
  { Domain = domainRule
    Ip = ipRule
    Port = None
    Networks = Some TCPAndUDP
    Protocol = None
    InboundTag = None
    Tag = proxyTag }

let constructBlock (domainRule, ipRule) =
  { Domain = domainRule
    Ip = ipRule
    Port = None
    Networks = Some TCPAndUDP
    Protocol = None
    InboundTag = None
    Tag = "blackhole" }

let constructSingleRule rule proxyTag =
  match rule with
  | Direct(domainRule, ipRule) -> Some(constructDirect (domainRule, ipRule))
  | Proxy(domainRule, ipRule) -> Some(constructProxy (domainRule, ipRule) proxyTag)
  | Block(domainRule, ipRule) -> Some(constructBlock (domainRule, ipRule))
  | Skip -> None

let constructRuleSet ruleSet proxyTag =
  match ruleSet with
  | Ok ruleSet ->
    List.map (fun rule -> constructSingleRule rule proxyTag) ruleSet
    |> List.filter Option.isSome
    |> List.map Option.get
    |> Ok
  | Error e -> Error e

let mergeRules rule1 rule2 =
  match rule1, rule2 with
  | Direct(xDomain, xIP), Direct(yDomain, yIP) -> Ok(Direct(mergeOptionList xDomain yDomain, mergeOptionList xIP yIP))
  | Proxy(xDomain, xIP), Proxy(yDomain, yIP) -> Ok(Proxy(mergeOptionList xDomain yDomain, mergeOptionList xIP yIP))
  | Block(xDomain, xIP), Block(yDomain, yIP) -> Ok(Block(mergeOptionList xDomain yDomain, mergeOptionList xIP yIP))
  | _ -> Error RuleTypeNotMatchException

let constructPreset constructionStrategy =
  if constructionStrategy.BlockAds then
    Block(Some [ Geosite "category-ads-all" ], None)
  else
    Skip

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

    constructRuleSet (
      (mergeRules userBlockRule blockPreset
       :: ([ userDirectRule
             userProxyRule
             geolocationProxy
             scholarProxy
             scholarDirect
             siteCNDirect
             sarProxy
             ipDirect
             Proxy(None, None) ]
           |> List.map Ok))
      |> traverseResultList id
    )
  | GFWList domainList ->
    let blockPreset = constructPreset constructionStrategy

    let userBlockRule = Block(Some userDomainRules.Block, Some userIpRules.Block)

    let userDirectRule = Block(Some userDomainRules.Direct, Some userIpRules.Direct)

    let userProxyRule = Proxy(Some userDomainRules.Proxy, Some userIpRules.Proxy)

    let proxyRule =
      Proxy(Some(domainList |> List.map (fun a -> Domain(Suffix a))), None)

    constructRuleSet (
      [ mergeRules userBlockRule blockPreset
        Ok userDirectRule
        mergeRules userProxyRule proxyRule
        Ok(Proxy(None, None)) ]
      |> traverseResultList id
    )
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

    let proxyRule = Proxy(Some([ Geosite "geolocation-!cn" ]), None)

    let directRule2 = Direct(Some([ Geosite "cn" ]), None)

    constructRuleSet (
      (mergeRules userBlockRule blockRule)
      :: ([ userDirectRule
            userProxyRule
            directRule1
            proxyRule
            directRule2
            Proxy(None, None) ]
          |> List.map Ok)
      |> traverseResultList id
    )
  | Empty ->
    let blockPreset = constructPreset constructionStrategy

    constructRuleSet (
      [ mergeRules blockPreset (Block(Some userDomainRules.Block, Some userIpRules.Block))
        Ok(Direct(Some userDomainRules.Direct, Some userIpRules.Direct))
        Ok(Proxy(Some userDomainRules.Proxy, Some userIpRules.Proxy)) ]
      |> traverseResultList id
    )
