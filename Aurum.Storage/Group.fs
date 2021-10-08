namespace Aurum.Configurator

open FSharp.Json
open SQLite
open SQLitePCL

module Group =
    type ConnectionRecordObject =
        { Id: string
          Name: string
          Tags: string list }

    type SubscriptionType =
        | [<JsonUnionCase("none")>] None
        | [<JsonUnionCase("base64")>] Base64 // not suggested
        | [<JsonUnionCase("sip008")>] SIP008
        | [<JsonUnionCase("oocv1")>] OOCv1

    type SubscriptionObject =
        { [<JsonField("type")>]
          SubscriptionType: SubscriptionType
          Source: string option }

    type GroupObject =
        { Id: string
          Name: string
          Subscription: SubscriptionType
          SubscriptionSource: string option
          Connections: string list (* stores id of connections belong to this group *)  }

    [<Table("Tags")>]
    type Tags(tag: string, nodeId: string) =
        [<PrimaryKey>]
        [<Column("tag")>]
        member this.Tag = tag

        [<Column("nodeId")>]
        member this.NodeId = nodeId

    [<Table("Connections")>]
    type Connections
        (
            name: string,
            id: string,
            configuration: string,
            connectionType: string,
            host: string,
            port: string
        ) =
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
