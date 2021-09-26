namespace Aurum.Configurator

open System.Collections.Generic
open Aurum

module ShareLink =
    let placeholder = ""

    let encodeBase64 (text: string) =
        let plainBytes = System.Text.Encoding.UTF8.GetBytes text
        System.Convert.ToBase64String plainBytes

    let decodeBase64 (encoded: string) =
        let encodedBytes = System.Convert.FromBase64String encoded
        System.Text.Encoding.UTF8.GetString encodedBytes

    let createVMessObjectFromUri (uriObject: System.Uri) =
        let uuid = uriObject.UserInfo
        let host = uriObject.Host
        let port = uriObject.Port
        let description = uriObject.Fragment

        let queryParams =
            Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery uriObject.Query

        let retrieveFromShareLink =
            Helpers.getFirstQuerystringEntry queryParams

        let tryRetrieveFromShareLink key =
            Helpers.tryGetFirstQuerystringEntry queryParams key
            |> Helpers.blankStringToNone

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

        ()

    let decodeShareLink link =
        let uriObject = System.Uri link

        match uriObject.Scheme with
        | "vmess" -> createVMessObjectFromUri uriObject
        | _ -> raise (Exceptions.ShareLinkFormatError("unknown shared link type"))
