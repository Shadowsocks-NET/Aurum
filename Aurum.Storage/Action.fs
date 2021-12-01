module Aurum.Storage.Action

open Aurum
open Aether
open Aether.Operators
open Aurum.Configuration.Intermediate

type ConfigurationModificationActions =
    | Name of string identity
    | Host of string identity
    | Port of int identity
    | Configuration of string identity
    | Type of string identity

let applyConfiguration action configuration =
    match action with
    | Name action -> Optic.map SerializedServerConfiguration.Name_ action configuration
    | Host action -> Optic.map SerializedServerConfiguration.Host_ action configuration
    | Port action -> Optic.map SerializedServerConfiguration.Port_ action configuration
    | Configuration action -> Optic.map SerializedServerConfiguration.Configuration_ action configuration
    | Type action -> Optic.map SerializedServerConfiguration.Type_ action configuration
