module Aurum.Configuration.Shared.Share

open System.Collections.Generic
open Aurum
open Aurum.Configuration.Shared
open Aurum.Configuration.Shared.Shadowsocks

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
  let description = uriObject.Fragment

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
      V2fly.createWebSocketObject (
        (tryRetrieveFromShareLink "path"),
        None,
        None,
        None,
        (tryRetrieveFromShareLink "host"),
        None
      )
    | "grpc" ->
      retrieveFromShareLink "serviceName"
      |> V2fly.createGrpcObject
    | "http" -> V2fly.createHttpObject (tryRetrieveFromShareLink "path", tryRetrieveFromShareLink "host", Dictionary())
    | "quic" -> V2fly.createQuicObject ()
    | "kcp" -> V2fly.createKCPObject (None, None, None, None, None, None, None, (tryRetrieveFromShareLink "seed"))
    | "tcp" -> V2fly.createTCPObject None
    | unknown -> raise (ConfigurationParameterException $"unknown transport protocol {unknown}")

  let protocolSetting =
    match protocol with
    | "vmess" -> V2fly.createVMessObject (host, port, uuid)
    | unknown -> raise (ShareLinkFormatException $"unknown sharelink protocol {unknown}")

  let securitySetting =
    match securityType with
    | "tls" ->
      V2fly.createTLSObject (
        tryRetrieveFromShareLink "sni",
        tryRetrieveFromShareLink "alpn"
        |> Option.map (fun alpn -> alpn.Split(",") |> Seq.toList),
        Some false
      )
    | "none" -> V2fly.TransportSecurity.None
    | unsupported -> raise (ShareLinkFormatException $"unsupported security type {unsupported}")

  (description, protocolSetting, transportSetting, securitySetting)

let createV2flyShadowsocksObjectFromSsNET (ssServer: Shadowsocks.Models.Server) =
  let name = ssServer.Name
  let host = ssServer.Host
  let port = ssServer.Port

  let method =
    match ssServer.Method with
    | "none" -> ShadowsocksEncryption.None
    | "plain" -> ShadowsocksEncryption.Plain
    | "chacha20-poly1305" -> ShadowsocksEncryption.ChaCha20
    | "chacha20-ietf-poly1305" -> ShadowsocksEncryption.ChaCha20IETF
    | "aes-128-gcm" -> ShadowsocksEncryption.AES128
    | "aes-256-gcm" -> ShadowsocksEncryption.AES256
    | method -> raise (ShareLinkFormatException $"unknown Shadowsocks encryption method {method}")

  let password = ssServer.Password

  (name, host, port, method, password)

let decodeShareLink link =
  let uriObject = System.Uri link

  match uriObject.Scheme with
  | "vmess"
  | "vless" -> createV2FlyObjectFromUri uriObject
  //| "ss" ->
  //  let mutable ssServer = Shadowsocks.Models.Server()
  //
  //  match Shadowsocks.Models.Server.TryParse(uriObject, &ssServer) with
  //  | true -> ()
  //  | false -> raise (ShareLinkFormatException "incorrect Shadowsocks link format")
  //
  //  createV2flyShadowsocksObjectFromSsNET ssServer
  | unknown -> raise (ShareLinkFormatException $"unsupported sharelink protocol {unknown}")
