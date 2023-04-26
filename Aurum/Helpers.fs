namespace Aurum

open System
open System.Runtime.InteropServices
open System.IO
open System.Collections.Generic
open Microsoft.Extensions.Primitives
open FSharp.Json
open System.Text.Json
open System.Text.Json.Serialization
open Aether

[<AutoOpen>]
module Helpers =
    type identity<'a> = 'a -> 'a

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

    let singSystemTextJsonOptions =
        JsonSerializerOptions(
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCase,
            WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        )

    JsonFSharpOptions
        .ThothLike()
        .WithUnwrapOption()
        .WithUnionUnwrapRecordCases()
        .WithUnionTagName("type")
        .WithUnionTagNamingPolicy(JsonNamingPolicy.SnakeCase)
        .WithUnionFieldNamingPolicy(JsonNamingPolicy.SnakeCase)
        .AddToJsonSerializerOptions(singSystemTextJsonOptions)

    let v2flySystemTextJsonOptions =
        JsonSerializerOptions(
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        )

    JsonFSharpOptions
        .ThothLike()
        .WithUnwrapOption()
        .WithUnionUnwrapRecordCases()
        .WithUnionTagName("type")
        .WithUnionTagNamingPolicy(JsonNamingPolicy.CamelCase)
        .WithUnionFieldNamingPolicy(JsonNamingPolicy.CamelCase)
        .AddToJsonSerializerOptions(v2flySystemTextJsonOptions)

    let decodeBase64 string =
        let rawBytes = System.Convert.FromBase64String string
        System.Text.Encoding.UTF8.GetString rawBytes

    let decodeBase64Url string =
        let rawBytes = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode string
        System.Text.Encoding.UTF8.GetString rawBytes

    let jsonOption =
        JsonConfig.create (unformatted = true, serializeNone = Omit, jsonFieldNaming = Json.lowerCamelCase)

    let serializeJson object = Json.serializeEx jsonOption object

    let deserializeJson<'T> string =
        Json.deserializeEx<'T> jsonOption string

    let getDataDirectory appName =
        let baseDirectory =
            if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            elif RuntimeInformation.IsOSPlatform(OSPlatform.OSX) then
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "/Library/Application Support"
                )
            else
                Option.ofObj (Environment.GetEnvironmentVariable("XDG_DATA_HOME"))
                |> Option.defaultValue (
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local/share")
                )

        Path.Combine(baseDirectory, appName)
