<#
.SYNOPSIS
    Builds and publishes the migration-sdk python package

.NOTES
    Author: Steffen Froehlich

.PARAMETER SkipPublish
	Builds everything but skips pushing to repository

.PARAMETER VersionOverride
	String used to override the version in the Directory.Build.props file in the root directory.
	The Python Hatchling project is required for this: https://pypi.org/project/hatchling/
#>

[CmdletBinding()]
param(
    [switch]$SkipPublish,
	[string]$VersionOverride
)

function Test-CommandExists ($command)
{
	$oldPreference = $ErrorActionPreference

	$ErrorActionPreference = 'stop'

	try {
		if(Get-Command $command) {
			return $true
		}
	}
	catch {
		return $false
	}
	finally {
		$ErrorActionPreference=$oldPreference
	}
}

pushd (Join-Path $PSScriptRoot "..")

$env:TWINE_NON_INTERACTIVE="1"

if (-not($SkipPublish)) {
	if( [string]::IsNullOrEmpty($env:TWINE_PASSWORD) ) {
		Write-Error "Set `$env:TWINE_PASSWORD to the svc_cmd password found in cyberark"
		exit 1
	}
}
if ($VersionOverride) {
	if( -not(Test-CommandExists hatch)) {
		Write-Error "install hatch to set custom version. 'pip install hatch'"
		exit 1
	}
}

try {
	& (Join-Path $PSScriptRoot "build-package.ps1")

	if ($VersionOverride) {
		$oldVersion = (hatch version)
		hatch version $VersionOverride
	}
	python -m build --wheel # Build the wheel package
	python -m build --sdist # Build the source dist package
	
	if (-not($SkipPublish)) {
		# The package will be uploaded to the package repository
		Write-Host "Uploading package to $($env:TWINE_REPOSITORY_URL)"
		python -m twine upload --repository-url $env:TWINE_REPOSITORY_URL dist/* 
	}
	
}
finally {
	popd

	if ($oldVersion){
		hatch version $oldVersion
	}
}
