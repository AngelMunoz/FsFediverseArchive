namespace MkWebHook

open MkWebHook.Types

open Thoth.Json.Net
open System

type Decoders =

  static member WebHookEventKindDecoder: Decoder<WebHookEventKind> =
    Decode.string
    |> Decode.andThen (function
      | "note" -> Decode.succeed Note
      | "reply" -> Decode.succeed Reply
      | value -> Decode.fail $"{value} is not a known event")

  static member EmojiDecoder: Decoder<Emoji> =
    Decode.object (fun get -> {
      name = get.Required.Field "name" Decode.string
      url = get.Required.Field "url" Decode.string
    })

  static member InstanceDecoder: Decoder<Instance> =
    Decode.object (fun get -> {
      name = get.Required.Field "name" Decode.string
      softwareName = get.Required.Field "softwareName" Decode.string
      softwareVersion = get.Required.Field "softwareVersion" Decode.string
      iconUrl = get.Required.Field "iconUrl" Decode.string
      faviconUrl = get.Required.Field "faviconUrl" Decode.string
      themeColor = get.Required.Field "themeColor" Decode.string
    })

  static member UserDecoder: Decoder<User> =
    Decode.object (fun get -> {
      id = get.Required.Field "id" Decode.string
      name = get.Required.Field "name" Decode.string
      username = get.Required.Field "username" Decode.string
      host = get.Optional.Field "host" Decode.string
      instance = get.Optional.Field "instance" Decoders.InstanceDecoder
      avatarUrl = get.Optional.Field "avatarUrl" Decode.string
      avatarBlurHash = get.Optional.Field "avatarBlurHash" Decode.string
      avatarColor = get.Optional.Field "avatarColor" Decode.string
      emojis = get.Required.Field "emojis" (Decode.array Decoders.EmojiDecoder)
      onlineStatus = get.Required.Field "onlineStatus" Decode.string
      driveCapacityOverrideMb = get.Optional.Field "driveCapacityOverrideMb" Decode.float
    })

  static member NoteDecoder: Decoder<Note> =
    Decode.object (fun get -> {
      id = get.Required.Field "id" Decode.string
      createdAt = get.Required.Field "createdAt" Decode.datetimeLocal
      userId = get.Required.Field "userId" Decode.string
      user = get.Required.Field "user" Decoders.UserDecoder
      text = get.Required.Field "text" Decode.string
      cw = get.Optional.Field "cw" Decode.string
      visibility = get.Required.Field "visibility" Decode.string
      renoteCount = get.Required.Field "renoteCount" Decode.int64
      repliesCount = get.Required.Field "repliesCount" Decode.int64
      emojis = get.Required.Field "emojis" (Decode.array Decoders.EmojiDecoder)
      fileIds = get.Required.Field "fileIds" (Decode.array Decode.string)
      replyId = get.Optional.Field "replyId" Decode.string
      renoteId = get.Optional.Field "renoteId" Decode.string
      mentions = get.Optional.Field "mentions" (Decode.array Decode.string)
      uri = get.Optional.Field "uri" Decode.string
      url = get.Optional.Field "url" Decode.string
      reply = get.Optional.Field "reply" Decoders.NoteDecoder
    })

  static member UnixMillisecondsDecoder: Decoder<DateTimeOffset> =
    Decode.int64
    |> Decode.andThen (DateTimeOffset.FromUnixTimeMilliseconds >> Decode.succeed)

  static member WebHookEventDecoder: Decoder<WebHookEvent> =
    Decode.object (fun get -> {
      hookId = get.Required.Field "hookId" Decode.string
      userId = get.Required.Field "userId" Decode.string
      eventId = get.Required.Field "eventId" Decode.guid
      createdAt = get.Required.Field "createdAt" Decoders.UnixMillisecondsDecoder
      ``type`` = get.Required.Field "type" Decoders.WebHookEventKindDecoder
      body =
        get.Required.Field
          "body"
          (Decode.object (fun get -> {|
            note = get.Required.Field "note" Decoders.NoteDecoder
          |}))
    })
