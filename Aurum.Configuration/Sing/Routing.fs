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
  { inbound: string list option
    ipVersion: IpVersion option
    network: NetworkType list option
    authUser: string list option
    protocol: RuleMatchProtocol list option
    domain: string list option
    domainSuffix: string list option
    domainKeyword: string list option
    domainRegex: string list option
    geosite: string list option
    geoip: string list option
    ipCidr: string list option
    port: int list option
    processName: string list option
    processPath: string list option
    packageName: string list option
    user: string list option
    userId: string list option
    clashMode: string option
    invert: bool option
    outbound: string }

  static member FromGenericRuleObject(genericRuleObject: Routing.RuleObject) =
    let ipList =
      genericRuleObject.Ip
      |> Option.map (fun inp ->
        inp
        |> List.choose (fun x ->
          match x with
          | Ip ip -> Some ip
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

    { inbound = genericRuleObject.InboundTag
      ipVersion = None
      network = NetworkType.FromRuleMatchNetwork genericRuleObject.Networks
      authUser = None
      protocol = None
      domain = domainList
      domainSuffix = domainSuffixList
      domainKeyword = domainKeywordList
      domainRegex = domainRegexList
      geosite = geositeList
      geoip = geoipList
      ipCidr = ipList
      port = None
      processName = None
      processPath = None
      packageName = None
      user = None
      userId = None
      clashMode = None
      invert = None
      outbound = genericRuleObject.Tag }

module RuleObject =
  let inline _inbound f p =
    f p.inbound <&> fun x -> { p with inbound = x }

let inline _ipVersion f p =
  f p.ipVersion <&> fun x -> { p with ipVersion = x }

let inline _network f p =
  f p.network <&> fun x -> { p with network = x }

let inline _authUser f p =
  f p.authUser <&> fun x -> { p with authUser = x }

let inline _protocol f p =
  f p.protocol <&> fun x -> { p with protocol = x }

let inline _domainKeyword f p =
  f p.domainKeyword <&> fun x -> { p with domainKeyword = x }

let inline _domainRegex f p =
  f p.domainRegex <&> fun x -> { p with domainRegex = x }

let inline _geoip f p =
  f p.geoip <&> fun x -> { p with geoip = x }

let inline _ipCidr f p =
  f p.ipCidr <&> fun x -> { p with ipCidr = x }

let inline _port f p =
  f p.port <&> fun x -> { p with port = x }

let inline _processName f p =
  f p.processName <&> fun x -> { p with processName = x }

let inline _processPath f p =
  f p.processPath <&> fun x -> { p with processPath = x }

let inline _packageName f p =
  f p.packageName <&> fun x -> { p with packageName = x }

let inline _user f p =
  f p.user <&> fun x -> { p with user = x }

let inline _userId f p =
  f p.userId <&> fun x -> { p with userId = x }

let inline _clashMode f p =
  f p.clashMode <&> fun x -> { p with clashMode = x }

let inline _invert f p =
  f p.invert <&> fun x -> { p with invert = x }

let inline _outbound f p =
  f p.outbound <&> fun x -> { p with outbound = x }
