namespace MkDisplay.Views

#nowarn "3391"

open System
open Falco.Markup

open MkLib
open MkLib.Clients.Types

module Markdown =
  open Markdig

  let private pipeline =
    lazy (MarkdownPipelineBuilder().UseAdvancedExtensions().DisableHtml().Build())

  let toHtml (content: string) =
    Markdown.ToHtml(content, pipeline.Value)



[<RequireQualifiedAccess>]
module Icons =
  let repeat =
    lazy
      (Elem.svg [
        Attr.style "height: 24px; width: 24px"
        Svg.Attr.viewBox "0 0 24 24"
        Svg.Attr.xmlns "http://www.w3.org/2000/svg"
      ] [
        Svg.Elem.path [
          Svg.Attr.fill "currentColor"
          Svg.Attr.d
            "M6,5.75L10.25,10H7V16H13.5L15.5,18H7A2,2 0 0,1 5,16V10H1.75L6,5.75M18,18.25L13.75,14H17V8H10.5L8.5,6H17A2,2 0 0,1 19,8V14H22.25L18,18.25Z"
        ] []
      ])

  let chevronRight =
    lazy
      (Elem.svg [
        Attr.style "height: 24px; width: 24px"
        Svg.Attr.viewBox "0 0 24 24"
        Svg.Attr.xmlns "http://www.w3.org/2000/svg"
      ] [
        Svg.Elem.path [
          Svg.Attr.fill "currentColor"
          Svg.Attr.d "M8.59,16.58L13.17,12L8.59,7.41L10,6L16,12L10,18L8.59,16.58Z"
        ] []
      ])

  let chevronLeft =
    lazy
      (Elem.svg [
        Attr.style "height: 24px; width: 24px"
        Svg.Attr.viewBox "0 0 24 24"
        Svg.Attr.xmlns "http://www.w3.org/2000/svg"
      ] [
        Svg.Elem.path [
          Svg.Attr.fill "currentColor"
          Svg.Attr.d "M15.41,16.58L10.83,12L15.41,7.41L14,6L8,12L14,18L15.41,16.58Z"
        ] []
      ])

  let reply =
    lazy
      (Elem.svg [
        Attr.style "height: 24px; width: 24px"
        Svg.Attr.viewBox "0 0 24 24"
        Svg.Attr.xmlns "http://www.w3.org/2000/svg"
      ] [
        Svg.Elem.path [
          Svg.Attr.fill "currentColor"
          Svg.Attr.d
            "M9 11H18V13H9V11M18 7H6V9H18V7M22 4V22L18 18H4C2.9 18 2 17.11 2 16V4C2 2.9 2.9 2 4 2H20C21.1 2 22 2.89 22 4M20 4H4V16H18.83L20 17.17V4Z"
        ] []
      ])

