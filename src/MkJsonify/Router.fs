namespace MkLib.Clients.Router

open MkLib.Clients.Types
open FsToolkit.ErrorHandling


[<Struct>]
type Route =
  | Notes of pagination: Pagination
  | Note of noteId: string

  static member FromParams page limit noteId =
    match noteId with
    | Some value -> Note value
    | None ->
      let pagination =
        match page, limit with
        | Some page, Some limit -> { page = page; limit = limit }
        | None, Some limit -> { page = 1; limit = limit }
        | Some page, None -> { page = page; limit = 10 }
        | None, None -> { page = 1; limit = 10 }

      Notes pagination

[<RequireQualifiedAccess>]
module Router =
  open Microsoft.AspNetCore.Http

  let inline get (container: IQueryCollection) =

    let page =
      match container.TryGetValue "page" with
      | true, value -> ValueOption.tryParse value
      | false, _ -> ValueNone
      |> ValueOption.defaultValue 1

    let limit =
      match container.TryGetValue "page" with
      | true, value -> ValueOption.tryParse value
      | false, _ -> ValueNone
      |> ValueOption.defaultValue 10

    match container.TryGetValue "page" with
    | true, value -> Note value
    | false, _ -> Notes { page = page; limit = limit }
