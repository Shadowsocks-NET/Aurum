module System.Text.Json.JsonNamingPolicy

open System.Text.Json
open Messerli.ChangeCase

type SnakeCaseNamingPolicy() =
  inherit JsonNamingPolicy()

  override this.ConvertName(name) = name.ToSnakeCase()

/// Added by Sing Configuration Generator.
///
/// This naming policy converts all names to lower-cased snake case, such as PascalCase -> pascal_case
let SnakeCase = SnakeCaseNamingPolicy()
