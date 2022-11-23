[misskey]: https://https://github.com/misskey-dev/misskey
[twitter]: https://twitter.com
[activitypub]: https://activitypub.rocks/

# F# Cloud Function Misskey Webhook

This is a simple and small project to set up an endpoint for misskey webhooks to send events about things I do when I'm navigating the website.

I plan to make a "cross-poster" from [misskey] to [twitter] since as far as I know there's none and I'd like to keep cross-posting my F# content

> # What is [MissKey]?
>
> Misskey is a social network that just as mastodon, pixelfed, and other decentralized social media, implements the [ActivityPub] protocol
> it can federate content and is compatible with mastodon up to some extent (where the spec is respected by both parties)

The idea is very straight forward grab events, parse the content and do whatever you want with it
