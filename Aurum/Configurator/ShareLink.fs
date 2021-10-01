namespace Aurum.Configurator

open System.Collections.Generic
open Aurum.Exceptions
open Aurum.Helpers

module ShareLink =
    let placeholder = ""

    let encodeBase64 (text: string) =
        let plainBytes = System.Text.Encoding.UTF8.GetBytes text
        System.Convert.ToBase64String plainBytes

    let decodeBase64 (encoded: string) =
        let encodedBytes = System.Convert.FromBase64String encoded
        System.Text.Encoding.UTF8.GetString encodedBytes

    let createV2FlyObjectFromUri (uriObject: System.Uri) =
        let uuid = uriObject.UserInfo
        let host = uriObject.Host
        let port = uriObject.Port
        let description = uriObject.Fragment

        let queryParams =
            Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery uriObject.Query

        let retrieveFromShareLink =
            getFirstQuerystringEntry queryParams

        let tryRetrieveFromShareLink key =
            tryGetFirstQuerystringEntry queryParams key
            |> blankStringToNone

        let transportType = retrieveFromShareLink "type"

        let transport =
            match transportType with
            | "ws" ->
                Transport.createWebSocketObject
                    (tryRetrieveFromShareLink "path")
                    None
                    None
                    None
                    (tryRetrieveFromShareLink "host")
                    None
            | "grpc" ->
                retrieveFromShareLink "serviceName"
                |> Transport.createGrpcObject
            | "http" ->
                Transport.createHttpObject
                    (tryRetrieveFromShareLink "path")
                    (tryRetrieveFromShareLink "host")
                    (Dictionary())
            | "quic" ->
                Transport.createQUICObject
                    (tryRetrieveFromShareLink "quicSecurity")
                    (tryRetrieveFromShareLink "key")
                    (tryRetrieveFromShareLink "headerType")
            | "kcp" ->
                Transport.createKCPObject
                    None
                    None
                    None
                    None
                    None
                    None
                    None
                    (tryRetrieveFromShareLink "headerType")
                    (tryRetrieveFromShareLink "seed")
            | "tcp" -> Transport.createTCPObject None
            | _ -> raise (ConfigurationParameterError "unknown transport protocol")

        ()

    let decodeShareLink link =
        let uriObject = System.Uri link

        match uriObject.Scheme with
        | "vmess"
        | "vless" -> createV2FlyObjectFromUri uriObject
        | "ss" ->
            let mutable ssServer = Shadowsocks.Models.Server()

            match Shadowsocks.Models.Server.TryParse(uriObject, &ssServer) with
            | true -> ()
            | false -> raise (ShareLinkFormatError "incorrect Shadowsocks link format")
        | _ -> raise (ShareLinkFormatError "unknown share link type")
