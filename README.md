[misskey]: https://github.com/misskey-dev/misskey
[twitter]: https://twitter.com
[activitypub]: https://activitypub.rocks/
[google cloud platform]: https://cloud.google.com
[function]: https://cloud.google.com/functions/docs/create-deploy-gcloud#functions-prepare-environment-csharp
[pub/sub]: https://cloud.google.com/pubsub/docs/publish-receive-messages-client-library?hl=en#pubsub-client-libraries-csharp
[firestore]: https://firebase.google.com/docs/firestore/query-data/get-data#c

# F# and Google Cloud Functions

This is a simple and small project to set up an endpoint for misskey webhooks every time I publish a note (toot/tweet) and I (or someone else) reply on my own notes.

The idea is quite simple:

- Publish notes in misskey
- The `MkWebHook` [function] endpoint gets hit in [google cloud platform]
- Parse the content and fire a [Pub/Sub] event
- The `MkSaveNote` function gets triggered when the a Pub/Sub topic is published from `MkWebHook` and saves the content of the note to [Firestore]
- The `MkDisplay` function grabs whatever it is currently stored in firestore and renders an HTML document with some of the information.

While arguably most of that could have been done in the same function I wanted to try to connect different services along the way, as cloud services are often more than just a tutorial kind of function where once you get out of your rails it is nothing like the tutorial, so far I find this satisfactory and it was a good learning excercise, feel free to ask and raise issues if you have questions.


> # What is [MissKey]?
>
> Misskey is a social network that just as mastodon, pixelfed, and other decentralized social media, implements the [ActivityPub] protocol
> it can federate content and is compatible with mastodon up to some extent (where the spec is respected by both parties)
