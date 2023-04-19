module Sing.Generator.Configuration

open Sing.Configuration.Outbound

type ConfigurationBase =
  { dns: obj
    log: obj
    inbounds: obj list
    outbounds: Outbounds list
    route: obj
    experimental: obj }

type OOCAPIToken =
  { version: int
    baseUrl: string
    secret: string
    userId: string }

type CachingPolicy =
  | Disabled
  | TTL of int
  | Diff

type Subscriptions =
  | Base64 of url: string
  | Clash of url: string // WIP
  | OOCv1 of apiToken: OOCAPIToken // WIP
  | OOCSing of apiToken: OOCAPIToken

type BaseConfiguration =
  { subscription: Subscriptions list
    caching: CachingPolicy
    template: ConfigurationBase }
