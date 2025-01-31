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

"""Wrapper for classes in Tableau.Migration.Engine.Manifest namespace."""

from tableau_migration import (
    cancellation_token
)

from tableau_migration.migration import (  # noqa: E402, F401
    PyMigrationManifest,
    get_service_provider,
    get_service
)

from Tableau.Migration.Engine.Manifest import (  # noqa: E402, F401
    MigrationManifestSerializer
)

class PyMigrationManifestSerializer():
    """Provides functionality to serialize and deserialize migration manifests in JSON format."""
    
    _dotnet_base = MigrationManifestSerializer
    
    def __init__(self) -> None:
        """Creates a new PyMigrationManifestSerializer object.
        
        Args:
            migration_manifest_serializer: A MigrationManifestSerializer object.
        
        Returns: None.
        """
        self._services = get_service_provider()
        self._dotnet = get_service(self._services, MigrationManifestSerializer)
        
    def save(self, manifest: PyMigrationManifest, path: str) -> None:
        """Saves a manifest in JSON format.
        
        Args:
            manifest: The manifest to save.
            path: The file path to save the manifest to.
        """
        self._dotnet.SaveAsync(manifest._migration_manifest, path).GetAwaiter().GetResult()
    
    def load(self, path: str) -> PyMigrationManifest:
        """Loads a manifest from JSON format.
        
        Args:
            path: The file path to load the manifest from.
            cancel: A cancellation token to cancel the operation.
        
        Returns: The loaded MigrationManifest, or None if the manifest could not be loaded.
        """
        result = self._dotnet.LoadAsync(path, cancellation_token).GetAwaiter().GetResult()
        return None if result is None else PyMigrationManifest(result)

    @classmethod
    def get_supported_manifest_version(cls) -> int:
        """This is the current MigrationManifest.ManifestVersion that this serializer supports."""
        return MigrationManifestSerializer.SupportedManifestVersion

# region _generated

from enum import IntEnum # noqa: E402, F401
from tableau_migration.migration import (  # noqa: E402, F401
    PyContentReference,
    PyContentLocation
)
from typing import Sequence # noqa: E402, F401
from typing_extensions import Self # noqa: E402, F401

import System # noqa: E402

from Tableau.Migration.Engine.Manifest import (  # noqa: E402, F401
    IMigrationManifestEntry,
    IMigrationManifestEntryEditor
)

class PyMigrationManifestEntryStatus(IntEnum):
    """An enumeration of the various migration statuses states of a content item."""
    
    """The content item has not yet been processed."""
    PENDING = 0
    
    """The content item was not migrated due to filtering."""
    SKIPPED = 1
    
    """The content item was migrated successfully."""
    MIGRATED = 2
    
    """An attempt was made to migrate the content item, but it resulted in one or more errors. The content item may be missing on the destination or may be partially migrated."""
    ERROR = 3
    
    """An attempt was made to migrate the content item, but the process was canceled mid-migration. The content item may be missing on the destination or may be partially migrated."""
    CANCELED = 4
    
class PyMigrationManifestEntry():
    """Interface for an entry on a IMigrationManifest that describes the migration state of single content item."""
    
    _dotnet_base = IMigrationManifestEntry
    
    def __init__(self, migration_manifest_entry: IMigrationManifestEntry) -> None:
        """Creates a new PyMigrationManifestEntry object.
        
        Args:
            migration_manifest_entry: A IMigrationManifestEntry object.
        
        Returns: None.
        """
        self._dotnet = migration_manifest_entry
        
    @property
    def source(self) -> PyContentReference:
        """Gets the content item's source information."""
        return None if self._dotnet.Source is None else PyContentReference(self._dotnet.Source)
    
    @property
    def mapped_location(self) -> PyContentLocation:
        """Gets the content item's intended destination location, regardless if a Destination value's location if available."""
        return None if self._dotnet.MappedLocation is None else PyContentLocation(self._dotnet.MappedLocation)
    
    @property
    def destination(self) -> PyContentReference:
        """Gets the content item's destination information, or null if the content item was not migrated due to filtering, or otherwise not found in the destination during the course of the migration."""
        return None if self._dotnet.Destination is None else PyContentReference(self._dotnet.Destination)
    
    @property
    def status(self) -> PyMigrationManifestEntryStatus:
        """Gets the migration status code of the content item for the current run. See HasMigrated for the migration status across all runs."""
        return None if self._dotnet.Status is None else PyMigrationManifestEntryStatus(self._dotnet.Status.value__)
    
    @property
    def has_migrated(self) -> bool:
        """Gets whether or not the content item has been migrated, either in a previous run or the current run."""
        return self._dotnet.HasMigrated
    
    @property
    def errors(self) -> Sequence[System.Exception]:
        """Gets errors that occurred while migrating the content item."""
        return None if self._dotnet.Errors is None else list(self._dotnet.Errors)
    
class PyMigrationManifestEntryEditor(PyMigrationManifestEntry):
    """Interface for a IMigrationManifestEntry that can be edited."""
    
    _dotnet_base = IMigrationManifestEntryEditor
    
    def __init__(self, migration_manifest_entry_editor: IMigrationManifestEntryEditor) -> None:
        """Creates a new PyMigrationManifestEntryEditor object.
        
        Args:
            migration_manifest_entry_editor: A IMigrationManifestEntryEditor object.
        
        Returns: None.
        """
        self._dotnet = migration_manifest_entry_editor
        
    def reset_status(self) -> Self:
        """Resets the status to Pending.
        
        Returns: The current entry editor, for fluent API usage.
        """
        result = self._dotnet.ResetStatus()
        return None if result is None else PyMigrationManifestEntryEditor(result)
    
    def map_to_destination(self, destination_location: PyContentLocation) -> Self:
        """Sets the intended mapped destination location to the manifest entry. Clears the Destination information if the mapped location is different.
        
        Args:
            destination_location: The intended destination location to migrate to.
        
        Returns: The current entry editor, for fluent API usage.
        """
        result = self._dotnet.MapToDestination(None if destination_location is None else destination_location._dotnet)
        return None if result is None else PyMigrationManifestEntryEditor(result)
    
    def destination_found(self, destination_info: PyContentReference) -> Self:
        """Sets the MappedLocation information based on the given destination item reference.
        
        Args:
            destination_info: The destination reference information.
        
        Returns: The current entry editor, for fluent API usage.
        """
        result = self._dotnet.DestinationFound(None if destination_info is None else destination_info._dotnet)
        return None if result is None else PyMigrationManifestEntryEditor(result)
    
    def set_skipped(self) -> Self:
        """Sets the entry to skipped status.
        
        Returns: The current entry editor, for fluent API usage.
        """
        result = self._dotnet.SetSkipped()
        return None if result is None else PyMigrationManifestEntryEditor(result)
    
    def set_canceled(self) -> Self:
        """Sets the entry to canceled status.
        
        Returns: The current entry editor, for fluent API usage.
        """
        result = self._dotnet.SetCanceled()
        return None if result is None else PyMigrationManifestEntryEditor(result)
    
    def set_migrated(self) -> Self:
        """Sets the entry to migrated status.
        
        Returns: The current entry editor, for fluent API usage.
        """
        result = self._dotnet.SetMigrated()
        return None if result is None else PyMigrationManifestEntryEditor(result)
    

# endregion

