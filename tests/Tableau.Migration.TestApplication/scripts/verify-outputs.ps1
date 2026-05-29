[CmdletBinding()]
param(
    [string]$RepoRoot = (Get-Location).Path
)

$ErrorActionPreference = "Stop"

. (Join-Path $RepoRoot "tests/Tableau.Migration.TestApplication/scripts/common-log-utils.ps1")

$paths = Get-TestAppPaths -RepoRoot $RepoRoot
$baseSettingsPath = $paths.BaseSettingsPath
$environmentSettingsPath = $paths.EnvironmentSettingsPath
$environmentName = $paths.EnvironmentName

$resolvedFolders = Resolve-LogAndManifestFolders -BaseSettingsPath $baseSettingsPath -EnvironmentSettingsPath $environmentSettingsPath
$logFolder = $resolvedFolders.LogFolder
$manifestFolder = $resolvedFolders.ManifestFolder

if ([string]::IsNullOrWhiteSpace($logFolder)) {
    throw ("log.folderPath is not configured in {0} or {1}" -f $baseSettingsPath, $environmentSettingsPath)
}

if ([string]::IsNullOrWhiteSpace($manifestFolder)) {
    throw ("log.manifestFolderPath is not configured in {0} or {1}" -f $baseSettingsPath, $environmentSettingsPath)
}

Write-Host "Verifying outputs using appsettings.json + environment override"
Write-Host "  DOTNET_ENVIRONMENT: $environmentName"
Write-Host "  Base settings      : $baseSettingsPath"
Write-Host "  Env settings       : $environmentSettingsPath"
Write-Host "  Logs folder     : $logFolder"
Write-Host "  Manifests folder: $manifestFolder"

$generalLogs = @()
$httpLogs = @()
$manifests = @()

if (Test-Path $logFolder) {
    $generalLogs = Get-ChildItem -Path $logFolder -Filter "Tableau.Migration.TestApplication-general-*.log" -File
    $httpLogs = Get-ChildItem -Path $logFolder -Filter "Tableau.Migration.TestApplication-http-*.log" -File
}

if (Test-Path $manifestFolder) {
    $manifests = Get-ChildItem -Path $manifestFolder -File |
        Where-Object { $_.Name -like ".Manifest-*.json" -or $_.Name -like "Manifest-*.json" } |
        Sort-Object LastWriteTime -Descending
}

$timestampCandidates = @()
foreach ($manifest in $manifests) {
    $ts = Get-RunTimestampFromFileName -FileName $manifest.Name
    if (-not [string]::IsNullOrWhiteSpace($ts)) {
        $timestampCandidates += [pscustomobject]@{
            Timestamp = $ts
            LastWriteTime = $manifest.LastWriteTime
        }
    }
}
$timestampCandidates = $timestampCandidates | Sort-Object LastWriteTime -Descending

$selectedTimestamp = $null
$matchedGeneralLogs = @()
$matchedHttpLogs = @()
$matchedManifests = @()

foreach ($candidate in $timestampCandidates) {
    $ts = $candidate.Timestamp

    $candidateGeneralLogs = @($generalLogs | Where-Object { (Get-RunTimestampFromFileName -FileName $_.Name) -eq $ts } | Sort-Object LastWriteTime -Descending)
    $candidateHttpLogs = @($httpLogs | Where-Object { (Get-RunTimestampFromFileName -FileName $_.Name) -eq $ts } | Sort-Object LastWriteTime -Descending)
    $candidateManifests = @($manifests | Where-Object { (Get-RunTimestampFromFileName -FileName $_.Name) -eq $ts } | Sort-Object LastWriteTime -Descending)

    if ($candidateGeneralLogs.Count -gt 0 -and $candidateHttpLogs.Count -gt 0 -and $candidateManifests.Count -gt 0) {
        $selectedTimestamp = $ts
        $matchedGeneralLogs = $candidateGeneralLogs
        $matchedHttpLogs = $candidateHttpLogs
        $matchedManifests = $candidateManifests
        break
    }
}

Write-Host ""
Write-Host "Verification:"
Write-Host ("  Total logs   : {0} (general: {1}, http: {2})" -f ($generalLogs.Count + $httpLogs.Count), $generalLogs.Count, $httpLogs.Count)
if ([string]::IsNullOrWhiteSpace($selectedTimestamp)) {
    Write-Host "  Timestamp    : NOT FOUND (no shared timestamp across general/http/manifest)"
    Write-Host ("  General logs : {0}" -f ($(if ($generalLogs.Count -gt 0) { "FOUND (latest: $($generalLogs | Sort-Object LastWriteTime -Descending | Select-Object -First 1 -ExpandProperty Name))" } else { "NOT FOUND" })))
    Write-Host ("  HTTP logs    : {0}" -f ($(if ($httpLogs.Count -gt 0) { "FOUND (latest: $($httpLogs | Sort-Object LastWriteTime -Descending | Select-Object -First 1 -ExpandProperty Name))" } else { "NOT FOUND" })))
    Write-Host ("  Manifests    : {0}" -f ($(if ($manifests.Count -gt 0) { "FOUND (latest: $($manifests[0].Name))" } else { "NOT FOUND" })))
    exit 1
}

Write-Host ("  Timestamp    : {0}" -f $selectedTimestamp)
Write-Host ("  Matched logs : {0} (general: {1}, http: {2})" -f ($matchedGeneralLogs.Count + $matchedHttpLogs.Count), $matchedGeneralLogs.Count, $matchedHttpLogs.Count)
Write-Host ("  General logs : FOUND ({0} file(s); latest: {1})" -f $matchedGeneralLogs.Count, $matchedGeneralLogs[0].Name)
Write-Host ("  HTTP logs    : FOUND ({0} file(s); latest: {1})" -f $matchedHttpLogs.Count, $matchedHttpLogs[0].Name)
Write-Host ("  Manifests    : FOUND ({0} file(s); latest: {1})" -f $matchedManifests.Count, $matchedManifests[0].Name)

exit 0
