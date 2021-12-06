[<AutoOpen>]
module Aurum.Storage.Mapping

open Aurum.Configuration.Intermediate
open SQLite

type ConnectionRecordObject =
    { Id: string
      Name: string
      Tags: string }

type SubscriptionType =
    | None = 0
    | Base64 = 1 // not suggested
    | SIP008 = 2
    | OOCv1 = 3

type GroupObject =
    { Id: string
      Name: string
      Subscription: SubscriptionType
      SubscriptionSource: string
      Connections: string (* stores id of connections belong to this group *)  }

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
type Connections(name: string, id: string, configuration: string, connectionType: string, host: string, port: string) =
    [<PrimaryKey>]
    [<Column("id")>]
    member this.Id = id

    [<Column("name")>]
    member this.Name = name

    [<Column("configuration")>]
    member this.Configuration = configuration

    [<Column("type")>]
    member this.Type = connectionType

    [<Column("host")>]
    member this.Host = host

    [<Column("port")>]
    member this.Port = port

    member this.ToIntermediate() =
        { SerializedServerConfiguration.Id = this.Id
          Name = this.Name
          Configuration = this.Configuration
          Type = this.Type
          Host = this.Host
          Port = this.Port |> int }

    new() = Connections("", "", "", "", "", "")

[<Table("Groups")>]
type Groups(name: string, id: string, connectionId: string, subType: SubscriptionType, subUrl: string) =
    [<PrimaryKey>]
    [<Column("id")>]
    member this.Id = id

    [<Column("name")>]
    member this.Name = name

    [<Column("connectionId")>]
    member this.connectionId = connectionId

    [<Column("subscriptionType")>]
    member this.Type = subType

    [<Column("subscriptionUrl")>]
    member this.Url = subUrl

[<Table("Routing")>]
type Routing(name: string, config: string, id: string) =
    [<PrimaryKey>]
    [<Column("id")>]
    member this.Id = id

    [<Column("name")>]
    member this.Name = name

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

    member this.Configuration = config

    member this.ToIntermediate() =
        { SerializedGenericConfiguration.Id = this.Id
          Name = this.Name
          Type = GenericConfigurationType.DNS
          Configuration = this.Configuration }

    new() = DNS("", "", "")
