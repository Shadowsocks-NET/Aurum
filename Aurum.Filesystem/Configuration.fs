module Aurum.Filesystem.Configuration

open System.IO
open Aurum
open Aurum.Configuration.TopLevel

let flushConfig (logConf, routingConf, dnsConf, inbounds, outbounds, id) =
    let topLevel =
        { Log = logConf
          Routing = routingConf
          DNS = dnsConf
          Inbounds = inbounds
          Outbounds = outbounds }

    let serializedTopLevel = serializeJson topLevel

    let dataDirectory = getDataDirectory "Aurum"

    if not <| Directory.Exists(dataDirectory) then
        Directory.CreateDirectory(dataDirectory) |> ignore

    if
        not
        <| Directory.Exists(Path.Combine(dataDirectory, "generated"))
    then
        Directory.CreateDirectory(Path.Combine(dataDirectory, "generated"))
        |> ignore

    let confPath =
        Path.Combine(dataDirectory, "generated", $"{id}.json")

    File.WriteAllText(confPath, serializedTopLevel)

    confPath
