[CmdletBinding()]
param(
    [string]$RepoRoot = (Get-Location).Path,
    [switch]$UseExistingManifest
)

$ErrorActionPreference = "Stop"

$testAppDirectory = Join-Path $RepoRoot "tests/Tableau.Migration.TestApplication"

Set-Location $testAppDirectory

Write-Host "Running Tableau.Migration.TestApplication..."

$args = @("credential-migration-enabled")
if ($UseExistingManifest) {
    $args += "use-existing-manifest"
}

dotnet run -- @args

Write-Host ""
Write-Host "Output files are written to your locally configured paths."
Write-Host "  Logs: check your TestApplication log.folderPath setting"
Write-Host "    - Tableau.Migration.TestApplication-general-YYYY-MM-DD-HH-mm-ss.log"
Write-Host "    - Tableau.Migration.TestApplication-http-YYYY-MM-DD-HH-mm-ss.log"
Write-Host "  Manifests: check your TestApplication log.manifestFolderPath setting"
Write-Host "    - Manifest-YYYY-MM-DD-HH-mm-ss.json"
