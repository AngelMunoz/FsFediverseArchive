namespace MkSaveNote

open Google.Cloud.Functions.Framework
open Google.Cloud.Firestore
open System.Threading.Tasks
open Google.Events.Protobuf.Cloud.PubSub.V1

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging

open Thoth.Json.Net

open MkLib
open MkLib.Decoders

open MkSaveNote.Firestore

/// <summary>
/// A function that can be triggered in responses to changes in Google Pub/Sub.
/// </summary>
type Function(logger: ILogger<Function>, config: IConfiguration) =

  let mutable db: FirestoreDb option = None

  let getDatabase (project: IConfigurationSection) =
    db
    |> Option.map (Task.FromResult)
    |> Option.defaultWith (fun _ -> project.GetValue "Name" |> FirestoreDb.CreateAsync)

  let project = lazy (config.GetRequiredSection "Project")

  interface ICloudEventFunction<MessagePublishedData> with
    /// <summary>
    /// Logic for your function goes here. Note that a CloudEvent function just consumes an event;
    /// it doesn't provide any response.
    /// </summary>
    /// <param name="cloudEvent">The CloudEvent your function should consume.</param>
    /// <param name="data">The deserialized data within the CloudEvent.</param>
    /// <param name="cancellationToken">A cancellation token that is notified if the request is aborted.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    member _.HandleAsync(cloudEvent, data, cancellationToken) = task {
      logger.LogInformation $"Handling event with id: {cloudEvent.Id}"

      let textData =
        data.Message
        |> Option.ofObj
        |> Option.map (fun msg -> msg.TextData |> Option.ofObj)
        |> Option.flatten
        |> Option.map (Decode.fromString Note.Decoder)

      match textData with
      | Some(Ok note) ->
        logger.LogInformation $"Note Was parsed Correctly: {note.text[0..100]}"


        let! db = getDatabase project.Value

        let collection = project.Value.GetValue "FsCollectionName" |> db.Collection

        try
          let! id =
            NoteRecord.Save(
              NoteRecord(NoteId = note.id, Content = data.Message.TextData),
              collection,
              cancellationToken
            )

          logger.LogInformation $"Saved document with id: {id}"
        with ex ->
          logger.LogError(ex.Message, ex)

      | Some(Error err) -> logger.LogWarning $"Failed to Decode data -> %s{err}"
      | None -> logger.LogWarning $"No data was found in the published event"

      return! Task.CompletedTask
    }
