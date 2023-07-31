namespace MkDisplay
// Disable warnings for implicit conversions (Request.Path (pathstring => string))
#nowarn "3391"

open System.Threading.Tasks

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging

open Google.Cloud.Functions.Framework
open Google.Cloud.Firestore

open MkLib.Clients.Firestore
open MkLib.Clients.Router

open MkDisplay.Views

type Function(logger: ILogger<Function>, config: IConfiguration) =

  let project = lazy (config.GetRequiredSection "Project")
  let mutable db: FirestoreDb option = None

  let getDatabase (project: IConfigurationSection) =
    db
    |> Option.map (Task.FromResult)
    |> Option.defaultWith (fun _ -> project.GetValue "Name" |> FirestoreDb.CreateAsync)


  interface IHttpFunction with
    /// <summary>
    /// Logic for your function goes here.
    /// </summary>
    /// <param name="context">The HTTP context, containing the request and the response.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    member _.HandleAsync context = task {
      let request = context.Request
      let response = context.Response

      match Router.get request.Query with
      | Notes pagination ->

        let! db = getDatabase project.Value

        let collection = project.Value.GetValue "FsCollectionName" |> db.Collection

        let! notes =
          try
            NoteRecord.paginate (logger, collection) pagination
          with ex ->
            logger.LogDebug($"Failed to retreive notes", ex)
            Task.FromResult(List.empty)

        response.ContentType <- "text/html;charset=utf-8"
        response.StatusCode <- 200
        return! notes |> Render.Notes pagination |> response.WriteAsync

      | Note note ->
        let! db = getDatabase project.Value

        let collection = project.Value.GetValue "FsCollectionName" |> db.Collection

        let! note =
          try
            NoteRecord.note (logger, collection) note
          with ex ->
            logger.LogDebug($"Failed to retreive notes", ex)
            Task.FromResult(None)

        match note with
        | Some note ->
          response.ContentType <- "text/html;charset=utf-8"
          response.StatusCode <- 200
          return! Render.Note note |> response.WriteAsync
        | None ->
          response.StatusCode <- 404
          return! response.WriteAsync "Not Found"
    }
