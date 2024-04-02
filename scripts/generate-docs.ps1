<# 
Copyright (c) 2023, Salesforce, Inc.
SPDX-License-Identifier: Apache-2

Licensed under the Apache License, Version 2.0 (the ""License"") 
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an ""AS IS"" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
#>

<#
.SYNOPSIS
	Generate Python and C# documentation.
.PARAMETER Serve
    Hosts a local documentation site.
.PARAMETER $SkipPreClean
    Skips cleaning the working directories. Useful when this script is being run on CI/CD such as Github since the directories 
	do not exist yet.
#>
param (
	[switch]$Serve,
	[switch]$SkipPreClean
);

# Solution folders

# Directory location of this script
$root_dir = Split-Path $PSScriptRoot -Parent

# Directory for our Python project
$python_dir = Join-Path $root_dir "src/Python"

# Directory for our main documentation framework. This is where DocFX resides.
$main_docs_dir = Join-Path $root_dir "src/Documentation"

# Directory that sphinx generates automatic documentation to.
$sphinx_output_dir = Join-Path $python_dir "Documentation/generated"

function Run-Command {
	param([string]$Cmd)
	Write-Host "Executing command";	
	Write-Host "$Cmd";

	try {
		$startTime = Get-Date;
		Invoke-Expression  $Cmd -OutVariable output | Format-Console;
		$endTime = Get-Date;
		$executionTime = $endTime - $startTime;
		
		Write-Host "Done. $($executionTime.Hours*60 + $executionTime.Minutes)m$($executionTime.Seconds)s$($executionTime.Milliseconds)ms." -ForegroundColor Green;
		
		return $output;
	}
	catch {
		Write-Host-With-Timestamp "Function: $((Get-Variable MyInvocation -Scope 1).Value.MyCommand.Name). Command: $Cmd. Error: $_" "ERROR";
	}		
}

function Restore-Tools {
	<#
.SYNOPSIS
    Restore required .NET tools.
#>
	Write-Host-With-Timestamp "Restoring dotnet tools with versions from global.config.";
	& dotnet tool restore -v q 
}

function Clear-Directory {
	param([string] $Path)
	<#
.SYNOPSIS
    Clear all files in a directory recursively.
.PARAMETER Path
    Path to the directory.	
#>
	if ($SkipPreClean) {
		Write-Host-With-Timestamp "Skipping directory clean since SkipPreClean flag is on: " + $path;
	}
	elseif (-Not (Test-Path $path)) {
		Write-Host-With-Timestamp "Skipping directory clean since the folder does not exist: " + $path;
	}
	else {
		Get-ChildItem -Path ($path + "/") * -Exclude ".gitignore" -Directory -Recurse | ForEach-Object { Remove-Item $_ -Recurse };
		Get-ChildItem -Path ($path + "/") * -Exclude ".gitignore" -File -Recurse | ForEach-Object { Remove-Item $_ };

		if ($LASTEXITCODE -ne 0) {
			Throw "Failed: Cleaning directory $path";
		}
	}
}

function Write-Sdk-Version {
	<#
.SYNOPSIS
    Generate a Migration SDK version metadata file for docfx to use.
#>
	$versionSourceFileName = "Directory.Build.props";
	$docfxMetadataFileName = "migration_sdk_metadata.json";

	Write-Host-With-Timestamp("Writing Tableau Migration SDK from $versionSourceFileName for docfx.");
		
	$buildPropsXml = (Run-Command ("[Xml] (Get-Content (Join-Path $root_dir $versionSourceFileName))"));
	
	$sdkVersion = $buildPropsXml.Project.PropertyGroup.Version;

	Write-Host("Tableau Migration SDK version is $sdkVersion");

	$fileContent = "{""_comment"": ""This is an auto-generated file. Do not modify."", ""_migrationSdkVersion"": ""$sdkVersion""}";
	$filePath = Join-Path $main_docs_dir $docfxMetadataFileName;

	Run-Command ("Out-File -FilePath '$filePath' -InputObject '$($fileContent | Out-String)'");	
}

