# Copyright (c) 2026, Salesforce, Inc.
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
    PyContentReference,
    PyContentLocation
)
from typing import (  # noqa: E402, F401
    Optional,
    Sequence
)
from typing_extensions import Self # noqa: E402, F401

import System # noqa: E402

from Tableau.Migration.Engine.Manifest import (  # noqa: E402, F401
    IMigrationManifestEntry,
    IMigrationManifestEntryEditor
)

from tableau_migration.migration_engine_manifest import (  # noqa: E402, F401
    PyMigrationManifestEntry,
    PyMigrationManifestEntryEditor,
    PyMigrationManifestEntryStatus
)


from Tableau.Migration.Engine.Manifest import MigrationManifestEntryStatus

# Extra imports for tests.
from tests.helpers.autofixture import AutoFixtureTestBase # noqa: E402, F401

class TestPyMigrationManifestEntryGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IMigrationManifestEntry)
        py = PyMigrationManifestEntry(dotnet)
        assert py._dotnet == dotnet
    
    def test_source_getter(self):
        dotnet = self.create(IMigrationManifestEntry)
        py = PyMigrationManifestEntry(dotnet)
        assert py.source == None if dotnet.Source is None else PyContentReference(dotnet.Source)
    
    def test_mapped_location_getter(self):
        dotnet = self.create(IMigrationManifestEntry)
        py = PyMigrationManifestEntry(dotnet)
        assert py.mapped_location == None if dotnet.MappedLocation is None else PyContentLocation(dotnet.MappedLocation)
    
    def test_destination_getter(self):
        dotnet = self.create(IMigrationManifestEntry)
        py = PyMigrationManifestEntry(dotnet)
        assert py.destination == None if dotnet.Destination is None else PyContentReference(dotnet.Destination)
    
    def test_status_getter(self):
        dotnet = self.create(IMigrationManifestEntry)
        py = PyMigrationManifestEntry(dotnet)
        assert py.status.value == (None if dotnet.Status is None else PyMigrationManifestEntryStatus(dotnet.Status.value__)).value
    
    def test_has_migrated_getter(self):
        dotnet = self.create(IMigrationManifestEntry)
        py = PyMigrationManifestEntry(dotnet)
        assert py.has_migrated == dotnet.HasMigrated
    
    def test_cascade_skip_getter(self):
        dotnet = self.create(IMigrationManifestEntry)
        py = PyMigrationManifestEntry(dotnet)
        assert py.cascade_skip == dotnet.CascadeSkip
    
    def test_errors_getter(self):
        dotnet = self.create(IMigrationManifestEntry)
        py = PyMigrationManifestEntry(dotnet)
        assert dotnet.Errors.Count != 0
        assert len(py.errors) == dotnet.Errors.Count
    
    def test_skipped_reason_getter(self):
        dotnet = self.create(IMigrationManifestEntry)
        py = PyMigrationManifestEntry(dotnet)
        assert py.skipped_reason == dotnet.SkippedReason
    
class TestPyMigrationManifestEntryEditorGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IMigrationManifestEntryEditor)
        py = PyMigrationManifestEntryEditor(dotnet)
        assert py._dotnet == dotnet
    

# endregion

