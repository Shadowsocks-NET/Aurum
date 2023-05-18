module Aurum.Configuration.Shared.Adapter

open Aurum.Configuration.Shared.V2fly
open Aurum.Configuration.Shared.Shadowsocks

type SharedConfigObject = { TcpFastOpen: bool }

type ConfigurationFamily =
  | V2fly of V2flyObject
  | Shadowsocks of ShadowsocksObject

type ConfigurationEntry =
  { Name: string
    Config: ConfigurationFamily }

let createConfigurationEntry (name, config) = { Name = name; Config = config }
