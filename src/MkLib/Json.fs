namespace MkLib

open System
open Thoth.Json.Net

module Encoders =

  type Emoji with

    static member Encode: Encoder<Emoji> =
      fun (emoji: Emoji) ->
        Encode.object [ "name", Encode.string emoji.name; "url", Encode.string emoji.url ]

  type Instance with

    static member Encode: Encoder<Instance> =
      fun (instance: Instance) ->
        Encode.object [
          "name", Encode.string instance.name
          "softwareName", Encode.string instance.softwareName
          "softwareVersion", Encode.string instance.softwareVersion
          "iconUrl", Encode.string instance.iconUrl
          "faviconUrl", Encode.string instance.faviconUrl
          "themeColor", Encode.string instance.themeColor
        ]

  type FileProperties with

    static member Encode: Encoder<FileProperties> =
      fun (props: FileProperties) ->
        Encode.object [ "width", Encode.float props.width; "height", Encode.float props.height ]

  type File with

    static member Encode: Encoder<File> =
      fun (file: File) ->
        Encode.object [
          "id", Encode.string file.id
          "createdAt", Encode.datetime file.createdAt
          "name", Encode.string file.name
          "type", Encode.string file.``type``
          "md5", Encode.string file.md5
          "size", Encode.float file.size
          "isSensitive", Encode.bool file.isSensitive
          "blurhash", Encode.string file.blurhash
          "properties", FileProperties.Encode file.properties
          "url", Encode.string file.url
          "thumbnailUrl", Encode.string file.thumbnailUrl
          match file.comment with
          | Some comment -> "comment", Encode.string comment
          | None -> ()
          match file.folderId with
          | Some folderId -> "folderId", Encode.string folderId
          | None -> ()
          match file.folder with
          | Some folder -> "folder", Encode.string folder
          | None -> ()
          match file.userId with
          | Some userId -> "userId", Encode.string userId
          | None -> ()
        ]

  type User with

    static member Encode: Encoder<User> =
      fun (user: User) ->
        Encode.object [
          "id", Encode.string user.id
          "name", Encode.string user.name
          "username", Encode.string user.username
          match user.host with
          | Some host -> "host", Encode.string host
          | None -> ()
          match user.avatarUrl with
          | Some avatarUrl -> "avatarUrl", Encode.string avatarUrl
          | None -> ()
          match user.avatarBlurHash with
          | Some avatarBlurHash -> "avatarBlurHash", Encode.string avatarBlurHash
          | None -> ()
          match user.avatarColor with
          | Some avatarColor -> "avatarColor", Encode.string avatarColor
          | None -> ()
          match user.instance with
          | Some instance -> "instance", Instance.Encode instance
          | None -> ()
          "onlineStatus", Encode.string user.onlineStatus
          "isBot", Encode.bool user.isBot
          "isCat", Encode.bool user.isCat
        ]

  type Note with

    static member Encode: Encoder<Note> =
      fun (note: Note) ->
        Encode.object [
          "id", Encode.string note.id
          "createdAt", Encode.datetime note.createdAt
          "userId", Encode.string note.userId
          "user", User.Encode note.user
          "text", Encode.string note.text
          match note.cw with
          | Some cw -> "cw", Encode.string cw
          | None -> ()
          "visibility", Encode.string note.visibility
          "renoteCount", Encode.int64 note.renoteCount
          "repliesCount", Encode.int64 note.repliesCount
          "reactions", note.reactions |> Map.map (fun _ v -> Encode.int64 v) |> Encode.dict
          "fileIds", note.fileIds |> Array.map Encode.string |> Encode.array
          "files", note.files |> Array.map File.Encode |> Encode.array
          match note.replyId with
          | Some replyId -> "replyId", Encode.string replyId
          | None -> ()
          match note.renoteId with
          | Some renoteId -> "renoteId", Encode.string renoteId
          | None -> ()
          match note.mentions with
          | Some mentions -> "mentions", mentions |> Array.map Encode.string |> Encode.array
          | None -> ()
          match note.reply with
          | Some reply -> "reply", Note.Encode reply
          | None -> ()
          match note.uri with
          | Some uri -> "uri", Encode.string uri
          | None -> ()
          match note.url with
          | Some url -> "url", Encode.string url
          | None -> ()
          match note.myReaction with
          | Some myReaction -> "myReaction", Encode.string myReaction
          | None -> ()
        ]

