gcloud functions deploy misskey-webhook \
--region us-west1 \
--entry-point MkWebHook.Function \
--runtime dotnet6 \
--trigger-http \
--allow-unauthenticated \
--security-level secure-always

X-Misskey-Hook-Id
X-Misskey-Hook-Secret
X-Misskey-Host
