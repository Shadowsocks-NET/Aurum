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

module RuleObject =
  let inline _inbound f p =
    f p.Inbound <&> fun x -> { p with Inbound = x }

let inline _ipVersion f p =
  f p.IpVersion <&> fun x -> { p with IpVersion = x }

let inline _network f p =
  f p.Network <&> fun x -> { p with Network = x }

let inline _authUser f p =
  f p.AuthUser <&> fun x -> { p with AuthUser = x }

let inline _protocol f p =
  f p.Protocol <&> fun x -> { p with Protocol = x }

let inline _domainKeyword f p =
  f p.DomainKeyword <&> fun x -> { p with DomainKeyword = x }

let inline _domainRegex f p =
  f p.DomainRegex <&> fun x -> { p with DomainRegex = x }

let inline _geoip f p =
  f p.Geoip <&> fun x -> { p with Geoip = x }

let inline _ipCidr f p =
  f p.IpCidr <&> fun x -> { p with IpCidr = x }

let inline _port f p =
  f p.Port <&> fun x -> { p with Port = x }

let inline _processName f p =
  f p.ProcessName <&> fun x -> { p with ProcessName = x }

let inline _processPath f p =
  f p.ProcessPath <&> fun x -> { p with ProcessPath = x }

let inline _packageName f p =
  f p.PackageName <&> fun x -> { p with PackageName = x }

let inline _user f p =
  f p.User <&> fun x -> { p with User = x }

let inline _userId f p =
  f p.UserId <&> fun x -> { p with UserId = x }

let inline _clashMode f p =
  f p.ClashMode <&> fun x -> { p with ClashMode = x }

let inline _invert f p =
  f p.Invert <&> fun x -> { p with Invert = x }

let inline _outbound f p =
  f p.Outbound <&> fun x -> { p with Outbound = x }
