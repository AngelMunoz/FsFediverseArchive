# MkWebHook

This is the hook that gets called whenever a misskey event fires,
I have it configured for two particular events

- Notes
- Replies

Whenever one of those fires up, my webhook gets called in GCP, this in turn makes the serverless machinery to start moving

## Gotchas

- Don't forget to write the response otherwise the function will hang up
- Events are asynchronous, so take precautions when you want to pub/sub thing sin order, account for randomness
