# Copyright (c) 2025, Salesforce, Inc.
# SPDX-License-Identifier: Apache-2
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

"""Testing module for the Python.TestApplication."""

import sys
import os
from os.path import abspath
from pathlib import Path

_module_path = abspath(Path(__file__).parent.resolve().__str__() + "/../../../src/Python/src")
sys.path.append(_module_path)

from tableau_migration import clr # noqa: E402
from  System.Reflection import Assembly # noqa: E402

print("Adding example application paths to sys.path")
sys.path.append(abspath(Path(__file__).parent.resolve().__str__() + "/../../../examples/Python.ExampleApplication/"))
sys.path.append(abspath(Path(__file__).parent.resolve().__str__() + "/../../../examples/Python.ExampleApplication/hooks/batch_migration_completed"))
sys.path.append(abspath(Path(__file__).parent.resolve().__str__() + "/../../../examples/Python.ExampleApplication/hooks/filters"))
sys.path.append(abspath(Path(__file__).parent.resolve().__str__() + "/../../../examples/Python.ExampleApplication/hooks/mappings"))
sys.path.append(abspath(Path(__file__).parent.resolve().__str__() + "/../../../examples/Python.ExampleApplication/hooks/migration_action_completed"))
sys.path.append(abspath(Path(__file__).parent.resolve().__str__() + "/../../../examples/Python.ExampleApplication/hooks/post_publish"))
sys.path.append(abspath(Path(__file__).parent.resolve().__str__() + "/../../../examples/Python.ExampleApplication/hooks/transformers"))

_bin_path = abspath(Path(__file__).parent.resolve().__str__() + "/../../../src/Python/src/tableau_migration/bin")
sys.path.append(_bin_path)
print("Added 'bin' at '" + _bin_path + "' to sys.path")

if os.environ.get('MIG_SDK_PYTHON_BUILD', 'false').lower() == 'true':
    print("MIG_SDK_PYTHON_BUILD set to true. Building dotnet binaries for python tests.")
    # Not required for GitHub Actions
    import subprocess
    import shutil

    shutil.rmtree(_bin_path, True)
    print("Building required binaries")
    _build_script = abspath(Path(__file__).parent.resolve().__str__() + "/../../../src/Python/scripts/build-package.ps1")
    subprocess.run(["pwsh", "-c", _build_script, "-Fast", "-IncludeTests"])
else:
    print("MIG_SDK_PYTHON_BUILD set to false. Skipping dotnet build for python tests.")

def add_assembly_reference(name,add_by_path):        
    if add_by_path:
        assemblyPath = _bin_path+ "/"+ name + ".dll"
        assembly = Assembly.LoadFile(assemblyPath)
        print(f"Added reference to {assembly.GetName().Name} v{assembly.GetName().Version} Full Name: {assembly.FullName}")
    else:
        clr.AddReference(name)
        assembly = Assembly.Load(name)
        print(f"Added reference to {assembly.GetName().Name} v{assembly.GetName().Version} Full Name: {assembly.FullName}")

add_assembly_reference("AutoFixture", True)
add_assembly_reference("AutoFixture.AutoMoq", True)
add_assembly_reference("Moq", True)
add_assembly_reference("Tableau.Migration", False)

_autofixture_helper_path = abspath(Path(__file__).parent.resolve().__str__() + "/helpers")    
sys.path.append(_autofixture_helper_path)
print("Added 'helpers' at '" + _autofixture_helper_path + "' to sys.path")




