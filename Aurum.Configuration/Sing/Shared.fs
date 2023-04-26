module Aurum.Configuration.Sing.Shared

open System.Text.Json.Serialization

type TLSOutbound =
  { enabled: bool
    disableSNI: bool option Skippable
    serverName: string option Skippable
    insecure: bool option Skippable
    alpn: string list option Skippable
    minVersion: string option Skippable
    maxVersion: string option Skippable
    cipherSuites: string list option Skippable
    certificate: string option Skippable
    certificatePath: string option Skippable }

type ACMERecord =
  { domain: string list
    dataDirectory: string
    defaultServerName: string
    email: string
    provider: string option Skippable
    disableHTTPChallenge: bool option Skippable
    disableTlsAlpnChallenge: bool option Skippable
    alternativeHTTPPort: int option Skippable
    alternativeTLSPort: int option Skippable
    externalAccount: obj option Skippable }

type TLSInbound =
  { enabled: bool
    serverName: string option Skippable
    alpn: string list option Skippable
    minVersion: string option Skippable
    maxVersion: string option Skippable
    cipherSuites: string list option Skippable
    certificate: string option Skippable
    certificatePath: string option Skippable
    key: string option Skippable
    keyPath: string option Skippable
    acme: ACMERecord }
