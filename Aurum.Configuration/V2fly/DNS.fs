module Aurum.Configuration.DNS

open System.Collections.Generic

type DomainRules = string list
type ExpectIpRules = string list

type ServerObject =
    { Address: string
      Port: int
      ClientIp: string
      SkipFallback: bool
      Domains: DomainRules
      ExpectIps: ExpectIpRules }

type DNSQueryStrategy =
    | [<JsonName("UseIP")>] UseIP
    | [<JsonName("UseIPv4")>] UseIPv4
    | [<JsonName("UseIPv6")>] UseIPv6

type Domain = string
type AddressList = string list

type DNSObject =
    { Hosts: Dictionary<Domain, AddressList> option
      Servers: ServerObject list option
      ClientIp: string option
      QueryStrategy: DNSQueryStrategy option
      DisableCache: bool option
      DisableFallback: bool option
      DisableFallbackIfMatch: bool option
      Tag: string option }
