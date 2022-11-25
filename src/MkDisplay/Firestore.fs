namespace MkDisplay.Firestore

open System
open Google.Cloud.Firestore

open MkLib
open MkLib.Decoders


type GuidConverter() =

  interface IFirestoreConverter<Guid> with
    member _.ToFirestore value = value.ToString("N")

    member _.FromFirestore value =
      match value with
      | :? string as v -> Guid.Parse(v)
      | value -> raise (ArgumentException($"Unexpected data: {value.GetType()}"))

// While we have this class in our other MkSaveNote function
// it is a simple piece we can just copy/paste as we don't need
// more intrincated logic/designs that warrant another project
// if we were going to go further with this like an aspnet app with streaming
// or more complex functionality
// we could have a separate project to avoid duplicates
[<FirestoreData; AllowNullLiteral>]
type NoteRecord() =

  [<FirestoreProperty(ConverterType = typeof<GuidConverter>)>]
  member val Id = Guid.NewGuid() with get, set

  [<FirestoreProperty>]
  member val NoteId: string = Unchecked.defaultof<string> with get, set


  [<FirestoreProperty>]
  member val Content: string = Unchecked.defaultof<string> with get, set

  [<FirestoreProperty; ServerTimestamp>]
  member val LastUpdated: DateTime = DateTime.Now with get, set

[<RequireQualifiedAccess>]
module NoteRecord =
  open MkDisplay.Types

  open Thoth.Json.Net
  open Microsoft.Extensions.Logging

  let note (logger: ILogger<_>, db: FirestoreDb) noteId = task {
    let collection = db.Collection("mk-notes")
    let! snapshot = collection.WhereEqualTo("NoteId", noteId).GetSnapshotAsync()

    return
      snapshot
      |> Seq.tryHead
      |> Option.map (fun doc -> doc.ConvertTo<NoteRecord>() |> Option.ofObj)
      |> Option.flatten
      |> Option.map (fun noteRecord ->
        match Decode.fromString Note.Decoder noteRecord.Content with
        | Ok note -> Some note
        | Error err ->
          logger.LogWarning(
            $"Failed to deserialize document FirestoreId:{noteRecord.Id} DocumentIds: {noteId} - {noteRecord.NoteId}",
            err
          )

          None)
      |> Option.flatten
  }

  let paginate (logger: ILogger<_>, db: FirestoreDb) pagination = task {
    let collection = db.Collection("mk-notes")
    let offset = pagination.limit * (pagination.page - 1)

    let! snapshot =
      collection
        .Limit(pagination.limit)
        .Offset(offset)
        .OrderByDescending("LastUpdated")
        .GetSnapshotAsync()

    return [
      for document in snapshot do
        let record =
          document.ConvertTo<NoteRecord>()
          |> Option.ofObj
          |> Option.map (fun record -> record, record.Content)

        match record with
        | Some(record, content) ->
          match Decode.fromString Note.Decoder content with
          | Ok note -> note
          | Error err ->
            logger.LogWarning
              $"Failed to deserialize document FirestoreId:{record.Id} DocumentIds: {document.Id} - {record.NoteId}"

            logger.LogDebug err
        | None -> ()
    ]
  }
