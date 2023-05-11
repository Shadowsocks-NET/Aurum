module Aurum.Configuration.Shared.Share

open System.Collections.Generic
open Aurum
open Aurum.Configuration.Shared.Adapter
open Aurum.Configuration.Shared.Shadowsocks
open Aurum.Configuration.Shared.V2fly

type OocApiToken =
  { version: int
    baseUrl: string
    secret: string
    userId: string }

type Subscriptions =
  | Base64 of url: string
  | Clash of url: string // WIP
  | OocV1 of apiToken: OocApiToken // WIP
  | OocSing of apiToken: OocApiToken

let createV2FlyObjectFromUri (uriObject: System.Uri) =
  let protocol = uriObject.Scheme
  let uuid = uriObject.UserInfo
  let host = uriObject.Host
  let port = uriObject.Port

  let description =
    if uriObject.Fragment.Length = 0 then
      ""
    else
      uriObject.Fragment.Substring(1)

  let queryParams =
    Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery uriObject.Query

  let retrieveFromShareLink = getFirstQuerystringEntry queryParams

  let tryRetrieveFromShareLink key =
    tryGetFirstQuerystringEntry queryParams key
    |> blankStringToNone

  let transportType = retrieveFromShareLink "type"

  let securityType =
    tryRetrieveFromShareLink "security"
    |> Option.defaultValue "none"

  let transportSetting =
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
    | "grpc" ->
      retrieveFromShareLink "serviceName"
      |> createGrpcObject
    | "http" -> createHttpObject (tryRetrieveFromShareLink "path", tryRetrieveFromShareLink "host", Dictionary())
    | "quic" -> createQuicObject ()
    | "kcp" -> createKCPObject (None, None, None, None, None, None, None, (tryRetrieveFromShareLink "seed"))
    | "tcp" -> createTCPObject ()
    | unknown -> raise (ConfigurationParameterException $"unknown transport protocol {unknown}")

  let protocolSetting =
    match protocol with
    | "vmess" -> createVMessObject (host, port, uuid, VMessSecurity.Auto)
    | unknown -> raise (ShareLinkFormatException $"unknown sharelink protocol {unknown}")

  let securitySetting =
    match securityType with
    | "tls" ->
      createTLSObject (
        tryRetrieveFromShareLink "sni",
        tryRetrieveFromShareLink "alpn"
        |> Option.map (fun alpn -> alpn.Split(",") |> Seq.toList),
        Some false
      )
    | "none" -> TransportSecurity.None
    | unsupported -> raise (ShareLinkFormatException $"unsupported security type {unsupported}")

  (description, V2fly(createV2flyObject protocolSetting transportSetting securitySetting))

let createShadowsocksObjectFromUri (uriObject: System.Uri) =
  let host = uriObject.Host
  let port = uriObject.Port

  let description =
    if uriObject.Fragment.Length = 0 then
      ""
    else
      uriObject.Fragment.Substring(1)

  let protocolString :: encryptionInfo =
    if uriObject.UserInfo.IndexOf(":") <> -1 then
      Array.toList (
        System
          .Uri
          .UnescapeDataString(uriObject.UserInfo)
          .Split(":")
      )
    else
      Array.toList ((decodeBase64Url uriObject.UserInfo).Split(":"))

  let method =
    match protocolString with
    | "none" -> ShadowsocksEncryption.None
    | "plain" -> ShadowsocksEncryption.Plain
    | "chacha20-poly1305" -> ShadowsocksEncryption.ChaCha20 encryptionInfo.Head
    | "chacha20-ietf-poly1305" -> ShadowsocksEncryption.ChaCha20IETF encryptionInfo.Head
    | "aes-128-gcm" -> ShadowsocksEncryption.AES128 encryptionInfo.Head
    | "aes-256-gcm" -> ShadowsocksEncryption.AES256 encryptionInfo.Head
    | "2022-blake3-aes-128-gcm" -> ShadowsocksEncryption.AES128_2022 encryptionInfo
    | "2022-blake3-aes-256-gcm" -> ShadowsocksEncryption.AES256_2022 encryptionInfo
    | "2022-blake3-chacha20-poly1305" -> ShadowsocksEncryption.ChaCha20_2022 encryptionInfo
    | "2022-blake3-chacha8-poly1305" -> ShadowsocksEncryption.ChaCha8_2022 encryptionInfo
    | method -> raise (ShareLinkFormatException $"unknown Shadowsocks encryption method {method}")

  (description, Shadowsocks(createShadowsocksObject (host, port, method)))

let decodeShareLink link =
  let uriObject = System.Uri link

  match uriObject.Scheme with
  | "vmess"
  | "vless" -> createV2FlyObjectFromUri uriObject
  | "ss" -> createShadowsocksObjectFromUri uriObject
  | unknown -> raise (ShareLinkFormatException $"unsupported sharelink protocol {unknown}")
