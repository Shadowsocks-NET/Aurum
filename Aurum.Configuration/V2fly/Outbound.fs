module Aurum.Configuration.V2fly.Outbound

open Aurum
open Aurum.Configuration.Shared.V2fly

type OutboundProtocols =
  | VMess of VMessObject
  | VLESS of obj
  | VLite of obj
  | Shadowsocks of obj
  | Trojan of obj

type OutboundObject =
  { SendThrough: string option
    Settings: OutboundProtocols
    Tag: string
    StreamSettings: StreamSettings option
    Mux: MuxObject option }

  static member SendThrough_ = (fun a -> a.SendThrough), (fun b a -> { a with SendThrough = b })

  static member Settings_ = (fun a -> a.Settings), (fun b a -> { a with Settings = b })

  static member Tag_ = (fun a -> a.Tag), (fun b a -> { a with Tag = b })

  static member StreamSettings_ =
    (fun a -> a.StreamSettings), (fun b a -> { a with StreamSettings = b })

  static member Mux_ = (fun a -> a.Mux), (fun b a -> { a with Mux = b })

  member this.GetConnectionType() =
    let protocol =
      match this.Settings with
      | VMess _ -> "VMess"
      | VLESS _ -> "VLESS"
      | VLite _ -> "VLite"
      | Shadowsocks _ -> "Shadowsocks"
      | Trojan _ -> "Trojan"

    let network =
      Option.map
        (fun streamSetting ->
          match streamSetting.TransportSettings with
          | GRPC _ -> "+gRPC"
          | HTTP _ -> "+HTTP2"
          | KCP _ -> "+mKCP"
          | QUIC -> "+QUIC"
          | TCP _ -> "+TCP"
          | WebSocket _ -> "+WS")
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

type OutboundJsonObject =
  { SendThrough: string option
    Protocol: string
    Settings: obj
    Tag: string
    StreamSettings: StreamSettings option
    Mux: MuxObject option }
  static member FromOutboundObject(outboundObject: OutboundObject) =
    let protocol, (settings: obj) =
      match outboundObject.Settings with
      | VMess setting -> "vmess", setting
      | VLESS setting -> "vless", setting
      | VLite setting -> "vlite", setting
      | Shadowsocks setting -> "shadowsocks", setting
      | Trojan setting -> "trojan", setting

    { SendThrough = outboundObject.SendThrough
      Protocol = protocol
      Settings = settings
      Tag = outboundObject.Tag
      StreamSettings = outboundObject.StreamSettings
      Mux = outboundObject.Mux }

  member this.ToOutboundObject() =
    let settings =
      match this.Protocol with
      | "vmess" -> VMess (downcast this.Settings)
      | "vless" -> VLESS (downcast this.Settings)
      | "vlite" -> VLite (downcast this.Settings)
      | "shadowsocks" -> Shadowsocks (downcast this.Settings)
      | "trojan" -> Trojan (downcast this.Settings)
      | _ -> raise (ConfigurationParameterException $"unknown protocol type '{this.Protocol}'")

    { OutboundObject.SendThrough = this.SendThrough
      Settings = settings
      Tag = this.Tag
      StreamSettings = this.StreamSettings
      Mux = this.Mux }

let createV2flyOutboundObject (sendThrough, setting, streamSetting, tag, mux) : OutboundObject =
  { OutboundObject.SendThrough = sendThrough
    StreamSettings = streamSetting
    Settings = setting
    Mux = mux
    Tag = tag }

