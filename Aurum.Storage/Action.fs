module Aurum.Storage.Action

open Aurum
open Aurum.Configuration.Outbound
open Aurum.Configuration.Intermediate
open Aurum.Storage.Intermediate
open Aether
open Aether.Operators

type ConfigurationModificationActions =
    | Name of string
    | Host of string
    | Port of int
    | Configuration of string identity
    | Record of GenericOutboundObject<OutboundConfigurationObject> identity
    | Type of string

let mapConfiguration configuration action =
    match action with
    | Name value -> Optic.set SerializedServerConfiguration.Name_ value configuration
    | Host value -> Optic.set SerializedServerConfiguration.Host_ value configuration
    | Port value -> Optic.set SerializedServerConfiguration.Port_ value configuration
    | Configuration action -> Optic.map SerializedServerConfiguration.Configuration_ action configuration
    | Record action ->
        Optic.map
            (SerializedServerConfiguration.Configuration_
             >-> jsonconf_)
            action
            configuration
    | Type action -> Optic.set SerializedServerConfiguration.Type_ action configuration

let foldConfiguration configuration actions =
    List.fold mapConfiguration configuration actions

type GenericAction<'a> =
    | Name of string
    | Configuration of string identity
    | Record of 'a identity

let mapGeneric configuration action =
    match action with
    | Name value -> Optic.set SerializedGenericConfiguration.Name_ value configuration
    | Configuration action -> Optic.map SerializedGenericConfiguration.Configuration_ action configuration
    | Record action ->
        Optic.map
            (SerializedGenericConfiguration.Configuration_
             >-> jsonconf_)
            action
            configuration

let foldGeneric configuration actions =
    List.fold mapGeneric configuration actions

type GroupAction =
    | Name of string
    | Subscription of SubscriptionType
    | SubscriptionSource of string

let mapGroup configuration action =
    match action with
    | Name value -> Optic.set GroupObject.Name_ value configuration
    | Subscription value -> Optic.set GroupObject.Subscription_ value configuration
    | SubscriptionSource value -> Optic.set GroupObject.SubscriptionSource_ value configuration

let foldGroup configuration actions =
    List.fold mapGroup configuration actions
