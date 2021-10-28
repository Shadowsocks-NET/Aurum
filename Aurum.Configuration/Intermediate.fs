namespace Aurum.Configurator

open Aurum

module Intermediate =
    type SerializedServerConfiguration =
        { Name: string
          Host: string
          Port: int
          Configuration: string
          Type: string }

    let serializeServerConfiguration
        (
            name,
            server: Outbound.GenericOutboundObject<Outbound.OutboundConfigurationObject>
        ) =
        let host, port = server.Settings.GetVnextServerInfo()
        let serverType = server.GetConnectionType()
        let configuration = Helpers.serializeJson server

        { SerializedServerConfiguration.Name = name
          Type = serverType
          Host = host
          Port = port
          Configuration = configuration }
