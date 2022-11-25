# MkDisplay

This function is meant to consume whatever information we had been storing within our Firestore database through the `MkSaveNote` function, this will render some html from the server which then can be viewed by browsers.

## Development

When developing with firestore, (and other GCP APIs that require auth) we can simplify by generating the default project credentials

```
gcloud auth application-default login
```

This will generate a set of credentials based on the default project your gcloud CLI is currently running

- Linux, macOS: `$HOME/.config/gcloud/application_default_credentials.json`
- Windows: `%APPDATA%\gcloud\application_default_credentials.json`

> **NOTE**: Please ensure that your default project is not a **_PRODUCTION_** one! otherwise you may end up developing with the wrong data :s

[More info about this here](https://cloud.google.com/docs/authentication/application-default-credentials)
