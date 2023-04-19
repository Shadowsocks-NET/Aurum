module Sing.Generator.GenerateConfig

open System.IO
open System.Text.Json
open Sing.Utils
open Sing.Generator.Configuration

let readConfigurationTemplate path =
  let configText = File.ReadAllText(path)
  JsonSerializer.Deserialize<BaseConfiguration>(configText, singJsonOptions)
