module Aurum.Storage.Action

open Aurum
open Aurum.Configuration
open Aurum.Configuration.Intermediate
open Aether
open Aether.Operators

type ConfigurationModificationActions =
    | Name of string identity
    | Host of string identity
    | Port of int identity
    | Configuration of string identity
    | Record of Outbound.GenericOutboundObject<Outbound.OutboundConfigurationObject> identity
    | Type of string identity

let mapConfiguration configuration action =
    match action with
    | Name action -> Optic.map SerializedServerConfiguration.Name_ action configuration
    | Host action -> Optic.map SerializedServerConfiguration.Host_ action configuration
    | Port action -> Optic.map SerializedServerConfiguration.Port_ action configuration
    | Configuration action -> Optic.map SerializedServerConfiguration.Configuration_ action configuration
    | Record action ->
        Optic.map
            (SerializedServerConfiguration.Configuration_
             >-> jsonconf_)
            action
            configuration
    | Type action -> Optic.map SerializedServerConfiguration.Type_ action configuration

let foldConfiguration configuration actions =
    List.fold mapConfiguration configuration actions

type GenericAction<'a> =
    | Name of string identity
    | Configuration of string identity
    | Record of 'a identity

let mapGeneric configuration action =
    match action with
    | Name action -> Optic.map SerializedGenericConfiguration.Name_ action configuration
    | Configuration action -> Optic.map SerializedGenericConfiguration.Configuration_ action configuration
    | Record action ->
        Optic.map
            (SerializedGenericConfiguration.Configuration_
             >-> jsonconf_)
            action
            configuration

let foldGeneric configuration actions =
    List.fold mapGeneric configuration actions
