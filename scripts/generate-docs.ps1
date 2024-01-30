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
	param([string] $cmd)
	Write-Host(" ")
	Write-Host($cmd)
	Invoke-Expression $cmd | Format-Console
	if ($LASTEXITCODE -ne 0) {
		Throw "Error: $cmd failed"
	}
	Write-Host(" ")
}

function Restore-Tools {
	<#
.SYNOPSIS
    Restore required .NET tools.
#>
	Write-Host("Restoring dotnet tools with versions from global.config.") 
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
		Write-Host("Skipping directory clean since SkipPreClean flag is on: " + $path)
	}
	elseif (-Not (Test-Path $path)) {
		Write-Host("Skipping directory clean since the folder does not exist: " + $path)
	}
	else {
		Write-Host("Cleaning directory: " + $path)

		Get-ChildItem -Path ($path + "/") * -Exclude ".gitignore" -File -Recurse | ForEach-Object { $_.Delete() }

		if ($LASTEXITCODE -ne 0) {
			Throw "Failed: Cleaning directory" + $path
		}
		else {
			Write-Host("Finished: Cleaning directory: " + $path)
		}
	}
}

function Write-Python-docs {
	<#
.SYNOPSIS
    Generate documentation markdown from Python doc comments.
#>

	# Sphinx related paths
	$sphinx_build_dir = Join-Path $python_dir "Documentation"

	Write-Host("Generating python docs.")
	& Run-Command ("Push-Location $python_dir")
	& Run-Command ("python -m pip install -q --upgrade pip")
	& Run-Command ("python -m pip install -q hatch")
	& Run-Command ("Clear-Directory -Path $sphinx_output_dir")	
	& Run-Command ("python -m hatch run docs:sphinx-build -M markdown $sphinx_build_dir $sphinx_output_dir")
	& Run-Command ("Pop-Location")
	& Write-Host("Finished: Generating python docs.")
	
	if ($LASTEXITCODE -ne 0) {
		Throw "Failed: Generating python docs."
	}
}

function Copy-Python-Docs {
	<#
.SYNOPSIS
    Merge markdown files from manually generated documentation.
	Sphinx fails to generate documentation for some of our Python files.
#>
	# Directory to which sphinx write generated markdown files.
	$sphinx_generated_files_dir = Join-Path $sphinx_output_dir "markdown/generated/*"

	# Directory where DocFX looks for markdown files to render into our 'Python Wrapper' section.
	$python_md_destination = Join-Path $main_docs_dir "python_wrapper"

	Write-Host("Copying python docs to final destination.")
	& Run-Command ("Clear-Directory -Path $python_md_destination")
	& Run-Command ("Copy-Item -Force -Recurse $sphinx_generated_files_dir -Destination $python_md_destination")
	if ($LASTEXITCODE -ne 0) {
		Throw "Failed: Copying python docs to final destination."
	}
}

function Write-Python-Docs-Toc {
	<#
.SYNOPSIS
    Generate a toc.yml file (table of contents for DocFX) from the python doc markdown files.
#>
	$python_md_destination = Join-Path $main_docs_dir "python_wrapper"
	Write-Host("Generating toc.yml for Python auto-generated doc files ($python_md_destination).")		
	& Run-Command ("dotnet DocFxTocGenerator --docfolder $python_md_destination")	
	
	if ($LASTEXITCODE -ne 0) {
		Throw "Failed: Generating toc.yml for Python auto-generated doc files."
	}
	else {
		Write-Host("Finished: Generating toc.yml for Python auto-generated doc files.")
	}
}


function Write-Final-Docs {
	<#
.SYNOPSIS
    Generate the final docs output with C#, Python auto-generated and manual markdown files.
	If run with a Serve switch, local site is hosted locally.
#>

	# Docfx related paths
	$docfx_config_path = Join-Path $main_docs_dir "docfx.json"
	$docfx_cmd = "dotnet docfx $docfx_config_path -t statictoc,templates\tableau --logLevel warning"

	# Run the docfx command to generate the final output	
	if ($Serve) {
		Write-Host("Generating final documentation output and hosting it locally.") 
		& Run-Command ($docfx_cmd + " --serve") 
		& Write-Host-With-Timestamp("Finished: Documentation generated and hosted.") 
	}
	else {
		$docs_output_dir = Join-Path (Split-Path $PSScriptRoot -Parent) "docs"

		Write-Host("Generating final documentation output.")	
		& Run-Command ("Clear-Directory -Path $docs_output_dir")	
		& Run-Command ($docfx_cmd)
		& Write-Host-With-Timestamp("Finished: API Reference documentation has been generated to: " + $docs_output_dir) 
	}
	if ($LASTEXITCODE -ne 0) {
		Throw "Failed: Generating final documentation output."
	}
}

function Format-Console {
	[CmdletBinding()]
	param([Parameter(ValueFromPipeline = $True)][string[]]$inputObject)
	PROCESS { 
		Write-Host "     $inputObject"
	}
}

function Write-Host-With-Timestamp {
	param([string]$message)

	$timestamp = Get-Date -Format "MM/dd/yyyy HH:mm:ss"
	Write-Host "${timestamp}: ${message}"
}

Restore-Tools && Write-Python-docs && Copy-Python-Docs && Write-Python-Docs-Toc && Write-Final-Docs