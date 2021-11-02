module Aurum.Interop.LogStream

open System.Text.RegularExpressions
open FSharp.Control.Reactive

type LogEntry =
    | General of string
    | Error of string
    | Warning of string
    | Info of string
    | Debug of string

let matchLog logContent =
    let regexMatcher = Regex(@"\[(Debug|Info|Warning|Error)\]")
    let result = regexMatcher.Match(logContent)

    if not result.Success then
        General logContent
    else
        match result.Captures.[0].Value with
        | "Debug" -> Debug logContent
        | "Info" -> Info logContent
        | "Warning" -> Warning logContent
        | "Error" -> Error logContent
        | _ -> General logContent

type LogEvent() =
    let event = new Event<LogEntry>()

    [<CLIEventAttribute>]
    member this.Event = event.Publish

    member this.Trigger(log) = event.Trigger(matchLog log)
