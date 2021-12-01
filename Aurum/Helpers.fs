namespace Aurum

open System
open System.IO
open System.Collections.Generic
open Microsoft.Extensions.Primitives
open FSharp.Json
open Aether

[<AutoOpen>]
module Helpers =
    type identity<'a> = 'a -> 'a

    let nullableToOption value =
        match value with
        | null -> None
        | value -> Some(value)

    let retrieveKeyFromDict (dict: Dictionary<'K, 'V>) key =
        try
            Ok(dict.[key])
        with
        | :? KeyNotFoundException as e -> Error(e)

    let tryRetrieveKeyFromDict (dict: Dictionary<'K, 'V>) key =
        try
            Some(dict.[key])
        with
        | :? KeyNotFoundException -> None

    let unwrapResult result =
        match result with
        | Ok some -> some
        | Error error -> raise error

    let getFirstQuerystringEntry (dict: Dictionary<string, StringValues>) (key: string) : string =
        (retrieveKeyFromDict dict key |> unwrapResult).[0]

    let tryGetFirstQuerystringEntry (dict: Dictionary<string, StringValues>) (key: string) : string option =
        match tryRetrieveKeyFromDict dict key with
        | Some value -> Some(value.[0])
        | None -> None

    let blankStringToNone (string: string option) =
        Option.filter (fun x -> x.Equals("")) string

    let mergeOptionList op1 op2 =
        match op1, op2 with
        | Some x, Some y -> Some(x @ y)
        | op1, op2 -> Option.orElse op1 op2

    let jsonOption =
        JsonConfig.create (unformatted = true, serializeNone = Omit, jsonFieldNaming = Json.lowerCamelCase)

    let serializeJson object = Json.serializeEx jsonOption object

    let deserializeJson<'T> string =
        Json.deserializeEx<'T> jsonOption string

    let getDataDirectory appName =
        let baseDirectory =
            match Runtime.InteropServices.RuntimeInformation.IsOSPlatform(Runtime.InteropServices.OSPlatform.Windows) with
            | true -> Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            | false ->
                match Runtime.InteropServices.RuntimeInformation.IsOSPlatform(Runtime.InteropServices.OSPlatform.OSX) with
                | true ->
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        "/Library/Application Support"
                    )
                | false ->
                    let xdg =
                        Option.ofObj (Environment.GetEnvironmentVariable("XDG_DATA_HOME"))

                    match xdg with
                    | Some xdg -> xdg
                    | None ->
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local/share")

        Path.Combine(baseDirectory, appName)

    let applyOptics<'a, 'b> optic (f: 'a identity) (data: 'b) =
        Optic.set optic (f <| Optic.get optic data) data
