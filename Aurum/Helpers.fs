module Aurum.Helpers

open System
open System.Collections.Generic
open Microsoft.Extensions.Primitives

let nullableToOption (value: 'T) : 'T option =
    match value with
    | null -> None
    | value -> Some(value)

let retrieveKeyFromDict (dict: Dictionary<'K, 'V>) (key: 'K) : Result<'V, KeyNotFoundException> =
    try
        Ok(dict.[key])
    with
    | :? KeyNotFoundException as e -> Error(e)

let tryRetrieveKeyFromDict (dict: Dictionary<'K, 'V>) (key: 'K) : 'V option =
    try
        Some(dict.[key])
    with
    | :? KeyNotFoundException as e -> None

let unwrapResult (result: Result<'V, 'E>) : 'V =
    let value =
        match result with
        | Ok (some) -> some
        | Error (error) -> raise (error)

    value

let getFirstQuerystringEntry (dict: Dictionary<string, StringValues>) (key: string) : string =
    (retrieveKeyFromDict dict key |> unwrapResult).[0]

let tryGetFirstQuerystringEntry (dict: Dictionary<string, StringValues>) (key: string) : string option =
    match tryRetrieveKeyFromDict dict key with
    | Some (value) -> Some(value.[0])
    | None -> None
