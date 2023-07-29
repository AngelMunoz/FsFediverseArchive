namespace MkLib.Clients.Router

open MkLib.Clients.Types

type Routes =
  | Notes
  | Note of noteId: string

  static member FromParams page limit =
    match page, limit with
    | Some page, Some limit -> { page = page; limit = limit }
    | None, Some limit -> { page = 1; limit = limit }
    | Some page, None -> { page = page; limit = 10 }
    | None, None -> { page = 1; limit = 10 }

[<RequireQualifiedAccess>]
module Router =
  open Elmish.UrlParser

  let get route =
    parseUrl (oneOf [ map Note (s "notes" </> str); map Notes (s "notes" </> top) ]) route

  let query route =
    parseUrl (map (Routes.FromParams) (top <?> intParam "page" <?> intParam "limit")) route
    |> function
      | Some pagination -> pagination
      | None -> { page = 1; limit = 10 }