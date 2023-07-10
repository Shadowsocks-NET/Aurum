module Aurum.Configuration.Sing.DNS

open Aurum.Configuration.Shared.Routing
open Aurum.Configuration.Sing.Routing

type DnsRuleObject =
  { Inbound: string list option
    QueryType: string list option
    Network: NetworkType option
    Protocol: RuleMatchProtocol list option
    Domain: string list option
    DomainSuffix: string list option
    DomainKeyword: string list option
    DomainRegex: string list option
    Port: int list option
    PortRange: string list option
    ProcessName: string list option
    ProcessPath: string list option
    PackageName: string list option
    ClashMode: string option
    Outbound: string list option
    Server: string
    DisableCache: bool option
    RewriteTtl: int option }

type DnsDomainRuleType =
  | Full of string list
  | Suffix of string list
  | Keyword of string list
  | Regex of string list

type DnsServerObject =
  { tag: string
    address: string }

type RuleType =
  | Block of rules: DnsDomainRuleType
  | Redirect of rules: DnsDomainRuleType * tag: string

type DnsStrategy =
  | PreferIpv4
  | PreferIpv6
  | Ipv4Only
  | Ipv6Only

type InternalDnsObject =
  { rules: RuleType list
    servers: DnsServerObject list
    final: string
    strategy: DnsStrategy }

type DnsObject =
  { rules: DnsRuleObject list
    servers: DnsServerObject list
    final: string
    strategy: DnsStrategy }

let mapDnsRuleType dnsRuleType (origObject: DnsRuleObject) =
  match dnsRuleType with
  | Full domain -> { origObject with Domain = Some domain }
  | Suffix domain -> { origObject with DomainSuffix = Some domain }
  | Keyword domain -> { origObject with DomainKeyword = Some domain }
  | Regex domain -> { origObject with DomainRegex = Some domain }

let mapRuleType ruleType =
  match ruleType with
  | Block rules ->
    { Inbound = None
      QueryType = None
      Network = None
      Protocol = None
      Domain = None
      DomainSuffix = None
      DomainKeyword = None
      DomainRegex = None
      Port = None
      PortRange = None
      ProcessName = None
      ProcessPath = None
      PackageName = None
      ClashMode = None
      Outbound = None
      Server = "block"
      DisableCache = None
      RewriteTtl = None } |> mapDnsRuleType rules
  | Redirect (rules, tag) ->
    { Inbound = None
      QueryType = None
      Network = None
      Protocol = None
      Domain = None
      DomainSuffix = None
      DomainKeyword = None
      DomainRegex = None
      Port = None
      PortRange = None
      ProcessName = None
      ProcessPath = None
      PackageName = None
      ClashMode = None
      Outbound = None
      Server = tag
      DisableCache = None
      RewriteTtl = None } |> mapDnsRuleType rules

let createDnsObject (internalDnsObject: InternalDnsObject) =
  { DnsObject.rules = List.map mapRuleType internalDnsObject.rules
    DnsObject.servers = internalDnsObject.servers
    DnsObject.final = internalDnsObject.final
    DnsObject.strategy = internalDnsObject.strategy }
