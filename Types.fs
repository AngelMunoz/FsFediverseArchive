module MkWebHook.Types

open System

type WebHookEventKind =
  | Note
  | Reply

type Emoji = { name: string; url: string }

type Instance = {
  name: string
  softwareName: string
  softwareVersion: string
  iconUrl: string
  faviconUrl: string
  themeColor: string
}

type User = {
  id: string
  name: string
  username: string
  host: string option
  avatarUrl: string option
  avatarBlurHash: string option
  avatarColor: string option
  instance: Instance option
  emojis: Emoji array
  onlineStatus: string
  driveCapacityOverrideMb: float option
}

type Note = {
  id: string
  createdAt: DateTime
  userId: string
  user: User
  text: string
  cw: string option
  visibility: string
  renoteCount: int64
  repliesCount: int64
  emojis: Emoji array
  fileIds: string array
  replyId: string option
  renoteId: string option
  mentions: string array option
  reply: Note option
  uri: string option
  url: string option
}

type WebHookEvent = {
  hookId: string
  userId: string
  eventId: Guid
  createdAt: DateTimeOffset
  ``type``: WebHookEventKind
  body: {| note: Note |}
}
