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

"""Helper for Python.TestApplication."""

import sys
import json
import build # Builds the C# project. This is here so we can make changes to the C# and python code without packaging

from os.path import abspath, isfile
from pathlib import Path
from datetime import datetime


build.build()

_main_module_path = abspath(Path(__file__).parent.resolve().__str__() + "/../../src/Python/src")
sys.path.append(_main_module_path)

import tableau_migration # noqa: E402, F401 
import clr               # noqa: E402

if isfile('config.DEV.json'):
    with open('config.DEV.json') as config_file:
        config = json.load(config_file)
else:
    with open('config.json') as config_file:
        config = json.load(config_file)

now_str = datetime.now().strftime("%Y-%m-%d-%H-%M-%S")
manifest_path = config['log']['manifest_folder_path'] + "PythonManifest-" + now_str + ".json"

