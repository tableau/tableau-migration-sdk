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

# Make sure the test can find the module
import sys
from os.path import abspath
from pathlib import Path
import uuid
import pytest
from tableau_migration.migration import (
   get_service_provider,
   get_service 
)

from tableau_migration.migration_engine_options import (
    PyMigrationPlanOptionsBuilder,
    PyMigrationPlanOptionsCollection
)

import System

from Tableau.Migration.Tests import TestPlanOptions as PyTestPlanOptions # Needed as this class name starts with Test, which means pytest wants to pick it up

from Tableau.Migration.Engine.Hooks import IMigrationHook

from Tableau.Migration.Engine.Options import (
    IMigrationPlanOptionsBuilder,
    IMigrationPlanOptionsCollection
)

import Moq
_dist_path = abspath(Path(__file__).parent.resolve().__str__() + "/../src/tableau_migration")
sys.path.append(_dist_path)


class TestMigrationPlanOptions():
    
    def test_init_collections(self):
        migration_plan_options_colllection_mock = Moq.Mock[IMigrationPlanOptionsCollection]()
        PyMigrationPlanOptionsCollection(migration_plan_options_colllection_mock.Object)
    
    def test_get(self):
        """Verify that the MigrationPlanOptionsBuilder can return the options that were configured"""
        input_option = PyTestPlanOptions()

        services = get_service_provider()
        dotnet_plan_options_builder = get_service(services, IMigrationPlanOptionsBuilder)
        
        builder = PyMigrationPlanOptionsBuilder(dotnet_plan_options_builder) 
        builder.configure(input_option)

        options = builder.build().get(PyTestPlanOptions)

        assert input_option == options
