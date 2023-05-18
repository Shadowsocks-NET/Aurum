module Aurum.Configuration.AdapterTypes.VMessLegacyShare

open System.Text.Json.Serialization

// this implementation disregards alterId, an obsolete feature
// the only *disguise* method supported is HTTP host

type VMessSecurity =
  | [<JsonName "none">] None
  | [<JsonName "zero">] Zero
  | [<JsonName "auto">] Auto
  | [<JsonName "aes-128-gcm">] AES
  | [<JsonName "chacha20-poly1305">] ChaCha20

type VMessTransport =
  | [<JsonName "tcp">] TCP
  | [<JsonName "ws">] WebSocket
  | [<JsonName "quic">] QUIC
  | [<JsonName "grpc">] GRPC
  | [<JsonName "mkcp">] MKCP // sing-box does not support this.

type TLSSetting =
  | [<JsonName "none">] None
  | [<JsonName "tls">] TLS

type DisguiseType =
  | [<JsonName "none">] None
  | [<JsonName "http">] HTTP

type ShareObject =
  { v: int
    add: string
    port: int
    id: string
    scy: VMessSecurity
    net: VMessTransport
    tls: TLSSetting
    sni: string
    [<JsonName "type">]
    dType: DisguiseType
    host: string }
