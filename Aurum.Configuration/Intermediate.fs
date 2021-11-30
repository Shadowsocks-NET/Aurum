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

    static member Name_ =
        (fun a -> a.Name), (fun b a -> { a with Name = b })

    static member Host_ =
        (fun a -> a.Host), (fun b a -> { a with Host = b })

    static member Port_ =
        (fun a -> a.Port),
        (fun b a ->
            { a with
                  SerializedServerConfiguration.Port = b })

    static member Configuration_ =
        (fun a -> a.Configuration), (fun b a -> { a with Configuration = b })

    static member Type_ =
        (fun a -> a.Type),
        (fun b a ->
            { a with
                  SerializedServerConfiguration.Type = b })

type SerializedRoutingConfiguration =
    { Name: string
      Configuration: string }
    static member Name_ =
        (fun a -> a.Name),
        (fun b a ->
            { a with
                  SerializedRoutingConfiguration.Name = b })

    static member Configuration_ =
        (fun a -> a.Configuration),
        (fun b a ->
            { a with
                  SerializedRoutingConfiguration.Configuration = b })

type SerializedDNSConfiguration =
    { Name: string
      Configuration: string }
    static member Name_ =
        (fun a -> a.Name),
        (fun b a ->
            { a with
                  SerializedRoutingConfiguration.Name = b })

    static member Configuration_ =
        (fun a -> a.Configuration),
        (fun b a ->
            { a with
                  SerializedRoutingConfiguration.Configuration = b })

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
