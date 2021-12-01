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
    | Name action -> applyOptics SerializedServerConfiguration.Name_ action configuration
    | Host action -> applyOptics SerializedServerConfiguration.Host_ action configuration
    | Port action -> applyOptics SerializedServerConfiguration.Port_ action configuration
    | Configuration action -> applyOptics SerializedServerConfiguration.Configuration_ action configuration
    | Type action -> applyOptics SerializedServerConfiguration.Type_ action configuration
