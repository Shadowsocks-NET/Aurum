module Aurum.Configuration.V2fly.Outbound

open Aurum.Configuration.Shared.V2fly

type OutboundProtocols =
  | VMess of VMessSettingObject
  | VLESS
  | Shadowsocks
  | Trojan

type OutboundObject =
  { SendThrough: string option
    Settings: OutboundProtocols
    Tag: string
    StreamSettings: StreamSettings option
    Mux: MuxObject option }

  static member SendThrough_ =
    (fun a -> a.SendThrough), (fun b a -> { a with SendThrough = b })

  static member Settings_ = (fun a -> a.Settings), (fun b a -> { a with Settings = b })

  static member Tag_ = (fun a -> a.Tag), (fun b a -> { a with Tag = b })

  static member StreamSettings_ =
    (fun a -> a.StreamSettings), (fun b a -> { a with StreamSettings = b })

  static member Mux_ = (fun a -> a.Mux), (fun b a -> { a with Mux = b })

  member this.GetConnectionType() =
    let protocol =
      match this.Settings with
      | VMess _ -> "VMess"
      | VLESS -> "VLESS"
      | Shadowsocks -> "Shadowsocks"
      | Trojan -> "Trojan"

    let network =
      Option.map
        (fun streamSetting ->
          match streamSetting.TransportSettings with
          | TransportProtocol.GRPC _ -> "+gRPC"
          | TransportProtocol.HTTP _ -> "+HTTP2"
          | TransportProtocol.KCP _ -> "+mKCP"
          | TransportProtocol.QUIC -> "+QUIC"
          | TransportProtocol.TCP _ -> "+TCP"
          | TransportProtocol.WebSocket _ -> "+WS")
        this.StreamSettings
      |> Option.defaultValue ""

    let tlsFlag =
      Option.map
        (fun streamSetting ->
          match streamSetting.SecuritySettings with
          | TransportSecurity.None -> ""
          | TransportSecurity.TLS -> "+TLS"
          | TransportSecurity.XTLS -> "+XTLS")
        this.StreamSettings
      |> Option.defaultValue ""

    protocol + network + tlsFlag

let createV2flyOutboundObject (sendThrough, setting, streamSetting, tag, mux) : OutboundObject =
  { OutboundObject.SendThrough = sendThrough
    StreamSettings = streamSetting
    Settings = setting
    Mux = mux
    Tag = tag }
