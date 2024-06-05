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

import pytest
import uuid

from tableau_migration.migration_content import PyUser
from tableau_migration.migration_engine_migrators_batch import PyContentBatchMigrationResult
from tests.helpers.autofixture import AutoFixtureTestBase

from Tableau.Migration.Content import IUser
from Tableau.Migration.Engine.Migrators.Batch import IContentBatchMigrationResult

class TestPyContentBatchMigrationResult(AutoFixtureTestBase):
    
    def test_item_results(self):
        dotnet = self.create(IContentBatchMigrationResult[IUser])
        
        py = PyContentBatchMigrationResult[PyUser](dotnet)
        
        assert len(dotnet.ItemResults) != 0
        assert len(py.item_results) == len(dotnet.ItemResults)
        assert py.item_results[0].manifest_entry.source.id == uuid.UUID(dotnet.ItemResults[0].ManifestEntry.Source.Id.ToString())