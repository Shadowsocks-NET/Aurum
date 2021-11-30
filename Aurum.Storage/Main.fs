module Aurum.Storage

open System.IO
open Nanoid
open SQLite
open Aurum
open Aurum.Configuration
open Aurum.Storage.Configuration

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
        _db.CreateTable<Tags>() |> ignore

        _db.CreateTable<Connections>() |> ignore

        _db.CreateTable<Groups>() |> ignore
        _db.CreateTable<DNS>() |> ignore

        _db.CreateTable<Routing>() |> ignore

    member this.insertServerConfiguration(config: Intermediate.SerializedServerConfiguration) =
        let id = Nanoid.Generate(size = 10)

        let serverConfig =
            new Connections(config.Name, id, config.Configuration, config.Type, config.Host, config.Port.ToString())

        _db.Insert(serverConfig)
