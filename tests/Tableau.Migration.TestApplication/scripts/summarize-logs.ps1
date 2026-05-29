[CmdletBinding()]
param(
    [string]$RepoRoot = (Get-Location).Path,
    [string]$Timestamp
)

$ErrorActionPreference = "Stop"

. (Join-Path $RepoRoot "tests/Tableau.Migration.TestApplication/scripts/common-log-utils.ps1")

$paths = Get-TestAppPaths -RepoRoot $RepoRoot
$baseSettingsPath = $paths.BaseSettingsPath
$environmentSettingsPath = $paths.EnvironmentSettingsPath

$resolvedFolders = Resolve-LogAndManifestFolders -BaseSettingsPath $baseSettingsPath -EnvironmentSettingsPath $environmentSettingsPath
$logFolder = $resolvedFolders.LogFolder

if ([string]::IsNullOrWhiteSpace($logFolder)) {
    throw ("log.folderPath is not configured in {0} or {1}" -f $baseSettingsPath, $environmentSettingsPath)
}

if (-not (Test-Path $logFolder)) {
    throw ("Logs folder does not exist: {0}" -f $logFolder)
}

$allGeneralLogs = @(Get-ChildItem -Path $logFolder -Filter "Tableau.Migration.TestApplication-general-*.log" -File)
$allHttpLogs = @(Get-ChildItem -Path $logFolder -Filter "Tableau.Migration.TestApplication-http-*.log" -File)
$allLogs = @($allGeneralLogs + $allHttpLogs)

if ($allLogs.Count -eq 0) {
    throw ("No test app logs found in {0}" -f $logFolder)
}

$selectedTimestamp = $Timestamp
if ([string]::IsNullOrWhiteSpace($selectedTimestamp)) {
    $selectedTimestamp = $null
    $tsCandidates = $allLogs |
        ForEach-Object { Get-RunTimestampFromFileName -FileName $_.Name } |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
        Sort-Object -Unique -Descending

    foreach ($ts in $tsCandidates) {
        $hasGeneral = @($allGeneralLogs | Where-Object { (Get-RunTimestampFromFileName -FileName $_.Name) -eq $ts }).Count -gt 0
        $hasHttp = @($allHttpLogs | Where-Object { (Get-RunTimestampFromFileName -FileName $_.Name) -eq $ts }).Count -gt 0
        if ($hasGeneral -and $hasHttp) {
            $selectedTimestamp = $ts
            break
        }
    }
}

if ([string]::IsNullOrWhiteSpace($selectedTimestamp)) {
    throw "Could not determine a shared timestamp that has both general and http logs."
}

$generalLogs = @($allGeneralLogs | Where-Object { (Get-RunTimestampFromFileName -FileName $_.Name) -eq $selectedTimestamp } | Sort-Object LastWriteTime -Descending)
$httpLogs = @($allHttpLogs | Where-Object { (Get-RunTimestampFromFileName -FileName $_.Name) -eq $selectedTimestamp } | Sort-Object LastWriteTime -Descending)
$targetFiles = @($generalLogs + $httpLogs) | Sort-Object LastWriteTime

if ($targetFiles.Count -eq 0) {
    throw ("No logs found for timestamp {0} in {1}" -f $selectedTimestamp, $logFolder)
}

$levelCounts = @{
    Critical = 0
    Error = 0
    Warning = 0
    Information = 0
    Debug = 0
    Trace = 0
    Verbose = 0
}

$logLinePattern = '^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} [+-]\d{2}:\d{2}\|(?<level>[^|]+)\|'

foreach ($file in $targetFiles) {
    $lines = Get-Content -Path $file.FullName
    foreach ($line in $lines) {
        $match = [regex]::Match($line, $logLinePattern)
        if ($match.Success) {
            $level = $match.Groups["level"].Value.Trim()
            if ($levelCounts.ContainsKey($level)) {
                $levelCounts[$level] = [int]$levelCounts[$level] + 1
            }
        }
    }
}

Write-Host "Log summary"
Write-Host ("  Timestamp        : {0}" -f $selectedTimestamp)
Write-Host ("  Logs analyzed    : {0} (general: {1}, http: {2})" -f $targetFiles.Count, $generalLogs.Count, $httpLogs.Count)
Write-Host ("  Total entries    : {0}" -f ($levelCounts.Critical + $levelCounts.Error + $levelCounts.Warning + $levelCounts.Information + $levelCounts.Debug + $levelCounts.Trace + $levelCounts.Verbose))
Write-Host ""
Write-Host "Level counts"
Write-Host ("  Information      : {0}" -f $levelCounts.Information)
Write-Host ("  Debug            : {0}" -f $levelCounts.Debug)
Write-Host ("  Trace            : {0}" -f $levelCounts.Trace)
Write-Host ("  Verbose          : {0}" -f $levelCounts.Verbose)
Write-Host ("  Warning          : {0}" -f $levelCounts.Warning)
Write-Host ("  Error            : {0}" -f $levelCounts.Error)
Write-Host ("  Critical         : {0}" -f $levelCounts.Critical)
