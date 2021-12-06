module Aurum.Storage.Handler

open System.IO
open SQLite
open Aurum
open Aurum.Configuration
open Aurum.Configuration.Intermediate
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

    member this.insertServerConf(config: SerializedServerConfiguration) =

        let serverConfig =
            Connections(config.Name, config.Id, config.Configuration, config.Type, config.Host, config.Port.ToString())

        _db.Insert(serverConfig)

    member this.updateServerConf(config: SerializedServerConfiguration, actions) =
        let serverConfig = Action.foldConfiguration config actions

        _db.Update(serverConfig)

    member this.selectServerConfById(id: string) =
        let table = _db.Table<Connections>()

        let result =
            query {
                for config in table do
                    where (config.Id.Equals(id))
                    select config
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

    member this.insertGenericConf(config: SerializedGenericConfiguration) =
        match config.Type with
        | DNS -> _db.Insert(DNS(config.Name, config.Configuration, config.Id))
        | Routing -> _db.Insert(Routing(config.Name, config.Configuration, config.Id))

    member this.selectRoutingConfById(id: string, confType: GenericConfigurationType) =
        let table = _db.Table<Routing>()

        let result =
            query {
                for config in table do
                    where (config.Id.Equals(id))
                    select config
                    exactlyOne
            }

        result.ToIntermediate()

    member this.selectDNSConfById(id: string, confType: GenericConfigurationType) =
        let table = _db.Table<DNS>()

        let result =
            query {
                for config in table do
                    where (config.Id.Equals(id))
                    select config
                    exactlyOne
            }

        result.ToIntermediate()

    member this.selectRoutingConfByName(name: string) =
        let table = _db.Table<Routing>()

        let result =
            query {
                for config in table do
                    where (config.Name.Equals(name))
                    select config
            }

        Seq.map (fun (x: Routing) -> x.ToIntermediate()) result

    member this.selectDNSConfByName(name: string) =
        let table = _db.Table<DNS>()

        let result =
            query {
                for config in table do
                    where (config.Name.Equals(name))
                    select config
            }

        Seq.map (fun (x: DNS) -> x.ToIntermediate()) result
