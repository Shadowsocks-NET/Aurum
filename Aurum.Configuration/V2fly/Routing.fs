module Aurum.Configuration.V2fly.Routing

open Aurum
open Aurum.Configuration.Shared
open Aurum.Configuration.Shared.Routing
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

[<JsonFSharpConverter(BaseUnionEncoding = JsonUnionEncoding.AdjacentTag)>]
type DomainObject =
  | Plain of string
  | Regex of string
  | RootDomain of string
  | Full of string

type GeoDataObject =
  { InverseMatch: bool option
    Code: string option
    FilePath: string option }

type CidrObject = { IpAddr: string; Prefix: int }

let createGeoDataObject (code, filepath) =
  { InverseMatch = None
    Code = Some code
    FilePath = filepath }

let mapDomainMatchingType domainMatchingType =
  match domainMatchingType with
  | DomainMatchingType.Full str -> Full str
  | DomainMatchingType.Regex str -> Regex str
  | Keyword str -> Plain str
  | Suffix str -> RootDomain str

let parseCidrString (cidrString: string) =
  let cidrStringParts = cidrString.Split("/")

  { IpAddr = cidrStringParts[0]
    Prefix = System.Int32.Parse(cidrStringParts[1]) }

type RuleObject =
  { Tag: string
    Domain: DomainObject list option
    GeoDomain: GeoDataObject list option
    Ip: CidrObject list option
    GeoIp: GeoDataObject list option
    Networks: RuleMatchNetwork option
    PortList: string option
    InboundTag: string list option }

  static member FromGenericRuleObject(genericRuleObject: Routing.RuleObject) =
    let ipList =
      genericRuleObject.Ip
      |> Option.map (fun imp ->
        imp
        |> List.choose (fun x ->
          match x with
          | Ip ip -> parseCidrString ip |> Some
          | _ -> None))
      |> Option.bind emptyListToNone

    let geoipList =
      genericRuleObject.Ip
      |> Option.map (fun inp ->
        inp
        |> List.choose (fun x ->
          match x with
          | Geoip geoip -> createGeoDataObject (geoip, None) |> Some
          | _ -> None))
      |> Option.bind emptyListToNone

    let domainList =
      genericRuleObject.Domain
      |> Option.map (fun inp ->
        inp
        |> List.choose (fun entries ->
          match entries with
          | Domain domainMatchingType -> mapDomainMatchingType domainMatchingType |> Some
          | _ -> None))
      |> Option.bind emptyListToNone

    let geositeList =
      genericRuleObject.Domain
      |> Option.map (fun inp ->
        inp
        |> List.choose (fun entries ->
          match entries with
          | Geosite geosite -> createGeoDataObject (geosite, None) |> Some
          | _ -> None))
      |> Option.bind emptyListToNone

    { RuleObject.Tag = genericRuleObject.Tag
      Domain = domainList
      GeoDomain = geositeList
      Ip = ipList
      GeoIp = geoipList
      Networks = genericRuleObject.Networks
      PortList = genericRuleObject.Port |> Option.map (fun imp -> System.String.Join(",", imp))
      InboundTag = genericRuleObject.InboundTag }

type RoutingObject =
  { DomainStrategy: DomainStrategy option
    DomainMatcher: DomainMatcher option
    Rules: RuleObject list option }

let createRoutingObject rules domainStrategy =
  { RoutingObject.Rules = rules
    DomainMatcher = Some MinimalPerfectHash
    DomainStrategy = domainStrategy }
