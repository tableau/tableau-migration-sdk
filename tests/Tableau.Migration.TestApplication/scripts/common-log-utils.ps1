function Read-JsonFile {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $raw = Get-Content -Path $Path -Raw
    try {
        return ($raw | ConvertFrom-Json)
    }
    catch {
        # Fallback for JSON-with-comment-lines used in local appsettings defaults.
        $json = [regex]::Replace($raw, '(?m)^\s*//.*$', '')
        return ($json | ConvertFrom-Json)
    }
}

function Get-LogPathValue {
    param(
        [Parameter(Mandatory = $false)]
        $SettingsObject,
        [Parameter(Mandatory = $true)]
        [string]$PropertyName
    )

    if ($null -eq $SettingsObject) { return $null }
    if ($null -eq $SettingsObject.log) { return $null }

    $value = $SettingsObject.log.$PropertyName
    if ([string]::IsNullOrWhiteSpace($value)) { return $null }

    return $value
}

function Get-RunTimestampFromFileName {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FileName
    )

    $match = [regex]::Match($FileName, '(?<ts>\d{4}-\d{2}-\d{2}-\d{2}-\d{2}-\d{2})')
    if (-not $match.Success) { return $null }

    return $match.Groups["ts"].Value
}

function Get-TestAppPaths {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RepoRoot
    )

    $testAppDirectory = Join-Path $RepoRoot "tests/Tableau.Migration.TestApplication"
    $baseSettingsPath = Join-Path $testAppDirectory "appsettings.json"
    $environmentName = $env:DOTNET_ENVIRONMENT
    if ([string]::IsNullOrWhiteSpace($environmentName)) { $environmentName = "Production" }
    $environmentSettingsPath = Join-Path $testAppDirectory ("appsettings.{0}.json" -f $environmentName.ToLower())

    return [pscustomobject]@{
        TestAppDirectory = $testAppDirectory
        BaseSettingsPath = $baseSettingsPath
        EnvironmentName = $environmentName
        EnvironmentSettingsPath = $environmentSettingsPath
    }
}

function Resolve-LogAndManifestFolders {
    param(
        [Parameter(Mandatory = $true)]
        [string]$BaseSettingsPath,
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentSettingsPath
    )

    $baseSettings = $null
    $environmentSettings = $null

    if (Test-Path $BaseSettingsPath) {
        $baseSettings = Read-JsonFile -Path $BaseSettingsPath
    }

    if (Test-Path $EnvironmentSettingsPath) {
        $environmentSettings = Read-JsonFile -Path $EnvironmentSettingsPath
    }

    $logFolder = Get-LogPathValue -SettingsObject $baseSettings -PropertyName "folderPath"
    $manifestFolder = Get-LogPathValue -SettingsObject $baseSettings -PropertyName "manifestFolderPath"

    if ($null -ne $environmentSettings) {
        $environmentLogFolder = Get-LogPathValue -SettingsObject $environmentSettings -PropertyName "folderPath"
        if (-not [string]::IsNullOrWhiteSpace($environmentLogFolder)) {
            $logFolder = $environmentLogFolder
        }
    }

    if ($null -ne $environmentSettings) {
        $environmentManifestFolder = Get-LogPathValue -SettingsObject $environmentSettings -PropertyName "manifestFolderPath"
        if (-not [string]::IsNullOrWhiteSpace($environmentManifestFolder)) {
            $manifestFolder = $environmentManifestFolder
        }
    }

    return [pscustomobject]@{
        LogFolder = $logFolder
        ManifestFolder = $manifestFolder
    }
}
