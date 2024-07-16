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

"""Builds the required C# dlls for the Python test app."""

import sys
import os

# Build the solution so we have the published binaries
from os.path import abspath
from pathlib import Path

def build():    
    """Build binaries and put them on the path."""
    bin_path = abspath(Path(__file__).parent.resolve().__str__() + "/bin")

    if os.environ.get('MIG_SDK_PYTHON_BUILD', 'false') == 'true':
        # Not required for GitHub Actions
        import subprocess
        import shutil
    
        migration_project = abspath("../../tests/Tableau.Migration.Tests/Tableau.Migration.Tests.csproj")
        shutil.rmtree(bin_path, True)
        subprocess.run(["dotnet", "publish", migration_project, "-o", bin_path, "-f", "net6.0"])

    sys.path.append(bin_path)
