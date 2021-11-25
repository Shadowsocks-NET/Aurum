module Aurum.Configuration.DNS

open Aurum.Configuration
open System.Collections.Generic
open FSharp.Json

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
    | [<JsonUnionCase("UseIP")>] UseIP
    | [<JsonUnionCase("UseIPv4")>] UseIPv4
    | [<JsonUnionCase("UseIPv6")>] UseIPv6

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
