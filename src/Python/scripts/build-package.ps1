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

$productOutputPath = ".\src\tableau_migration\bin"
$testOutputPath = "..\..\dist\tests"

try
{
	Remove-Item -Recurse -ErrorAction SilentlyContinue dist/* 
	
	dotnet publish /p:DebugType=None /p:DebugSymbols=false $sdkProjectPath -c $Configuration -o $productOutputPath -f net8.0

	if($IncludeTests) {
		dotnet publish /p:DebugType=None /p:DebugSymbols=false $testsProjectPath -c $Configuration -o $testOutputPath -f net8.0
	}		
}
finally
{
	popd
}
