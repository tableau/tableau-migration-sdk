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

"""Wrapper for classes in Tableau.Migration.TestComponents.Engine.Manifest namespace."""
import tableau_migration.migration

from Tableau.Migration.TestComponents.Engine.Manifest import MigrationManifestSerializer
from System.Text.Json import JsonSerializerOptions
from tableau_migration.migration import PyMigrationManifest

class PyMigrationManifestSerializer():
    """A class to serialize the migration manifest."""

    _dotnet_base = MigrationManifestSerializer
    
    def __init__(self) -> None:
        """Default __init__."""
        self._services = tableau_migration.migration.get_service_provider()
        self._migration_manifest_serializer = tableau_migration.migration.get_service(self._services, MigrationManifestSerializer)
        
    def save(self, manifest: PyMigrationManifest, path: str, cancel = tableau_migration.cancellation_token, json_options: JsonSerializerOptions = None) -> None:
        """
        Saves a manifest in JSON format.

        Args:
            manifest (PyMigrationManifest): The manifest to save.
            path (str): The file path to save the manifest to.
            cancel (_type_, optional): A cancellation token to obey. Defaults to tableau_migration.cancellation_token.
            json_options (JsonSerializerOptions, optional): Optional JSON options to use. Defaults to None.

        Returns:
            None
        """
        if(json_options is None):
            return self._migration_manifest_serializer.SaveAsync(manifest._migration_manifest, path).GetAwaiter().GetResult()
        else:
            return self._migration_manifest_serializer.SaveAsync(manifest._migration_manifest, path, json_options).GetAwaiter().GetResult()
    
    def load(self, path: str, cancel = tableau_migration.cancellation_token, json_options: JsonSerializerOptions = None) -> PyMigrationManifest | None:
        """
        Loads a manifest from JSON format.

        Args:
            path (str): The file path to load the manifest from.
            cancel (_type_, optional): A cancellation token to obey. Defaults to tableau_migration.cancellation_token.
            json_options (JsonSerializerOptions, optional): Optional JSON options to use. Defaults to None.

        Returns:
            PyMigrationManifest: The PyMigrationManifest, or null if the manifest could not be loaded.
        """
        manifest = self._migration_manifest_serializer.LoadAsync(path,cancel,json_options).GetAwaiter().GetResult()
        if manifest is not None:
            return PyMigrationManifest(manifest)
        
        return None
        
