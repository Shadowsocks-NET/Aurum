module Aurum.Configuration.Base

open Aurum.Configuration.Sing.Base
open Aurum.Configuration.V2fly.Base

type Backend =
    | SingBox of SingBaseConfiguration
    | V2fly of V2flyBaseConfiguration
