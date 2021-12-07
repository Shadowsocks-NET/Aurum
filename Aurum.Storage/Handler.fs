module Aurum.Storage.Handler

open System.IO
open SQLite
open Aurum
open Aurum.Configuration
open Aurum.Configuration.Intermediate
open Aurum.Storage
open Aurum.Storage.Records

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

    member this.updateServerConf(config, actions) =
        let updatedConfig = Action.foldConfiguration config actions

        let serverConfig =
            Connections(
                updatedConfig.Name,
                updatedConfig.Id,
                updatedConfig.Configuration,
                updatedConfig.Type,
                updatedConfig.Host,
                updatedConfig.Port.ToString()
            )

        _db.Update(serverConfig)

    member this.selectServerConfById(id) =
        let table = _db.Table<Connections>()

        let result =
            query {
                for config in table do
                    where (config.Id.Equals(id))
                    select config
                    exactlyOne
            }

        result.ToIntermediate()

    member this.selectServerConfByName(name) =
        let table = _db.Table<Connections>()

        let result =
            query {
                for config in table do
                    where (config.Name.Equals(name))
                    select config
            }

        Seq.map (fun (x: Connections) -> x.ToIntermediate()) result

    member this.insertGenericConf(config) =
        match config.Type with
        | DNS -> _db.Insert(DNS(config.Name, config.Configuration, config.Id))
        | Routing -> _db.Insert(Routing(config.Name, config.Configuration, config.Id))

    member this.updateRoutingConf(config, actions) =
        let updatedConfig = Action.foldGeneric config actions

        _db.Update(Routing(updatedConfig.Name, updatedConfig.Configuration, updatedConfig.Id))

    member this.updateDNSConf(config, actions) =
        let updatedConfig = Action.foldGeneric config actions

        _db.Update(DNS(updatedConfig.Name, updatedConfig.Configuration, updatedConfig.Id))

    member this.selectRoutingConfById(id) =
        let table = _db.Table<Routing>()

        let result =
            query {
                for config in table do
                    where (config.Id.Equals(id))
                    select config
                    exactlyOne
            }

        result.ToIntermediate()

    member this.selectDNSConfById(id) =
        let table = _db.Table<DNS>()

        let result =
            query {
                for config in table do
                    where (config.Id.Equals(id))
                    select config
                    exactlyOne
            }

        result.ToIntermediate()

    member this.selectRoutingConfByName(name) =
        let table = _db.Table<Routing>()

        let result =
            query {
                for config in table do
                    where (config.Name.Equals(name))
                    select config
            }

        Seq.map (fun (x: Routing) -> x.ToIntermediate()) result

    member this.selectDNSConfByName(name) =
        let table = _db.Table<DNS>()

        let result =
            query {
                for config in table do
                    where (config.Name.Equals(name))
                    select config
            }

        Seq.map (fun (x: DNS) -> x.ToIntermediate()) result

    member this.createGroup(group) =
        let mapping =
            Groups(group.Name, group.Name, group.Subscription, group.SubscriptionSource)

        _db.Insert(mapping) |> ignore

        List.map (fun x -> ConnGroups(group.Id, x)) group.Connections
        |> List.iter (fun x -> _db.Insert(x) |> ignore)

        ()

    member this.updateGroup(group, actions) =
        let updatedGroup =
        ()
