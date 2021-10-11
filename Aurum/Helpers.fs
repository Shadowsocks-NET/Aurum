namespace Aurum

open System.Collections.Generic
open Microsoft.Extensions.Primitives
open FSharp.Json

module Helpers =
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

    let jsonOption =
        JsonConfig.create (unformatted = true, serializeNone = Omit, jsonFieldNaming = Json.lowerCamelCase)

    let serializeJson object = Json.serializeEx jsonOption object

    let deserializeJson<'T> string = Json.deserializeEx<'T> jsonOption string
