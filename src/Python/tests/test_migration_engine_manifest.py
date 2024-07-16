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

import os
import tempfile
from tests.helpers.autofixture import AutoFixtureTestBase # noqa: E402, F401
from Tableau.Migration import IMigrationManifest # noqa: E402, F401
from tableau_migration import (
    IMigrationManifestEntry,
    MigrationManifest,
    MigrationManifestSerializer)

class TestManifestSaveLoad(AutoFixtureTestBase):
    
    def test_saveload(self):
        serializer = MigrationManifestSerializer()
        manifest = MigrationManifest(self.create(IMigrationManifest))
        
        with tempfile.TemporaryDirectory() as temp_dir:
            temp_file_path = os.path.join(temp_dir, 'manifest.json')
            serializer.save(manifest, temp_file_path)
            loaded = serializer.load(temp_file_path)
        
        assert manifest.plan_id == loaded.plan_id
        assert manifest.manifest_version == loaded.manifest_version
        assert manifest.migration_id == loaded.migration_id
        
        manifest_entries = [IMigrationManifestEntry(x) for x in manifest.entries]
        loaded_entries = [IMigrationManifestEntry(x) for x in loaded.entries]
        assert len(manifest_entries) > 0
        assert len(loaded_entries) > 0
        assert len(manifest_entries) == len(loaded_entries)
        
        assert manifest.errors.Count > 0
        assert loaded.errors.Count > 0
        assert manifest.errors.Count == loaded.errors.Count


# region _generated

from enum import IntEnum # noqa: E402, F401
from tableau_migration.migration import (  # noqa: E402, F401
    PyContentLocation,
    PyContentReference
)
from typing_extensions import Self # noqa: E402, F401

from Tableau.Migration.Engine.Manifest import IMigrationManifestEntryEditor # noqa: E402, F401

from tableau_migration.migration_engine_manifest import (  # noqa: E402, F401
    PyMigrationManifestEntryEditor,
    PyMigrationManifestEntryStatus
)


from Tableau.Migration.Engine.Manifest import MigrationManifestEntryStatus

# Extra imports for tests.
from tests.helpers.autofixture import AutoFixtureTestBase # noqa: E402, F401

class TestPyMigrationManifestEntryEditorGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IMigrationManifestEntryEditor)
        py = PyMigrationManifestEntryEditor(dotnet)
        assert py._dotnet == dotnet
    

# endregion

