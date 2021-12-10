module Aurum.Configuration.Intermediate

open Aurum
open Aurum.Configuration
open Aurum.Configuration.DNS
open Aurum.Configuration.Routing
open Nanoid

type SerializedServerConfiguration =
    { Id: string
      Name: string
      Host: string
      Port: int
      Configuration: string option
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

type GenericConfigurationType =
    | Routing
    | DNS

type SerializedGenericConfiguration =
    { Id: string
      Name: string
      Type: GenericConfigurationType
      Configuration: string }
    static member Name_ =
        (fun a -> a.Name),
        (fun b a ->
            { a with
                  SerializedGenericConfiguration.Name = b })

    static member Configuration_ =
        (fun a -> a.Configuration),
        (fun b a ->
            { a with
                  SerializedGenericConfiguration.Configuration = b })

let jsonconf_: (string -> 'a) * ('a -> string) = // isomorphism for JSON <-> record conversion
    deserializeJson, serializeJson

let serializeServerConfiguration (name, server: Outbound.GenericOutboundObject<Outbound.OutboundConfigurationObject>) =
    let host, port = server.Settings.GetVnextServerInfo()
    let serverType = server.GetConnectionType()
    let configuration = serializeJson server
    let id = Nanoid.Generate(size = 10)

    { SerializedServerConfiguration.Name = name
      Id = id
      Type = serverType
      Host = host
      Port = port
      Configuration = Some configuration }

let serializeRoutingConfiguration (name, routing: RuleObject list) =
    let configuration = serializeJson routing
    let id = Nanoid.Generate(size = 10)

    { SerializedGenericConfiguration.Name = name
      Id = id
      Type = Routing
      Configuration = configuration }

let serializeDNSConfiguration (name, dns: DNSObject) =
    let configuration = serializeJson dns
    let id = Nanoid.Generate(size = 10)

    { SerializedGenericConfiguration.Name = name
      Id = id
      Type = DNS
      Configuration = configuration }