function Write-Python-docs {
	<#
.SYNOPSIS
    Generate documentation markdown from Python doc comments.
#>

	# Sphinx related paths
	$sphinx_build_dir = Join-Path $python_dir "Documentation"

	Write-Host-With-Timestamp "Generating python docs.";
	Run-Command ("Push-Location $python_dir");
	Run-Command ("python -m pip install -q --upgrade pip");
	Run-Command ("python -m pip install -q hatch");
	Run-Command ("Clear-Directory -Path $sphinx_output_dir");	
	Run-Command ("python -m hatch run docs:sphinx-build -M markdown $sphinx_build_dir $sphinx_output_dir -q");
	Run-Command ("Pop-Location");
	Write-Host-With-Timestamp "Finished: Generating python docs.";
	
	if ($LASTEXITCODE -ne 0) {
		Throw "Failed: Generating python docs.";
	}
}

function Copy-Python-Docs {
	<#
.SYNOPSIS
    Merge markdown files from manually generated documentation.
	Sphinx fails to generate documentation for some of our Python files.
#>
	# Directory to which sphinx write generated markdown files.
	$sphinx_generated_files_dir = Join-Path $sphinx_output_dir "markdown/generated/*";

	# Directory where DocFX looks for markdown files to render into our 'Python Wrapper' section.
	$python_md_destination = Join-Path $main_docs_dir "python_wrapper";

	Write-Host-With-Timestamp "Copying python docs to final destination.";
	Run-Command ("Clear-Directory -Path $python_md_destination");
	Run-Command ("Copy-Item -Force -Recurse $sphinx_generated_files_dir -Destination $python_md_destination");
	if ($LASTEXITCODE -ne 0) {
		Throw "Failed: Copying python docs to final destination.";
	}
}

function Write-Python-Docs-Toc {
	<#
.SYNOPSIS
    Generate a toc.yml file (table of contents for DocFX) from the python doc markdown files.
#>
	$python_md_destination = Join-Path $main_docs_dir "python_wrapper";
	Write-Host-With-Timestamp "Generating toc.yml for Python auto-generated doc files ($python_md_destination).";

	class DocFileInfo {
		[string]$Package;
		[string]$Module;
		[string]$Member;
		[string]$Category;
		[string]$FileName;
	}
	
	$docFiles = New-Object Collections.Generic.List[DocFileInfo];
	$files = Get-ChildItem $python_md_destination -include *.md -Recurse -File | Select-Object BaseName, NameString | Sort-Object -Property BaseName.Length;
	
	foreach ($file in $files) {
		
		#file names are in the package.module.Member format.
		# Hence we split them into their individual components are put them into
		# a DocFileInfo object
		$splitName = $file.BaseName.Split(".");
	
		$newDocFile = New-Object -TypeName DocFileInfo;
		$newDocFile.FileName = $file.NameString;
	
		if ($splitName.Length -gt 2) {        
			$newDocFile.Member = $splitName[2];
			$newDocFile.Category = "Member";
		}
		if ($splitName.Length -gt 1) {        
			$newDocFile.Module = $splitName[1];  
			if ($null -eq $newDocFile.Category) {
				$newDocFile.Category = "Module";
			}     
		}
		if ($splitName.Length -gt 0) {        
			$newDocFile.Package = $splitName[0];
			if ($null -eq $newDocFile.Category) {
				$newDocFile.Category = "Package";
			}
		}
		
		$docFiles.Add($newDocFile);    
	}

	# Build the yaml file from DocFileInfo list
	$fileContent = New-Object Collections.Generic.List[string];
	$fileContent.Add("items:");
	$packages = $docFiles | Where-Object { $_.Category -eq "Package" }
	if ($packages.Length -eq 0) {
		return;
	}
	foreach ($package in $packages) {    
		$fileContent.Add("- name: $($package.Package)");
		$fileContent.Add("  href: $($package.FileName)");
		$moduleGroups = $docFiles | Where-Object { $_.Category -ne "Package" -and $_.Package -eq $package.Package } | Group-Object -Property Package, Module;
		if ($moduleGroups.Length -eq 0) {
			continue;
		}
		
		$fileContent.Add("  items:");			
		foreach ($moduleGroup in $moduleGroups) {
			$module = $moduleGroup.Group | Where-Object { $_.Category -eq "Module" };
			$fileContent.Add("    - name: $($module.Module)");
			$fileContent.Add("      href: $($module.FileName)");
			
			$members = $moduleGroup.Group | Where-Object { $_.Category -eq "Member" };
			if ($members.Length -eq 0) {
				continue;
			}
			
			$fileContent.Add("      items:");
			$functions = $members | Where-Object { $_.Member -match "[a-zA-Z]+(_[a-zA-Z]+)" };	
			$classes = $members | Where-Object { $_.Member -notmatch "[a-zA-Z]+(_[a-zA-Z]+)" };
			
			foreach ($member in $functions) {
				$fileContent.Add("      - name: $($member.Member)");
				$fileContent.Add("        href: $($member.FileName)");
			}
			foreach ($member in $classes) {
				$fileContent.Add("      - name: $($member.Member)");
				$fileContent.Add("        href: $($member.FileName)");
			}
			
		}		
	}
	
	$tocPath = Join-Path -Path $python_md_destination -ChildPath toc.yml;
	
	Run-Command ("Out-File -FilePath '$tocPath' -InputObject '$($fileContent | Out-String)'");
	
	if ($LASTEXITCODE -ne 0) {
		Throw "Failed: Generating toc.yml for Python auto-generated doc files."
	}
	else {
		Write-Host-With-Timestamp "Finished: Generating toc.yml for Python auto-generated doc files."
	}
}

