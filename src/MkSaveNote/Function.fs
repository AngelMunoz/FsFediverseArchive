namespace MkSaveNote

open CloudNative.CloudEvents
open Google.Cloud.Functions.Framework
open Google.Events.Protobuf.Cloud.Storage.V1
open System.Threading.Tasks
open Google.Events.Protobuf.Cloud.PubSub.V1

open Microsoft.Extensions.Logging

open Thoth.Json.Net

open MkLib
open MkLib.Decoders

/// <summary>
/// A function that can be triggered in responses to changes in Google Pub/Sub.
/// </summary>
type Function(logger: ILogger<Function>) =

  interface ICloudEventFunction<MessagePublishedData> with
    /// <summary>
    /// Logic for your function goes here. Note that a CloudEvent function just consumes an event;
    /// it doesn't provide any response.
    /// </summary>
    /// <param name="cloudEvent">The CloudEvent your function should consume.</param>
    /// <param name="data">The deserialized data within the CloudEvent.</param>
    /// <param name="cancellationToken">A cancellation token that is notified if the request is aborted.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    member this.HandleAsync(cloudEvent, data, cancellationToken) =
      match data.Subscription with
      | "projects/misskey-automations/topics/mk-publish-note" ->
        let textData = data.Message.TextData |> Option.ofObj
        let textData = textData |> Option.map (Decode.fromString Note.Decoder)

        match textData with
        | Some(Ok note) -> logger.LogInformation $"Note Was parsed Correctly: {note.text[0..100]}"
        | Some(Error err) -> logger.LogDebug $"Failed to Decode data -> %s{err}"
        | None -> logger.LogDebug $"No data was found in the published event"
      | event -> logger.LogWarning $"'{event}' doesn't have any handler."

      // In this example, we don't need to perform any asynchronous operations, so we
      // just return an completed Task to conform to the interface.
      Task.CompletedTask
