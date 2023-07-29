# Deploy

## MkWebHook

```
gcloud functions deploy misskey-webhook \
--gen2 \
--region us-west1 \
--entry-point MkWebHook.Function \
--runtime dotnet6 \
--trigger-http \
--allow-unauthenticated \
--set-build-env-vars=GOOGLE_BUILDABLE=MkWebHook
```

## MkDisplay

```
gcloud functions deploy misskey-display \
--gen2 \
--region us-west1 \
--entry-point MkDisplay.Function \
--runtime dotnet6 \
--trigger-http \
--allow-unauthenticated \
--set-build-env-vars=GOOGLE_BUILDABLE=MkDisplay
```

## MkJsonify

```
gcloud functions deploy misskey-jsonify \
--gen2 \
--region us-west1 \
--entry-point MkJsonify.Function \
--runtime dotnet6 \
--trigger-http \
--allow-unauthenticated \
--set-build-env-vars=GOOGLE_BUILDABLE=MkJsonify
```

## MkSaveNote

```
gcloud functions deploy misskey-save-note \
--gen2 \
--region us-west1 \
--entry-point MkSaveNote.Function \
--runtime dotnet6 \
--trigger-topic mk-publish-note \
--set-build-env-vars=GOOGLE_BUILDABLE=MkSaveNote
```
