module Aurum.Storage.Handler

open System.IO
open SQLite
open Aurum
open Aurum.Configuration
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
        _db.CreateTable<Tags>() |> ignore
        _db.CreateTable<Connections>() |> ignore
        _db.CreateTable<Groups>() |> ignore
        _db.CreateTable<DNS>() |> ignore
        _db.CreateTable<Routing>() |> ignore

    member this.insertServerConf(config: Intermediate.SerializedServerConfiguration) =

        let serverConfig =
            new Connections(
                config.Name,
                config.Id,
                config.Configuration,
                config.Type,
                config.Host,
                config.Port.ToString()
            )

        _db.Insert(serverConfig)

    member this.updateServerConf(config: Intermediate.SerializedServerConfiguration, actions) =
        let serverConfig = Action.foldConfiguration config actions

        _db.Update(serverConfig)

    member this.selectServerConfById(id: string) =
        let table = _db.Table<Connections>()

        let result =
            query {
                for config in table do
                    where (config.Id.Equals(id))
                    exactlyOne
            }

        result.ToIntermediate()

    member this.selectServerConfByName(name: string) =
        let table = _db.Table<Connections>()

        let result =
            query {
                for config in table do
                    where (config.Name.Equals(name))
                    select config
            }

        Seq.map (fun (x: Connections) -> x.ToIntermediate()) result
