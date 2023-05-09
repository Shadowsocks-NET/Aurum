# Import Share Links

Aurum supports importing VMessAEAD/VLESS (deprecated), Shadowsocks SIP002 (
via [Shadowsocks.NET](https://github.com/Shadowsocks-NET/Shadowsocks.NET)) and legacy v2rayN base64 share links. This
describes behaviors when importing share links.

## VMessAEAD/VLESS

VLESS is considered obsolete and is subject to removal (in the latest `v2ray@5` release), thus Aurum will not actively
support VLESS related functionalities.

VMessAEAD will be the major supported protocol, but there is some subtle difference compared to the VMessAEAD share
links standard proposal:

- Blank strings for all non-mandatory fields are silently ignored and won't throw error, instead falling back to the
  default entry value.
- gRPC's `mode` field is always silently ignored, since none of `v2ray@5` and `sing-box` supports alternative gRPC modes
  specified in the share link spec (those modes are proposed mainly by Xray teams).
- XTLS related field is ignored (as VMess does not support XTLS, and VLESS which supports that is obsolete).

Xray support is undetermined, as XTLS and its successor Reality is a largely incompatible feature which requires VLESS,
a protocol that is considered obsolete for most other implementations. Also, its developer and community had shown
public hostility for some members of the original v2fly community.

## Legacy v2rayN base64 format

The legacy v2rayN base64 share link format support generally follows
the [v2rayN spec (in Chinese)](https://github.com/2dust/v2rayN/wiki/%E5%88%86%E4%BA%AB%E9%93%BE%E6%8E%A5%E6%A0%BC%E5%BC%8F%E8%AF%B4%E6%98%8E(ver-2)),
with the following exceptions:

- None of `v2ray@5` and `sing-box` supports disguise type (other than `http` which is the only available one in TCP)
  with KCP and QUIC, thus the `type` field will always be ignored. This might cause you to not be able to connect to
  some proxy providers that still uses this feature, please be careful
- Though disguise type other than `http` is always ignored, `host` will still be respected when the transport is TCP or
  HTTP (HTTP/2). None of `v2ray@5` and `sing-box` supports custom headers on QUIC and KCP, thus `host` will be ignored
  on these transports.
