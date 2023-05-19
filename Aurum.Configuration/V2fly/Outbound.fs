module Aurum.Configuration.V2fly.Outbound

open System.Text.Json.Serialization
open Aurum
open Aurum.Configuration.Shared
open Aurum.Configuration.Shared.Adapter
open Aurum.Configuration.Shared.V2fly
open Aurum.Configuration.Shared.Shadowsocks

[<RequireQualifiedAccess>]
type ShadowsocksEncryption =
  | [<JsonName("none")>] None
  | [<JsonName("plain")>] Plain
  | [<JsonName("chacha20-poly1305")>] ChaCha20
  | [<JsonName("chacha20-ietf-poly1305")>] ChaCha20Ietf
  | [<JsonName("aes-128-gcm")>] AES128
  | [<JsonName("aes-256-gcm")>] AES256

type V2flyShadowsocksObject =
  { Address: string
    Port: int
    Method: ShadowsocksEncryption
    Password: string }

  static member FromGenericShadowsocksObject(genericShadowsocksObject: ShadowsocksObject) =
    let method, password =
      match genericShadowsocksObject.Encryption with
      | Shadowsocks.ShadowsocksEncryption.None -> ShadowsocksEncryption.None, ""
      | Shadowsocks.ShadowsocksEncryption.Plain -> ShadowsocksEncryption.Plain, ""
      | Shadowsocks.ShadowsocksEncryption.AES128 password -> ShadowsocksEncryption.AES128, password
      | Shadowsocks.ShadowsocksEncryption.AES256 password -> ShadowsocksEncryption.AES256, password
      | Shadowsocks.ShadowsocksEncryption.ChaCha20 password -> ShadowsocksEncryption.ChaCha20, password
      | Shadowsocks.ShadowsocksEncryption.ChaCha20Ietf password -> ShadowsocksEncryption.ChaCha20Ietf, password
      | encryption -> raise (ConfigurationParameterException $"unsupported Shadowsocks encryption ${encryption}")

    { Address = genericShadowsocksObject.Host
      Port = genericShadowsocksObject.Port
      Method = method
      Password = password }

type OutboundProtocols =
  | VMess of VMessObject
  | VLESS of obj
  | VLite of obj
  | Shadowsocks of V2flyShadowsocksObject
  | Trojan of obj

type OutboundObject =
  { SendThrough: string option
    Settings: OutboundProtocols
    Tag: string
    StreamSettings: StreamSettings option
    Mux: MuxObject option }

  static member FromGenericConfigurationObject(tag, genericConfiguration: ConfigurationFamily) =
    match genericConfiguration with
    | V2fly settings ->
      match settings.Protocol with
      | Protocols.VMess protocolSettings ->
        { OutboundObject.SendThrough = None
          Settings = VMess protocolSettings
          Tag = tag
          StreamSettings = Some settings.StreamSettings
          Mux = None }
      | Protocols.VLESS ->
        { OutboundObject.SendThrough = None
          Settings = VLESS {| |} // this is a stub. VLESS support will be implemented later
          Tag = tag
          StreamSettings = Some settings.StreamSettings
          Mux = None }
    | ConfigurationFamily.Shadowsocks settings ->
      { OutboundObject.SendThrough = None
        Settings = V2flyShadowsocksObject.FromGenericShadowsocksObject settings |> Shadowsocks
        Tag = tag
        StreamSettings = None
        Mux = None }

  static member ParseGenericShadowsocksPlugin(genericConfiguration: ShadowsocksObject) =
    genericConfiguration.Plugin
    |> Option.map (fun inp ->
      match inp with
      | SimpleObfs _ -> raise (ConfigurationParameterException "simple-obfs plugin is unsupported by v2ray")
      | V2ray opt ->
        let options = opt.Split(";") |> Array.toList

        if options.Length = 0 then
          let transportSettings = createWebSocketObject (None, None, None, None, None, None)

          { TransportSettings = transportSettings
            SecuritySettings = TransportSecurity.None }
        elif options |> List.contains "tls" then
          let host = (options |> List.find (fun a -> a.IndexOf("host") <> -1)).Split("=")[1]

          let transportSettings =
            createWebSocketObject (None, None, None, Some host, None, None)

          let securitySettings = createTLSObject (Some host, None)

          { TransportSettings = transportSettings
            SecuritySettings = securitySettings }
        elif options |> List.exists (fun a -> a.IndexOf("mode") <> -1) then
          let host = (options |> List.find (fun a -> a.IndexOf("host") <> -1)).Split("=")[1]
          let transportSettings = createQuicObject ()
          let securitySettings = createTLSObject (Some host, None)

          { TransportSettings = transportSettings
            SecuritySettings = securitySettings }
        else
          raise (ConfigurationParameterException $"unknown Shadowsocks v2ray-plugin options '{opt}'"))

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
      | "vmess" -> VMess(downcast this.Settings)
      | "vless" -> VLESS(downcast this.Settings)
      | "vlite" -> VLite(downcast this.Settings)
      | "shadowsocks" -> Shadowsocks(downcast this.Settings)
      | "trojan" -> Trojan(downcast this.Settings)
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
