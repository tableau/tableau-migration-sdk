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

"""Wrapper for classes in Tableau.Migration.Engine.Migrators namespace."""
from Tableau.Migration import IMigrator
from tableau_migration import (
    cancellation_token
)
from tableau_migration.migration import (
   PyMigrationResult,
   PyMigrationManifest,
   get_service_provider, 
   get_service
)
from tableau_migration.migration_engine import PyMigrationPlan

class PyMigrator():
    """Interface for an object that can migration Tableau data between Tableau sites."""
    
    _dotnet_base = IMigrator

    def __init__(self) -> None:
        """Default init.
        
        Returns: None.
        """
        self._services = get_service_provider()
        self._migrator = get_service(self._services, IMigrator)
        
    def execute(self, plan: PyMigrationPlan, previous_manifest: PyMigrationManifest = None, cancel = None):
        """Executes a migration asynchronously.

        Args:
            plan: The migration plan to execute.
            previous_manifest: A manifest from a previous migration of the same plan to use to determine what progress has already been made.
            cancel: The cancellation token to obey.

        Returns: The results of the migration.
        """
        if cancel is None:
            cancel = cancellation_token

        if(previous_manifest is None):
            return PyMigrationResult(self._migrator.ExecuteAsync(plan._migration_plan, cancel).GetAwaiter().GetResult())
        else:
            return PyMigrationResult(self._migrator.ExecuteAsync(plan._migration_plan, previous_manifest._migration_manifest, cancel).GetAwaiter().GetResult())
# region _generated

from tableau_migration.migration import (  # noqa: E402, F401
    _generic_wrapper,
    PyResult
)
from tableau_migration.migration_engine_manifest import PyMigrationManifestEntry # noqa: E402, F401
from typing import (  # noqa: E402, F401
    Generic,
    TypeVar
)
from typing_extensions import Self # noqa: E402, F401

from Tableau.Migration.Engine.Migrators import IContentItemMigrationResult # noqa: E402, F401

TContent = TypeVar("TContent")

class PyContentItemMigrationResult(Generic[TContent], PyResult):
    """IResult object for a content item migration action."""
    
    _dotnet_base = IContentItemMigrationResult
    
    def __init__(self, content_item_migration_result: IContentItemMigrationResult) -> None:
        """Creates a new PyContentItemMigrationResult object.
        
        Args:
            content_item_migration_result: A IContentItemMigrationResult object.
        
        Returns: None.
        """
        self._dotnet = content_item_migration_result
        
    @property
    def continue_batch(self) -> bool:
        """Gets whether or not the current migration batch should continue."""
        return self._dotnet.ContinueBatch
    
    @property
    def is_canceled(self) -> bool:
        """Gets whether the item migration was canceled."""
        return self._dotnet.IsCanceled
    
    @property
    def manifest_entry(self) -> PyMigrationManifestEntry:
        """Gets the manifest entry for the content item."""
        return None if self._dotnet.ManifestEntry is None else PyMigrationManifestEntry(self._dotnet.ManifestEntry)
    
    def for_continue_batch(self, continue_batch: bool) -> Self:
        """Creates a new ContinueBatch value.
        
        Args:
            continue_batch: Whether or not the current migration batch should continue.
        
        Returns: The new IContentItemMigrationResult object.
        """
        result = self._dotnet.ForContinueBatch(continue_batch)
        return None if result is None else PyContentItemMigrationResult[TContent](result)
    

# endregion

