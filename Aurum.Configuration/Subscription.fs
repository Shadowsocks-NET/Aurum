module Aurum.Configuration.Subscription

open System.Net.Http

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

let httpClient = new HttpClient()
