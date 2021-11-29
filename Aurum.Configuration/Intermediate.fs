module Aurum.Configuration.Intermediate

open Aurum
open Aurum.Configuration
open Aurum.Configuration.DNS
open Aurum.Configuration.Routing

type SerializedServerConfiguration =
    { Name: string
      Host: string
      Port: int
      Configuration: string
      Type: string }

type SerializedRoutingConfiguration = { Name: string; Configuration: string }

type SerializedDNSConfiguration = { Name: string; Configuration: string }

let serializeServerConfiguration (name, server: Outbound.GenericOutboundObject<Outbound.OutboundConfigurationObject>) =
    let host, port = server.Settings.GetVnextServerInfo()
    let serverType = server.GetConnectionType()
    let configuration = serializeJson server

    { SerializedServerConfiguration.Name = name
      Type = serverType
      Host = host
      Port = port
      Configuration = configuration }

let serializeRoutingConfiguration (name, routing: RuleObject list) =
    let configuration = serializeJson routing

    { SerializedRoutingConfiguration.Name = name
      Configuration = configuration }

let serializeDNSConfiguration (name, dns: DNSObject) =
    let configuration = serializeJson dns

    { SerializedRoutingConfiguration.Name = name
      Configuration = configuration }
