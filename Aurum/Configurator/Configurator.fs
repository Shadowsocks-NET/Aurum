namespace Aurum.Configurator

type Protocol =
    | VLESS
    | VMess
    | Shadowsocks
    | Trojan
    | NaiveProxy

type Credentials(uuid, encryption) =
    member this.uuid = uuid
    member this.encryption = encryption
    new() = Credentials("", "none")
