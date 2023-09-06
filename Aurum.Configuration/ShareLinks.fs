module Aurum.Configuration.ShareLinks

open System.Collections.Generic
open Microsoft.AspNetCore.WebUtilities
open Aurum
open Aurum.Configuration.Shared.Adapter
open Aurum.Configuration.Shared.Shadowsocks
open Aurum.Configuration.Shared.V2fly
open FSharpPlus
open FSharpPlus.Data


type WsTransportSettings = { Path: string; Host: string }
type GrpcTransportSettings = { ServiceName: string }

let createV2FlyObjectFromUri (uriObject: System.Uri) =
  let protocolSetting =
    match uriObject.Scheme with
    | "vmess" ->
      createVMessObject (uriObject.Host, uriObject.Port, uriObject.UserInfo, VMessSecurity.Auto)
      |> Success
    | unknown -> Failure([ ShareLinkFormatException $"unknown sharelink protocol {unknown}" ])

  let description =
    if uriObject.Fragment.Length = 0 then
      ""
    else
      uriObject.Fragment.Substring(1)

  let queryParams = QueryHelpers.ParseQuery uriObject.Query

  let retrieveFromShareLink = getFirstQuerystringEntry queryParams

  let tryRetrieveFromShareLink = tryRetrieveFromShareLink queryParams

  let securityType = tryRetrieveFromShareLink "security" |> Option.defaultValue "none"

  let securitySetting =
    match securityType with
    | "tls" ->
      createTLSObject (
        tryRetrieveFromShareLink "sni",
        tryRetrieveFromShareLink "alpn"
        |> Option.map (fun alpn -> alpn.Split(",") |> Seq.toList)
      )
      |> Success
    | "none" -> Success(TransportSecurity.None)
    | unsupported -> Failure([ ShareLinkFormatException $"unsupported security type {unsupported}" ])

  let transportType: Validation<exn list, string> =
    retrieveFromShareLink "type"
    |> Result.mapError (fun e -> [ e ])
    |> Validation.ofResult

  let transportSetting =
    Validation.bind
      (fun transportType ->
        match transportType with
        | "ws" ->
          createWebSocketObject (
            (tryRetrieveFromShareLink "path"),
            None,
            None,
            None,
            (tryRetrieveFromShareLink "host"),
            None
          )
          |> Success
        | "grpc" ->
          retrieveFromShareLink "serviceName"
          |> Result.mapError (fun e -> [ e ])
          |> Validation.ofResult
          |> Validation.map createGrpcObject
        | "http" ->
          createHttpObject (tryRetrieveFromShareLink "path", tryRetrieveFromShareLink "host", Dictionary())
          |> Success
        | "quic" -> createQuicObject () |> Success
        | "kcp" ->
          createKCPObject (None, None, None, None, None, None, None, (tryRetrieveFromShareLink "seed"))
          |> Success
        | "tcp" -> createTCPObject () |> Success
        | unknown -> Failure([ ConfigurationParameterException $"unknown transport protocol {unknown}" ]))
      transportType

  let v2flyObject =
    createV2flyObject <!> protocolSetting <*> transportSetting <*> securitySetting
    |> Validation.map V2fly

  Validation.map (createConfigurationEntry description) v2flyObject

type SsEncryptionInfo = { Method: string; PSKs: string list }

let createShadowsocksObjectFromUri (uriObject: System.Uri) =
  let host = uriObject.Host
  let port = uriObject.Port

  let queryParams = QueryHelpers.ParseQuery uriObject.Query

  let tryRetrieveFromShareLink = tryRetrieveFromShareLink queryParams

  let description =
    if uriObject.Fragment.Length = 0 then
      ""
    else
      uriObject.Fragment.Substring(1)

  let encryptionInfo =
    match
      (if uriObject.UserInfo.IndexOf(":") <> -1 then
         Array.toList (System.Uri.UnescapeDataString(uriObject.UserInfo).Split(":"))
       else
         Array.toList ((decodeBase64Url uriObject.UserInfo).Split(":")))
    with
    | protocol :: info -> { Method = protocol; PSKs = info } |> Success
    | _ -> Failure([ ShareLinkFormatException $"ill-formed user info \"{uriObject.UserInfo}\"" ])

  let method =
    Validation.bind
      (fun encryptionInfo ->
        match encryptionInfo.Method with
        | "none" -> ShadowsocksEncryption.None |> Success
        | "plain" -> ShadowsocksEncryption.Plain |> Success
        | "chacha20-poly1305" -> ShadowsocksEncryption.ChaCha20 encryptionInfo.PSKs.Head |> Success
        | "chacha20-ietf-poly1305" -> ShadowsocksEncryption.ChaCha20Ietf encryptionInfo.PSKs.Head |> Success
        | "aes-128-gcm" -> ShadowsocksEncryption.AES128 encryptionInfo.PSKs.Head |> Success
        | "aes-256-gcm" -> ShadowsocksEncryption.AES256 encryptionInfo.PSKs.Head |> Success
        | "2022-blake3-aes-128-gcm" -> ShadowsocksEncryption.AES128_2022 encryptionInfo.PSKs |> Success
        | "2022-blake3-aes-256-gcm" -> ShadowsocksEncryption.AES256_2022 encryptionInfo.PSKs |> Success
        | "2022-blake3-chacha20-poly1305" -> ShadowsocksEncryption.ChaCha20_2022 encryptionInfo.PSKs |> Success
        | "2022-blake3-chacha8-poly1305" -> ShadowsocksEncryption.ChaCha8_2022 encryptionInfo.PSKs |> Success
        | method -> Failure([ ShareLinkFormatException $"unknown Shadowsocks encryption method {method}" ]))
      encryptionInfo

  let plugin =
    tryRetrieveFromShareLink "plugin"
    |> Option.map (fun op ->
      let pluginOpt = op.Split ";" |> Array.toList

      match pluginOpt with
      | "obfs" :: opts -> SimpleObfs(System.String.Join(",", List.toArray opts)) |> Success
      | "v2ray" :: opts -> V2ray(System.String.Join(",", List.toArray opts)) |> Success
      | pluginName :: _ -> Failure([ ShareLinkFormatException $"unknown plugin {pluginName}" ])
      | unknown -> Failure([ ShareLinkFormatException $"ill-formed plugin option \"{unknown}\"" ]))
    |> sequence

  let shadowsocksObject =
    createShadowsocksObject host port <!> method <*> plugin
    |> Validation.map Shadowsocks

  Validation.map (createConfigurationEntry description) shadowsocksObject


let decodeShareLink link =
  let uriObject = System.Uri link

  match uriObject.Scheme with
  | "vmess"
  | "vless" -> createV2FlyObjectFromUri uriObject
  | "ss" -> createShadowsocksObjectFromUri uriObject
  | unknown -> Failure([ ShareLinkFormatException $"unsupported sharelink protocol {unknown}" ])
