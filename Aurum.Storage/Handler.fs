module Aurum.Storage.Handler

open System.IO
open SQLite
open Aurum
open Aurum.Configuration
open Aurum.Configuration.Intermediate
open Aurum.Storage
open Aurum.Storage.Records
open Aurum.Storage.Action

type ApplicationName = string

type DatabasePath =
    | Standard of ApplicationName
    | Custom of string

type DatabaseHandler(databasePath) =
    let _db =
        match databasePath with
        | Standard name ->
            let dataDirecotry = getDataDirectory name
            Directory.CreateDirectory(dataDirectory)
            new SQLiteConnection(Path.Combine(dataDirectory, "database"))
        | Custom path -> new SQLiteConnection(path)

    do
        _db.CreateTable<Tags>() |> ignore
        _db.CreateTable<Connections>() |> ignore
        _db.CreateTable<Groups>() |> ignore
        _db.CreateTable<DNS>() |> ignore
        _db.CreateTable<Routing>() |> ignore

    member this.insertServerConf(config: SerializedServerConfiguration) =

        let serverRecord =
            Connections(config.Name, config.Id, config.Type, config.Host, config.Port.ToString())

        let serverConfig =
            ConnectionConfig(config.Id, Option.get config.Configuration)

        _db.Insert(serverRecord) |> ignore
        _db.Insert(serverConfig) |> ignore

    member this.updateServerConf(config, actions) =
        let updatedConfig = foldConfiguration config actions

        let serverConfig =
            Connections(
                updatedConfig.Name,
                updatedConfig.Id,
                updatedConfig.Type,
                updatedConfig.Host,
                updatedConfig.Port.ToString()
            )

        _db.Update(serverConfig) |> ignore

    member this.selectServerRecById(id) =
        let table = _db.Table<Connections>()

        let result =
            query {
                for config in table do
                    where (config.Id.Equals(id))
                    select config
                    exactlyOne
            }

        result.ToIntermediate()

    member this.selectServerRecByName(name) =
        let table = _db.Table<Connections>()

        let result =
            query {
                for config in table do
                    where (config.Name.Equals(name))
                    select config
            }

        Seq.map (fun (x: Connections) -> x.ToIntermediate()) result
        |> Seq.toList

    member this.selectServerConfString(id) =
        let table = _db.Table<ConnectionConfig>()

        query {
            for config in table do
                where (config.Id.Equals(id))
                select config.Configuration
                exactlyOne
        }

    member this.selectServerConf(id) =
        this.selectServerConfString id
        |> deserializeJson<Outbound.GenericOutboundObject<Outbound.OutboundConfigurationObject>>

    member this.insertGenericConf(config) =
        (match config.Type with
         | DNS -> _db.Insert(DNS(config.Name, config.Configuration, config.Id))
         | Routing -> _db.Insert(Routing(config.Name, config.Configuration, config.Id)))
        |> ignore

    member this.updateRoutingConf(config, actions) =
        let updatedConfig = foldGeneric config actions

        _db.Update(Routing(updatedConfig.Name, updatedConfig.Configuration, updatedConfig.Id))
        |> ignore

    member this.updateDNSConf(config, actions) =
        let updatedConfig = foldGeneric config actions

        _db.Update(DNS(updatedConfig.Name, updatedConfig.Configuration, updatedConfig.Id))
        |> ignore

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
        |> Seq.toList

    member this.selectDNSConfByName(name) =
        let table = _db.Table<DNS>()

        let result =
            query {
                for config in table do
                    where (config.Name.Equals(name))
                    select config
            }

        Seq.map (fun (x: DNS) -> x.ToIntermediate()) result
        |> Seq.toList

    member this.createGroup(group) =
        let mapping =
            Groups(group.Name, group.Name, group.Subscription, group.SubscriptionSource)

        _db.Insert(mapping) |> ignore

        List.map (fun x -> ConnGroups(group.Id, x)) group.Connections
        |> List.iter (fun x -> _db.Insert(x) |> ignore)

        ()

    member this.updateGroup(group, actions) =
        let updatedGroup = foldGroup group actions

        let mapping =
            Groups(updatedGroup.Name, updatedGroup.Name, updatedGroup.Subscription, updatedGroup.SubscriptionSource)

        _db.Update(mapping) |> ignore

    member this.insertGroupConn(groupId, connId) =
        let mapping = ConnGroups(groupId, connId)

        _db.Insert(mapping) |> ignore

    member this.removeGroupConn(connId: string) =
        _db.Delete<ConnGroups>(connId) |> ignore

    member this.selectGroupByName(name: string) =
        let groupTable = _db.Table<Groups>()
        let connGroupTable = _db.Table<ConnGroups>()

        let groups =
            query {
                for group in groupTable do
                    where (group.Name.Equals(name))
                    select group
            }

        let groupConnections =
            Seq.map
                (fun (x: Groups) ->
                    query {
                        for connGroup in connGroupTable do
                            where (connGroup.Id.Equals(x.Id))
                            select connGroup
                    })
                groups

        Seq.map2
            (fun (x: Groups) y ->
                { GroupObject.Id = x.Id
                  Name = x.Name
                  Subscription = x.Type
                  SubscriptionSource = x.Url
                  Connections =
                      Seq.map (fun (z: ConnGroups) -> z.ConnId) y
                      |> Seq.toList })
            groups
            groupConnections
        |> Seq.toList

    member this.selectGroupById(id: string) =
        let groupTable = _db.Table<Groups>()
        let connGroupTable = _db.Table<ConnGroups>()

        let group =
            query {
                for group in groupTable do
                    where (group.Id.Equals(id))
                    select group
                    exactlyOne
            }

        let groupConnections =
            query {
                for connGroup in connGroupTable do
                    where (connGroup.Id.Equals(group.Id))
                    select connGroup
            }

        { GroupObject.Id = group.Id
          Name = group.Name
          Subscription = group.Type
          SubscriptionSource = group.Url
          Connections =
              Seq.map (fun (x: ConnGroups) -> x.ConnId) groupConnections
              |> Seq.toList }
