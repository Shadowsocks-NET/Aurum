namespace Aurum

open FSharp.Json
open SQLite
open SQLitePCL

module Group =
    type ConnectionRecordObject =
        { id: string
          name: string
          tags: string list }

    type SubscriptionType =
        | None
        | Base64
        | SIP008
        | OOCv1

    type SubscriptionObject =
        { [<JsonField("type")>]
          subscriptionType: SubscriptionType
          source: string option }

    type GroupObject =
        { id: string
          name: string
          subscription: SubscriptionType
          subscriptionSource: string option
          connections: string list (* stores id of connections belong to this group *)  }

    [<Table("Tags")>]
    type Tags(tag: string, nodeId: string) =
        [<PrimaryKey>]
        [<Column("tag")>]
        member this.tag = tag

        [<Column("nodeId")>]
        member this.nodeId = nodeId

    [<Table("Connections")>]
    type Connections(name: string, id: string, configuration: string, connType: string, host: string, port: string) =
        [<PrimaryKey>]
        [<Column("id")>]
        member this.id = id

        [<Column("name")>]
        member this.name = name

        [<Column("configuration")>]
        member this.configuration = configuration

        [<Column("type")>]
        member this.connType = connType

        [<Column("host")>]
        member this.host = host

        [<Column("port")>]
        member this.port = port
