module Aurum.Storage.Action

open Aurum
open Aether
open Aether.Operators

[<RequireQualifiedAccess>]
type ConfigurationModificationActions =
    | Name of string identity
    | Host of string identity
    | Port of int identity
    | Configuration of string identity
    | Type of string identity
