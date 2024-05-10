<# 
Copyright (c) 2024, Salesforce, Inc.
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
	Generate license headers for python files.
.PARAMETER Targets
    List of directories to update .py files
#>
param([string[]]$Target)

$license_header_python = "# Copyright (c) 2024, Salesforce, Inc.
# SPDX-License-Identifier: Apache-2
#
# Licensed under the Apache License, Version 2.0 (the ""License"");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an ""AS IS"" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
"

function Write-Header ($file)
{
    $content = Get-Content $file
    $filename = Split-Path -Leaf $file
    $fileheader = $license_header_python
    Set-Content $file $fileheader
    Add-Content $file $content
}

Get-ChildItem $Target -Recurse | ? { $_.Extension -like ".py" } | % `
{
    Write-Header $_.PSPath.Split(":", 3)[2]
}