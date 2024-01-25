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
	
	if(($Fast -and $IsWindows) -or (!$Fast)) {
		dotnet publish /p:DebugType=None /p:DebugSymbols=false  $projectToBuild -a win-x64 -c $Configuration -o .\src\tableau_migration\bin\win-x64 -f net7.0	
	}
	if(($Fast -and $IsMacOS) -or (!$Fast)){
		dotnet publish /p:DebugType=None /p:DebugSymbols=false $projectToBuild -a osx-x64 -c $Configuration -o .\src\tableau_migration\bin\osx-x64 -f net7.0
	}
	if(($Fast -and $IsLinux) -or (!$Fast)){
		dotnet publish /p:DebugType=None /p:DebugSymbols=false $projectToBuild -a linux-x64 -c $Configuration -o .\src\tableau_migration\bin\linux-x64 -f net7.0
	}
	
}
finally
{
	popd
}
