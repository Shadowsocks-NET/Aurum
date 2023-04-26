module Aurum.Configuration.Sing.Base

open Aurum.Configuration.Sing.Outbound

type ConfigurationBase =
  { dns: obj
    log: obj
    inbounds: obj list
    outbounds: Outbounds list
    route: obj
    experimental: obj }

type OocApiToken =
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
  | OocV1 of apiToken: OocApiToken // WIP
  | OocSing of apiToken: OocApiToken

type BaseConfiguration =
  { subscription: Subscriptions list
    caching: CachingPolicy
    template: ConfigurationBase }
