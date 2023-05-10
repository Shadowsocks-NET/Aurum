module Aurum.Configuration.Shared.Adapter

open Aurum.Configuration.Shared.V2fly
open Aurum.Configuration.Shared.Shadowsocks

type SharedConfigObject =
  { TcpFastOpen: bool }

type Configuration =
  | V2fly of V2flyObject
  | Shadowsocks of ShadowsocksObject
