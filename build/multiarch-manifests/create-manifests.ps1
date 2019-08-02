Param(
    [parameter(Mandatory=$true)][string]$registry
)

if ([String]::IsNullOrEmpty($registry)) {
    Write-Host "Registry must be set to docker registry to use" -ForegroundColor Red
    exit 1 
}

Write-Host "This script creates the local manifests, for pushing the multi-arch manifests" -ForegroundColor Yellow
Write-Host "Tags used are linux-master, win-master, arm32-master, linux-dev, win-dev, arm32-dev, linux-latest, win-latest, arm32-latest" -ForegroundColor Yellow
Write-Host "Multiarch images tags will be master, dev, latest" -ForegroundColor Yellow


$services = "bot.api"

foreach ($svc in $services) {
    Write-Host "Creating manifest for $svc and tags :latest, :master, and :dev"
    docker manifest create $registry/${svc}:master $registry/${svc}:linux-master $registry/${svc}:win-master $registry/${svc}:arm32-master
    docker manifest create $registry/${svc}:dev $registry/${svc}:linux-dev $registry/${svc}:win-dev $registry/${svc}:arm32-dev
    docker manifest create $registry/${svc}:latest $registry/${svc}:linux-latest $registry/${svc}:win-latest $registry/${svc}:arm32-latest
    Write-Host "Pushing manifest for $svc and tags :latest, :master, and :dev"
    docker manifest push $registry/${svc}:latest
    docker manifest push $registry/${svc}:dev
    docker manifest push $registry/${svc}:master
}