# Copyright (c) 2024, Salesforce, Inc.
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

import sys
import os
from os.path import abspath
from pathlib import Path

_module_path = abspath(Path(__file__).parent.resolve().__str__() + "/../src/tableau_migration")
sys.path.append(_module_path)

if os.environ.get('MIG_SDK_PYTHON_BUILD', 'false').lower() == 'true':
    print("MIG_SDK_PYTHON_BUILD set to true. Building dotnet binaries for python tests.")
    # Not required for GitHub Actions
    import subprocess
    import shutil
    
    _bin_path = abspath(Path(__file__).parent.resolve().__str__() + "/../src/tableau_migration/bin")
    shutil.rmtree(_bin_path, True)
    
    _build_script = abspath(Path(__file__).parent.resolve().__str__() + "/../scripts/build-package.ps1")
    subprocess.run(["pwsh", "-c", _build_script, "-Fast", "-IncludeTests"])
else:
    print("MIG_SDK_PYTHON_BUILD set to false. Skipping dotnet build for python tests.")

from tableau_migration import clr
clr.AddReference("AutoFixture")
clr.AddReference("AutoFixture.AutoMoq")
clr.AddReference("Moq")
clr.AddReference("Tableau.Migration.Tests")
