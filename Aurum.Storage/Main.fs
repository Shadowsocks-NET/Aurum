module Aurum.Storage

open System.IO
open SQLite
open SQLitePCL
open Aurum
open Aurum.Storage

type ApplicationName = string

type DatabasePath =
    | Standard of ApplicationName
    | Custom of string

type DatabaseHandler(databasePath) =
    let _db =
        match databasePath with
        | Standard name -> new SQLiteConnection(Path.Combine(getDataDirectory name, "db"))
        | Custom path -> new SQLiteConnection(path)

    do
        _db.CreateTable<Configuration.Tags>() |> ignore

        _db.CreateTable<Configuration.Connections>()
        |> ignore

        _db.CreateTable<Configuration.Groups>() |> ignore
