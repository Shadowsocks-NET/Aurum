module Sing.Utils

open System.Text.Json
open System.Text.Json.Serialization

let singJsonOptions =
  JsonSerializerOptions(
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCase,
    WriteIndented = true,
    NumberHandling = JsonNumberHandling.AllowReadingFromString
  )

JsonFSharpOptions
  .ThothLike()
  .WithUnwrapOption()
  .WithUnionUnwrapRecordCases()
  .WithUnionTagName("type")
  .WithUnionTagNamingPolicy(JsonNamingPolicy.SnakeCase)
  .WithUnionFieldNamingPolicy(JsonNamingPolicy.SnakeCase)
  .AddToJsonSerializerOptions(singJsonOptions)

let decodeBase64 string =
  let rawBytes = System.Convert.FromBase64String string
  System.Text.Encoding.UTF8.GetString rawBytes

let decodeBase64Url string =
  let rawBytes = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode string
  System.Text.Encoding.UTF8.GetString rawBytes
