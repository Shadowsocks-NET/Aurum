module Aurum.Configuration.Sing.Shared


type TLSOutbound =
  { enabled: bool
    disableSNI: bool option
    serverName: string option
    insecure: bool option
    alpn: string list option
    minVersion: string option
    maxVersion: string option
    cipherSuites: string list option
    certificate: string option
    certificatePath: string option }

type ACMERecord =
  { domain: string list
    dataDirectory: string
    defaultServerName: string
    email: string
    provider: string option
    disableHTTPChallenge: bool option
    disableTlsAlpnChallenge: bool option
    alternativeHTTPPort: int option
    alternativeTLSPort: int option
    externalAccount: obj option }

type TLSInbound =
  { enabled: bool
    serverName: string option
    alpn: string list option
    minVersion: string option
    maxVersion: string option
    cipherSuites: string list option
    certificate: string option
    certificatePath: string option
    key: string option
    keyPath: string option
    acme: ACMERecord }