type Elements =

  static member layout
    (
      content: XmlNode list,
      ?head: XmlNode list,
      ?headScripts: XmlNode list,
      ?bodyAttrs: XmlAttribute list,
      ?bodyScripts: XmlNode list
    ) =
    let inline css (value: string) = value
    let head = defaultArg head List.empty
    let headScripts = defaultArg headScripts List.empty
    let bodyAttrs = defaultArg bodyAttrs List.empty
    let bodyScripts = defaultArg bodyScripts List.empty

    Elem.html [ Attr.lang "en-US" ] [
      Elem.head [] [
        Elem.meta [ Attr.charset "UTF-8" ]
        Elem.meta [ Attr.name "viewport"; Attr.content "width=device-width, initial-scale=1.0" ]
        Elem.title [] [ Text.raw "Angel Munoz In The Fediverse" ]
        Elem.link [ Attr.rel "stylesheet"; Attr.href "https://unpkg.com/bamboo.css" ]
        Elem.link [
          Attr.rel "preload"
          Attr.href "https://media.misskey.cloud/files/816160f7-6887-43d8-afa1-12dd217e75e3.jpg"
          Attr.create "as" "image"
        ]
        yield! head
        Elem.style [] [
          Text.raw (
            css
              """:root {
  --b-link: #ff8b00;
  --b-txt: #00ffdc;
  --card-bg: #2e3440b0;
}
body {
  background-image: url('https://media.misskey.cloud/files/816160f7-6887-43d8-afa1-12dd217e75e3.jpg');
  height: 100vh;
  display: flex;
  flex-direction: column;
}
.mk-note {
  background-color: var(--card-bg);
  -webkit-backdrop-filter: blur(20px);
  backdrop-filter: blur(20px);
  padding: 1.5em;
  border: 1px solid var(--b-line);
  margin: 0.5em;
  border-radius: 8px;
}
.mk-note:hover {
  -webkit-backdrop-filter: blur(60px);
  backdrop-filter: blur(60px);
}
details {
  background-color: transparent;
  display: flex;
  flex-direction: column;
  align-items: center
}
main {
  flex: 1 0;
  height: 95vh; 
  overflow: auto;
}
summary {
  list-style: none;
}
summary p { margin: 0; }
details .mk-note { border: none; }

@media (prefers-color-scheme: light) {
  :root {
    --b-txt: #00636d;
    --card-bg: #bcd2ffb0;
    --b-bg-1: #bcd2ffb0;
  }
}
"""
          )
        ]
        yield! headScripts
      ]
      Elem.body (Attr.merge [] bodyAttrs) [ yield! content; yield! bodyScripts ]
    ]

  static member topbar(?pagination: Pagination) =
    Elem.header [
      Attr.style
        "margin-bottom: auto;
         position: sticky;
         top: 0;
         display: flex;
         justify-content: space-between;
         background-color: var(--card-bg);
         backdrop-filter: blur(10px);
         padding: 0.5em;
         margin: 0.5em;
         font-weight: bold;"
    ] [
      Elem.section [] [
        Elem.p [] [
          Elem.a [ Attr.href "https://misskey.cloud/@angelmunoz" ] [ Text.raw "@angelmunoz" ]
          Text.raw " content in the fediverse"
        ]
      ]
      match pagination with
      | Some pagination ->
        Elem.section [ Attr.style "display: flex; justify-content: space-evenly" ] [
          if pagination.page > 1 then
            Elem.a [ Attr.href $"/?page={pagination.page - 1}" ] [ Icons.chevronLeft.Value ]
          Elem.p [] [ Text.rawf " Page: %i " pagination.page ]
          Elem.a [ Attr.href $"/?page={pagination.page + 1}" ] [ Icons.chevronRight.Value ]
        ]
      | None ->
        Elem.section [ Attr.style "display: flex; justify-content: space-evenly" ] [
          Elem.a [ Attr.href $"/" ] [ Text.raw "Home" ]
        ]
    ]

  static member card
    (
      content: XmlNode list,
      ?cardAttributes: XmlAttribute list,
      ?header: XmlNode list,
      ?footer: XmlNode list
    ) =
    let cardAttributes = defaultArg cardAttributes List.empty

    Elem.section (Attr.merge [ Attr.class' "mk-note" ] cardAttributes) [
      match header with
      | Some header -> Elem.header [] header
      | None -> ()

      Elem.div [] content

      match footer with
      | Some footer -> Elem.footer [] footer
      | None -> ()
    ]

  static member noteInfobar
    (
      user: UserVersion,
      createdAt: DateTime,
      ?noteUrl: string,
      ?reactions: Map<string, int64>
    ) =
    Elem.section [] [
      match reactions with
      | Some reactions ->
        Elem.div [] [
          // reactions
          for KeyValue(reaction, count) in reactions do
            Elem.span [ Attr.style ("font-size: 1.2 em;") ] [ Text.enc reaction ]

            Elem.span [ Attr.style ("font-weight: bold;") ] [ Text.enc $"{count}" ]
        ]
      | None -> ()
      Elem.section [] [
        Elem.a [
          Attr.style "font-weight: lighter; font-size: 1.2em; display: flex;"
          match noteUrl with
          | Some noteUrl -> Attr.href noteUrl
          | None -> ()
        ] [
          let date =
            let date: DateTimeOffset = DateTime.SpecifyKind(createdAt, DateTimeKind.Local)

            date.ToOffset(TimeSpan(-6, 0, 0))

          Text.enc (date.ToString("f"))

          match noteUrl with
          | Some _ -> Icons.chevronRight.Value
          | None -> ()
        ]
        match user.instance with
        | Some instance ->
          let host = user.host |> Option.defaultValue "misskey.cloud"

          Elem.div [
            Attr.style
              $"background-color: transparent;
                display: flex;
                flex-direction: column;
                align-items: flex-start;
                backdrop-filter: blur(1px);"
          ] [
            Elem.div [] [
              Elem.span [] [ Text.raw "Reply from " ]
              Elem.a [ Attr.href $"https://{host}/@{user.username}" ] [
                Text.enc $"@{user.username}"
              ]
            ]
            Elem.div [ Attr.style "display: flex; align-items: center" ] [
              Elem.img [
                Attr.style "height: 24px; width: 24px; object-fit: contain;"
                Attr.src instance.iconUrl
              ]
              Elem.a [ Attr.href $"https://{instance.name}"; Attr.target "_blank" ] [
                Text.enc $"{instance.name}: {instance.softwareName} - {instance.softwareVersion}"
              ]

            ]
          ]
        | None -> ()
      ]
    ]

  static member reply(note: NoteVersion) =
    let markdown = Markdown.toHtml (note.text)

    Elem.details [] [
      Elem.summary [] [ Text.raw ($"{markdown[0..50]}...") ]
      Elements.card (
        [ Text.raw (markdown) ],
        header = [
          Elem.a [ Attr.href $"/?note={note.id}" ] [ Text.enc $"@{note.text[0..10]}..." ]
        ],
        cardAttributes = [ Attr.dataAttr "note-id" note.id ],
        footer = [
          Elements.noteInfobar (
            note.user,
            note.createdAt,
            ?noteUrl = note.url,
            reactions = note.reactions
          )
        ]
      )
    ]

  static member note(note: NoteVersion) =
    let text =
      match note with
      | V0 note -> note.text
      | Current note -> note.text

    let reply =
      match note with
      | V0 note -> note.reply |> Option.map V0
      | Current note -> note.reply |> Option.map Current

    let markdown = Markdown.toHtml (text)

    Elements.card (
      [
        match reply with
        | Some noteReply -> Elements.reply (noteReply)
        | None -> ()
        Elem.p [] [ Text.raw (markdown) ]
      ],
      cardAttributes = [ Attr.dataAttr "note-id" note.id ],
      header = [
        Elem.a [ Attr.href $"/?note={note.id}" ] [ Text.enc $"{note.text[0..30]}..." ]
      ],
      footer = [
        let noteUrl =

          note.url
          |> Option.defaultWith (fun _ -> $"https://misskey.cloud/notes/{note.id}")

        Elements.noteInfobar (
          note.user,
          note.createdAt,
          noteUrl = noteUrl,
          reactions = note.reactions
        )
      ]
    )

[<RequireQualifiedAccess>]
module Render =
  open type Elements

  let Notes (pagination: Pagination) (notes: NoteVersion list) =

    layout (
      [
        topbar pagination
        Elem.main [] [
          for note in notes do
            Elements.note note
        ]
      ]
    )
    |> renderHtml

  let Note (note: NoteVersion) =
    layout [ topbar (); Elem.main [] [ Elements.note note ] ] |> renderHtml
