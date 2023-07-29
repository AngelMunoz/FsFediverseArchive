param([switch] $all = $false, [string] $function)


$region = "us-west1"
$runtime = "dotnet6"


function New-HttpFn-Deploy($name, $namespace) {
    $entryPoint = "$namespace.Function"
    $buildEnvVars = "GOOGLE_BUILDABLE=$namespace"
    
    gcloud functions deploy "$name" --gen2 --region=$region --entry-point=$entryPoint --runtime=$runtime --trigger-http --set-build-env-vars=$buildEnvVars
    return $LASTEXITCODE
}

function New-TopicFn-Deploy($name, $namespace, $topic) {
    $entryPoint = "$namespace.Function"
    $buildEnvVars = "GOOGLE_BUILDABLE=$namespace"

    gcloud functions deploy "$name" --gen2 --region=$region --entry-point=$entryPoint --runtime=$runtime --trigger-topic=$topic --set-build-env-vars=$buildEnvVars
    return $LASTEXITCODE
}


if ($all) {
    $results =
    @(
        New-HttpFn-Deploy -name misskey-webhook -namespace MkWebHook -isPublic $true
        New-HttpFn-Deploy -name misskey-display -namespace MkDisplay -isPublic $true
        New-HttpFn-Deploy -name misskey-jsonify -namespace MkJsonify -isPublic $true
        New-TopicFn-Deploy -name misskey-save-note -namespace MkSaveNote -Topic mk-publish-note
    );

    $hasError = $false
    foreach ($result in $results) {
        if ($results -gt 0) {
            $hasError = $true
            Write-Host "A Deployment failed, see logs for more details."
        }
    }

    if ($hasError) {
        exit 1
    }
    exit 0
}
else {
    Switch ($function) {
        "webhook" {
            New-HttpFn-Deploy -name misskey-webhook -namespace MkWebHook -isPublic $true
        }
        "display" {
            New-HttpFn-Deploy -name misskey-display -namespace MkDisplay -isPublic $true
        }
        "jsonify" {
            New-HttpFn-Deploy -name misskey-jsonify -namespace MkJsonify -isPublic $true
        }
        "save-note" {
            New-TopicFn-Deploy -name misskey-save-note -namespace MkSaveNote -Topic mk-publish-note
        }
        default {
            Write-Host "Unknown function name: $function"
            exit 1
        }
    }
    exit 0
}
