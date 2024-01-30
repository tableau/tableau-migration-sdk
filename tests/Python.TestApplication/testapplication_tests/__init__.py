# Copyright (c) 2023, Salesforce, Inc.
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
import build # noqa: F401
from os.path import abspath
from pathlib import Path

_main_module_path = abspath(Path(__file__).parent.resolve().__str__() + "/../../../src/Python/src")
sys.path.append(_main_module_path)

_test_app_path = abspath(Path(__file__).parent.resolve().__str__() + "/..")
sys.path.append(_test_app_path)

build.build()

import tableau_migration            # noqa: E402,F401
from tableau_migration import clr   # noqa: E402

clr.AddReference("Moq")
clr.AddReference("Tableau.Migration.Tests")
clr.AddReference("Tableau.Migration.TestComponents")
