[<AutoOpen>]
module Aurum.Storage.Mapping

open Aurum.Configuration.Intermediate
open Aurum.Storage.Records
open SQLite

// preserved for future usage
[<Table("Tags")>]
type Tags(tag: string, nodeId: string) =
    [<PrimaryKey>]
    [<AutoIncrement>]
    [<Column("id")>]
    member this.Id = null

    [<Column("tag")>]
    member this.Tag = tag

    [<Column("nodeId")>]
    member this.NodeId = nodeId

[<Table("Connections")>]
type Connections(name: string, id: string, connectionType: string, host: string, port: string) =
    [<PrimaryKey>]
    [<Column("id")>]
    member this.Id = id

    [<Column("name")>]
    member this.Name = name

    [<Column("type")>]
    member this.Type = connectionType

    [<Column("host")>]
    member this.Host = host

    [<Column("port")>]
    member this.Port = port

    member this.ToIntermediate() =
        { SerializedServerConfiguration.Id = this.Id
          Name = this.Name
          Configuration = None
          Type = this.Type
          Host = this.Host
          Port = this.Port |> int }

    new() = Connections("", "", "", "", "")

[<Table("ConnectionConfig")>]
type ConnectionConfig(id: string, configuration: string) =
    [<PrimaryKey>]
    [<Column("id")>]
    member this.Id = id

    [<Column("configuration")>]
    [<MaxLength(512)>]
    member this.Configuration = configuration

    new() = ConnectionConfig("", "")

[<Table("Groups")>]
type Groups(name: string, id: string, subType: SubscriptionType, subUrl: string) =
    [<PrimaryKey>]
    [<Column("id")>]
    member this.Id = id

    [<Column("name")>]
    member this.Name = name

    [<Column("subscriptionType")>]
    member this.Type = subType

    [<Column("subscriptionUrl")>]
    member this.Url = subUrl

    new() = Groups("", "", SubscriptionType.None, "")

[<Table("ConnectionGroups")>]
type ConnGroups(id: string, connId: string) =
    [<Column("id")>]
    member this.Id = id

    [<PrimaryKey>]
    [<Column("connId")>]
    member this.ConnId = connId

    new() = ConnGroups("", "")


[<Table("Routing")>]
type Routing(name: string, config: string, id: string) =
    [<PrimaryKey>]
    [<Column("id")>]
    member this.Id = id

    [<Column("name")>]
    member this.Name = name

    [<Column("configuration")>]
    [<MaxLength(512)>]
    member this.Configuration = config

    member this.ToIntermediate() =
        { SerializedGenericConfiguration.Id = this.Id
          Name = this.Name
          Type = GenericConfigurationType.Routing
          Configuration = this.Configuration }

    new() = Routing("", "", "")

[<Table("DNS")>]
type DNS(name: string, config: string, id: string) =
    [<PrimaryKey>]
    [<Column("id")>]
    member this.Id = id

    [<Column("name")>]
    member this.Name = name

    [<Column("configuration")>]
    [<MaxLength(512)>]
    member this.Configuration = config

    member this.ToIntermediate() =
        { SerializedGenericConfiguration.Id = this.Id
          Name = this.Name
          Type = GenericConfigurationType.DNS
          Configuration = this.Configuration }

    new() = DNS("", "", "")
