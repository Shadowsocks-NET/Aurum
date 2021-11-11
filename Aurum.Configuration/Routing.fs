module Aurum.Configuration.Routing

open Aurum
open Aurum.Configuration
open FSharp.Json

type DomainStrategy =
    | [<JsonUnionCase("AsIs")>] AsIs
    | [<JsonUnionCase("IPIfNonMatch")>] IPIfNonMatch
    | [<JsonUnionCase("IPOnDemand")>] IPOnDemand

type DomainMatcher =
    | [<JsonUnionCase("mph")>] MinimalPerfectHash
    | [<JsonUnionCase("linear")>] Original

type RuleObjectType = | [<JsonUnionCase("field")>] Field

type RuleObject =
    { DomainMatcher: DomainMatcher option
      Type: RuleObjectType
      Domains: string list option
      IP: string list option
      Port: string option
      SourcePort: string option }
