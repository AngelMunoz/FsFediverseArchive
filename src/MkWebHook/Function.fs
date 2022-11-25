namespace MkWebHook

open System.IO
open Google.Cloud.Functions.Framework
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Thoth.Json.Net
open Google.Cloud.PubSub.V1

open MkLib
open MkLib.Encoders
open MkLib.Decoders
open System.Threading.Tasks

[<AutoOpen>]
module Patterns =

  let (|MatchHost|_|) (expected: string) (headers: IHeaderDictionary) =
    match headers.TryGetValue("X-Misskey-Host") with
    | true, value when value.ToString() = expected -> Some()
    | _, _ -> None

  let (|MatchSecret|_|) (expected: string) (headers: IHeaderDictionary) =
    match headers.TryGetValue("X-Misskey-Hook-Secret") with
    | true, value when value.ToString() = expected -> Some()
    | _, _ -> None

[<RequireQualifiedAccess>]
module Publish =
  let Note (publisher: PublisherClient, note) = task {
    let message = (Note.Encode note).ToString()
    return! publisher.PublishAsync(message)
  }

type Function(logger: ILogger<Function>, config: IConfiguration) =

  let mutable publisher: PublisherClient option = None

  let getPublisher (project: IConfigurationSection) =
    publisher
    |> Option.map (Task.FromResult)
    |> Option.defaultWith (fun _ ->
      PublisherClient.CreateAsync(TopicName(project.GetValue "Name", project.GetValue "Topic")))

  let misskeyInfo = lazy (config.GetRequiredSection("MissKeyInfo"))
  let project = lazy (config.GetRequiredSection("Project"))

  interface IHttpFunction with
    /// <summary>
    /// Logic for your function goes here.
    /// </summary>
    /// <param name="context">The HTTP context, containing the request and the response.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    member _.HandleAsync context = task {

      let validSecret = misskeyInfo.Value.GetValue("Secret")
      let validHost = misskeyInfo.Value.GetValue("Host")

      let response = context.Response

      match context.Request.Headers with
      | MatchSecret validSecret & MatchHost validHost ->
        use reader = new StreamReader(context.Request.Body)
        let! content = reader.ReadToEndAsync()

        match Decode.fromString WebHookEvent.Decoder content with
        | Ok result ->
          logger.LogInformation(
            $"Parsed Event correctly: {result.``type``} - {result.body.note.visibility}"
          )

          logger.LogInformation $"{result.body.note.text[0..100]}..."

          let! publisher = getPublisher project.Value

          let! publishId = Publish.Note(publisher, result.body.note)
          logger.LogInformation $"Published Message with id: {publishId}"
        | Error err -> logger.LogDebug err

        return! response.WriteAsync "Ok"
      | _ ->
        response.StatusCode <- 401
        return! response.WriteAsync "Not Authorized"
    }
