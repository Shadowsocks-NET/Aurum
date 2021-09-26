# Import Share Links

For now, Aurum only supports importing VMessAEAD/VLESS (deprecated) and Shadowsocks SIP002 (via [Shadowsocks.NET](https://github.com/Shadowsocks-NET/Shadowsocks.NET)) share links. This describes behaviors when importing share links.

## VMessAEAD/VLESS

VLESS is considered obsolete (actually it overlaps too much with `Trojan`) and is subject to removal (in the upcoming `v2ray@5` release), so Aurum will gradually deprecate VLESS related functionalities, please be notified.

VMessAEAD will be the major supported protocol, but there is some subtle difference compared to the VMessAEAD share links standard proposal:

- Blank strings for all non-mandatory fields are silently ignored and won't throw error, instead falling back to the default entry value.
- gRPC's `mode` field is always silently ignored and fall back to the default `gun`.
- XTLS related field is ignored (as VMess does not support XTLS, and VLESS which supports that is obsolete).

