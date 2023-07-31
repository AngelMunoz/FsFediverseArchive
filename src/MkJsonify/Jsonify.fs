namespace MkJsonify.Jsonify

open System
open Thoth.Json.Net

open MkLib
open MkLib.Clients.Types


[<RequireQualifiedAccess>]

module Encode =

  let private instance (instance: Instance) =
    Encode.object [
      "name", Encode.string instance.name
      "softwareName", Encode.string instance.softwareName
      "softwareVersion", Encode.string instance.softwareVersion
      "iconUrl", Encode.string instance.iconUrl
      "faviconUrl", Encode.string instance.faviconUrl
      "themeColor", Encode.string instance.themeColor
    ]

  let private user (user: UserVersion) =
    Encode.object [
      "instance", Encode.option instance user.instance
      "host", Encode.option Encode.string user.host
      "username", Encode.string user.username
    ]

  let rec note (initialNote: NoteVersion) =
    let reply =
      match initialNote with
      | V0 note -> note.reply |> Option.map V0
      | Current note -> note.reply |> Option.map Current

    Encode.object [
      "id", Encode.string initialNote.id
      "createdAt", Encode.datetime initialNote.createdAt
      "url", Encode.option Encode.string initialNote.url
      "reactions", Encode.map Encode.string Encode.int64 initialNote.reactions
      "text", Encode.string initialNote.text
      "user", user initialNote.user
      "reply", Encode.option note reply
    ]

  let notes (notes: NoteVersion list) = notes |> List.map note |> Encode.list
