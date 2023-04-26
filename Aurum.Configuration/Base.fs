module Aurum.Configuration.Base

open Aurum.Configuration.Sing.Base
open Aurum.Configuration.V2Fly.Base

type Backend =
    | SingBox of SingBaseConfiguration
    | V2fly of V2flyBaseConfiguration
