module Aurum.Configuration.Sing.Routing

open System.Text.Json.Serialization
open Aurum.Configuration.Shared
open Aurum.Configuration.Shared.Routing

type IpVersion =
  | [<JsonName 4>] IpV4
  | [<JsonName 6>] IpV6

type NetworkType =
  | [<JsonName "tcp">] TCP
  | [<JsonName "udp">] UDP

  static member FromRuleMatchNetwork(ruleMatchNetwork: RuleMatchNetwork option) =
    Option.map
      (fun inp ->
        match inp with
        | RuleMatchNetwork.TCP -> [ TCP ]
        | RuleMatchNetwork.UDP -> [ UDP ]
        | RuleMatchNetwork.TCPAndUDP -> [ TCP; UDP ])
      ruleMatchNetwork

type RuleObject =
  { Inbound: string list option
    IpVersion: IpVersion option
    Network: NetworkType list option
    AuthUser: string list option
    Protocol: RuleMatchProtocol list option
    Domain: string list option
    DomainSuffix: string list option
    DomainKeyword: string list option
    DomainRegex: string list option
    Geosite: string list option
    Geoip: string list option
    IpCidr: string list option
    Port: int list option
    ProcessName: string list option
    ProcessPath: string list option
    PackageName: string list option
    User: string list option
    UserId: string list option
    ClashMode: string option
    Invert: bool option
    Outbound: string }

  static member FromGenericRuleObject(genericRuleObject: Routing.RuleObject) =
    let ipList =
      genericRuleObject.Ip
      |> Option.map (fun inp ->
        inp
        |> List.choose (fun x ->
          match x with
          | IP ip -> Some ip
          | _ -> None))

    let geoipList =
      genericRuleObject.Ip
      |> Option.map (fun inp ->
        inp
        |> List.choose (fun x ->
          match x with
          | Geoip code -> Some code
          | _ -> None))

    let domainList =
      genericRuleObject.Domain
      |> Option.map (fun inp ->
        inp
        |> List.choose (fun x ->
          match x with
          | Domain domainType ->
            match domainType with
            | Full domain -> Some domain
            | _ -> None
          | _ -> None))

    let domainSuffixList =
      genericRuleObject.Domain
      |> Option.map (fun inp ->
        inp
        |> List.choose (fun x ->
          match x with
          | Domain domainType ->
            match domainType with
            | Suffix domain -> Some domain
            | _ -> None
          | _ -> None))

    let domainRegexList =
      genericRuleObject.Domain
      |> Option.map (fun inp ->
        inp
        |> List.choose (fun x ->
          match x with
          | Domain domainType ->
            match domainType with
            | Regex domain -> Some domain
            | _ -> None
          | _ -> None))

    let domainKeywordList =
      genericRuleObject.Domain
      |> Option.map (fun inp ->
        inp
        |> List.choose (fun x ->
          match x with
          | Domain domainType ->
            match domainType with
            | Keyword domain -> Some domain
            | _ -> None
          | _ -> None))

    let geositeList =
      genericRuleObject.Domain
      |> Option.map (fun inp ->
        inp
        |> List.choose (fun x ->
          match x with
          | Geosite code -> Some code
          | _ -> None))

    { Inbound = genericRuleObject.InboundTag
      IpVersion = None
      Network = NetworkType.FromRuleMatchNetwork genericRuleObject.Networks
      AuthUser = None
      Protocol = None
      Domain = domainList
      DomainSuffix = domainSuffixList
      DomainKeyword = domainKeywordList
      DomainRegex = domainRegexList
      Geosite = geositeList
      Geoip = geoipList
      IpCidr = ipList
      Port = None
      ProcessName = None
      ProcessPath = None
      PackageName = None
      User = None
      UserId = None
      ClashMode = None
      Invert = None
      Outbound = genericRuleObject.Tag }