module Decoders =

  let UnixMilliseconds: Decoder<DateTimeOffset> =
    Decode.int64
    |> Decode.andThen (DateTimeOffset.FromUnixTimeMilliseconds >> Decode.succeed)

  type WebHookEventKind with

    static member Decoder: Decoder<WebHookEventKind> =
      Decode.string
      |> Decode.andThen (function
        | "note" -> Decode.succeed Note
        | "reply" -> Decode.succeed Reply
        | value -> Decode.fail $"{value} is not a known event")

  type Emoji with

    static member Decoder: Decoder<Emoji> =
      Decode.object (fun get -> {
        name = get.Required.Field "name" Decode.string
        url = get.Required.Field "url" Decode.string
      })

  type Instance with

    static member Decoder: Decoder<Instance> =
      Decode.object (fun get -> {
        name = get.Required.Field "name" Decode.string
        softwareName = get.Required.Field "softwareName" Decode.string
        softwareVersion = get.Required.Field "softwareVersion" Decode.string
        iconUrl = get.Required.Field "iconUrl" Decode.string
        faviconUrl = get.Required.Field "faviconUrl" Decode.string
        themeColor = get.Required.Field "themeColor" Decode.string
      })

  type User with

    static member Decoder: Decoder<User> =
      Decode.object (fun get -> {
        id = get.Required.Field "id" Decode.string
        name = get.Required.Field "name" Decode.string
        username = get.Required.Field "username" Decode.string
        host = get.Optional.Field "host" Decode.string
        instance = get.Optional.Field "instance" Instance.Decoder
        avatarUrl = get.Optional.Field "avatarUrl" Decode.string
        avatarBlurHash = get.Optional.Field "avatarBlurHash" Decode.string
        avatarColor = get.Optional.Field "avatarColor" Decode.string
        onlineStatus = get.Required.Field "onlineStatus" Decode.string
        isBot = get.Required.Field "isBot" Decode.bool
        isCat = get.Required.Field "isCat" Decode.bool
      })

  type FileProperties with

    static member Decoder: Decoder<FileProperties> =
      Decode.object (fun get -> {
        width = get.Required.Field "width" Decode.float
        height = get.Required.Field "height" Decode.float
      })

  type File with

    static member Decoder: Decoder<File> =
      Decode.object (fun get -> {
        id = get.Required.Field "id" Decode.string
        createdAt = get.Required.Field "createdAt" Decode.datetimeLocal
        name = get.Required.Field "name" Decode.string
        ``type`` = get.Required.Field "type" Decode.string
        md5 = get.Required.Field "md5" Decode.string
        size = get.Required.Field "size" Decode.float
        isSensitive = get.Required.Field "isSensitive" Decode.bool
        blurhash = get.Required.Field "blurhash" Decode.string
        properties = get.Required.Field "properties" FileProperties.Decoder
        url = get.Required.Field "url" Decode.string
        thumbnailUrl = get.Required.Field "thumbnailUrl" Decode.string
        comment = get.Optional.Field "comment" Decode.string
        folderId = get.Optional.Field "folderId" Decode.string
        folder = get.Optional.Field "folder" Decode.string
        userId = get.Optional.Field "userId" Decode.string
      })



  type Note with

    static member Decoder: Decoder<Note> =
      Decode.object (fun get -> {
        id = get.Required.Field "id" Decode.string
        createdAt = get.Required.Field "createdAt" Decode.datetimeLocal
        userId = get.Required.Field "userId" Decode.string
        user = get.Required.Field "user" User.Decoder
        text = get.Required.Field "text" Decode.string
        cw = get.Optional.Field "cw" Decode.string
        visibility = get.Required.Field "visibility" Decode.string
        renoteCount = get.Required.Field "renoteCount" Decode.int64
        repliesCount = get.Required.Field "repliesCount" Decode.int64
        reactions = get.Required.Field "reactions" (Decode.dict Decode.int64)
        files = get.Required.Field "files" (Decode.array File.Decoder)
        fileIds = get.Required.Field "fileIds" (Decode.array Decode.string)
        replyId = get.Optional.Field "replyId" Decode.string
        renoteId = get.Optional.Field "renoteId" Decode.string
        mentions = get.Optional.Field "mentions" (Decode.array Decode.string)
        uri = get.Optional.Field "uri" Decode.string
        url = get.Optional.Field "url" Decode.string
        reply = get.Optional.Field "reply" Note.Decoder
        myReaction = get.Optional.Field "myReaction" Decode.string
      })


  type WebHookEvent with

    static member Decoder: Decoder<WebHookEvent> =
      Decode.object (fun get -> {
        hookId = get.Required.Field "hookId" Decode.string
        userId = get.Required.Field "userId" Decode.string
        eventId = get.Required.Field "eventId" Decode.guid
        createdAt = get.Required.Field "createdAt" UnixMilliseconds
        ``type`` = get.Required.Field "type" WebHookEventKind.Decoder
        body =
          get.Required.Field
            "body"
            (Decode.object (fun get -> {|
              note = get.Required.Field "note" Note.Decoder
            |}))
      })

  module V0 =
    open Encoders

    type V0.User with

      static member Decoder: Decoder<V0.User> =
        Decode.object (fun get -> {
          id = get.Required.Field "id" Decode.string
          name = get.Required.Field "name" Decode.string
          username = get.Required.Field "username" Decode.string
          host = get.Optional.Field "host" Decode.string
          instance = get.Optional.Field "instance" Instance.Decoder
          avatarUrl = get.Optional.Field "avatarUrl" Decode.string
          avatarBlurHash = get.Optional.Field "avatarBlurHash" Decode.string
          avatarColor = get.Optional.Field "avatarColor" Decode.string
          emojis = get.Required.Field "emojis" (Decode.array Emoji.Decoder)
          onlineStatus = get.Required.Field "onlineStatus" Decode.string
          driveCapacityOverrideMb = get.Optional.Field "driveCapacityOverrideMb" Decode.float
        })

    type V0.Note with

      static member Decoder: Decoder<V0.Note> =
        Decode.object (fun get -> {
          id = get.Required.Field "id" Decode.string
          createdAt = get.Required.Field "createdAt" Decode.datetimeLocal
          userId = get.Required.Field "userId" Decode.string
          user = get.Required.Field "user" V0.User.Decoder
          text = get.Required.Field "text" Decode.string
          cw = get.Optional.Field "cw" Decode.string
          visibility = get.Required.Field "visibility" Decode.string
          renoteCount = get.Required.Field "renoteCount" Decode.int64
          repliesCount = get.Required.Field "repliesCount" Decode.int64
          emojis = get.Required.Field "emojis" (Decode.array Emoji.Decoder)
          reactions = get.Required.Field "reactions" (Decode.dict Decode.int64)
          files = get.Required.Field "files" (Decode.array File.Decoder)
          fileIds = get.Required.Field "fileIds" (Decode.array Decode.string)
          replyId = get.Optional.Field "replyId" Decode.string
          renoteId = get.Optional.Field "renoteId" Decode.string
          mentions = get.Optional.Field "mentions" (Decode.array Decode.string)
          uri = get.Optional.Field "uri" Decode.string
          url = get.Optional.Field "url" Decode.string
          reply = get.Optional.Field "reply" V0.Note.Decoder
        })
