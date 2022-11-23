namespace MkLib

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

[<Struct>]
type FileProperties = { width: float; height: float }

type File = {
  id: string
  createdAt: DateTime
  name: string
  ``type``: string
  md5: string
  size: float
  isSensitive: bool
  blurhash: string
  properties: FileProperties
  url: string
  thumbnailUrl: string
  comment: string option
  folderId: string option
  folder: string option
  userId: string option
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
  reactions: Map<string, int64>
  fileIds: string array
  files: File array
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
