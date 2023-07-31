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

  let inline private getKey
    (container: ^Container when ^Container: (member TryGetValue: string * byref<obj> -> bool))
    key
    =
    let mutable output = Unchecked.defaultof<obj>

    let parsed = container.TryGetValue(key, &output)

    match parsed with
    | true -> ValueSome(output.ToString())
    | _ -> ValueNone


  let inline get query =
    let page =
      getKey query "page"
      |> ValueOption.bind ValueOption.tryParse
      |> ValueOption.defaultValue 1

    let limit =
      getKey query "limit"
      |> ValueOption.bind ValueOption.tryParse
      |> ValueOption.defaultValue 10

    match getKey query "note" with
    | ValueSome value -> Note value
    | ValueNone -> Notes { page = page; limit = limit }
