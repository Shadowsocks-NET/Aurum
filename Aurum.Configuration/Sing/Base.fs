module Aurum.Configuration.Sing.Base

open Aurum.Configuration.Sing.Outbound

type SingBaseConfiguration =
  { dns: obj
    log: obj
    inbounds: obj list
    outbounds: Outbounds list
    route: obj
    experimental: obj }
