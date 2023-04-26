module Sing.Generator.GenerateConfig

open System.IO
open System.Text.Json
open Aurum.Helpers
open Aurum.Configuration.Sing.Base

let readConfigurationTemplate path =
  let configText = File.ReadAllText(path)
  JsonSerializer.Deserialize<BaseConfiguration>(configText, singSystemTextJsonOptions)
