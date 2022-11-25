namespace MkSaveNote.Firestore

open System.Runtime.InteropServices
open Google.Cloud.Firestore
open System

type GuidConverter() =

  interface IFirestoreConverter<Guid> with
    member _.ToFirestore value = value.ToString("N")

    member _.FromFirestore value =
      match value with
      | :? string as v -> Guid.Parse(v)
      | value -> raise (ArgumentException($"Unexpected data: {value.GetType()}"))


[<FirestoreData>]
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

  let Save (noteRecord: NoteRecord, collection: CollectionReference, cancel) = task {
    let! docRef = collection.AddAsync(noteRecord, cancel)
    return docRef.Id
  }
