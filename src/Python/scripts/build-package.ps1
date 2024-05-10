<#
.SYNOPSIS
    Simply compiles the dotnet binaries and places them in the correct path

.PARAMETER Configuration
	Default to Release. Can be set to Debug

.PARAMETER Fast 
	Only build for current OS. 

.PARAMETER $IncludeTests
	Builds the tests and sdk. Without this just the sdk is built

.NOTES
    Author: Steffen Froehlich
#>
[CmdletBinding()]
param(
    [ValidateSet("Release", "Debug")]
    [string]$Configuration = "Release",
	[switch]$Fast,
	[switch]$IncludeTests
)

pushd (Join-Path $PSScriptRoot "..")

$sdkProjectPath = "..\Tableau.Migration\Tableau.Migration.csproj"
$testsProjectPath = "..\..\tests\Tableau.Migration.Tests\Tableau.Migration.Tests.csproj"

if($IncludeTests) {
	$projectToBuild = $testsProjectPath
}
else {
	$projectToBuild = $sdkProjectPath
}

try
{
	Remove-Item -Recurse -ErrorAction SilentlyContinue dist/* 
	
	dotnet publish /p:DebugType=None /p:DebugSymbols=false  $projectToBuild -c $Configuration -o .\src\tableau_migration\bin -f net6.0		
}
finally
{
	popd
}