function Write-Final-Docs {
	<#
.SYNOPSIS
    Generate the final docs output with C#, Python auto-generated and manual markdown files.
	If run with a Serve switch, local site is hosted locally.
#>

	# Docfx related paths
	$docfx_config_path = Join-Path $main_docs_dir "docfx.json";
	$docfx_cmd = "dotnet docfx $docfx_config_path -t statictoc,templates\tableau --logLevel warning";

	# Run the docfx command to generate the final output	
	if ($Serve) {
		Write-Host-With-Timestamp "Generating final documentation output and hosting it locally.";
		Run-Command ($docfx_cmd + " --serve");
		Write-Host-With-Timestamp "Finished: Documentation generated and hosted.";
	}
	else {
		$docs_output_dir = Join-Path (Split-Path $PSScriptRoot -Parent) "docs";

		Write-Host-With-Timestamp "Generating final documentation output.";
		Run-Command ("Clear-Directory -Path $docs_output_dir");
		Run-Command ($docfx_cmd);
		Write-Host-With-Timestamp "Finished: API Reference documentation has been generated to: $docs_output_dir";
	}
	if ($LASTEXITCODE -ne 0) {
		Throw "Failed: Generating final documentation output.";
	}
}

function Format-Console {
	[CmdletBinding()]
	param([Parameter(ValueFromPipeline = $True)][string[]]$inputObject)
	PROCESS { 
		Write-Host "     $inputObject";
	}
}

function Write-Host-With-Timestamp {
	param(
		[string]$message,
		[string]$level = "INFO")

	$timestamp = Get-Date -Format "MM/dd/yyyy HH:mm:ss"

	if ($level -eq "ERROR") {
		Write-Host "${timestamp}: $level :" -ForegroundColor Red;
		Write-Error -Message "${message}" -ErrorAction Stop;
	}
	else {
		Write-Host "${timestamp}: $level : ${message}";
	}
}

Restore-Tools;
Write-Python-docs;
Copy-Python-Docs;
Write-Python-Docs-Toc;
Write-Sdk-Version;
Write-Final-Docs;