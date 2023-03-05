namespace MkDisplay.Types

open MkLib

[<Struct>]
type Pagination = { page: int; limit: int }

[<RequireQualifiedAccess>]
type UserVersion =
  | V0 of V0.User
  | Current of User

  member this.instance =
    match this with
    | V0 user -> user.instance
    | Current user -> user.instance

  member this.host =
    match this with
    | V0 user -> user.host
    | Current user -> user.host

  member this.username =
    match this with
    | V0 user -> user.username
    | Current user -> user.username

type NoteVersion =
  | V0 of V0.Note
  | Current of Note

  member this.id =
    match this with
    | V0 note -> note.id
    | Current note -> note.id

  member this.createdAt =
    match this with
    | V0 note -> note.createdAt
    | Current note -> note.createdAt

  member this.url =
    match this with
    | V0 note -> note.url
    | Current note -> note.url

  member this.reactions =
    match this with
    | V0 note -> note.reactions
    | Current note -> note.reactions

  member this.text =
    match this with
    | V0 note -> note.text
    | Current note -> note.text

  member this.user =
    match this with
    | V0 note -> UserVersion.V0 note.user
    | Current note -> UserVersion.Current note.user
