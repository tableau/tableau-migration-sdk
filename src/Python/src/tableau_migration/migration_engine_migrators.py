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

"""Wrapper for classes in Tableau.Migration.Engine.Migrators namespace."""
import tableau_migration
from Tableau.Migration import IMigrator
from tableau_migration.migration import (
   PyMigrationResult,
   PyMigrationManifest,
   get_service_provider, 
   get_service )
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
        
    def execute(self, plan: PyMigrationPlan, previous_manifest: PyMigrationManifest = None, cancel = tableau_migration.cancellation_token):
        """Executes a migration asynchronously.

        Args:
            plan: The migration plan to execute.
            previous_manifest: A manifest from a previous migration of the same plan to use to determine what progress has already been made.
            cancel: A cancellation token to obey.

        Returns: The results of the migration.
        """
        if(previous_manifest is None):
            return PyMigrationResult(self._migrator.ExecuteAsync(plan._migration_plan, cancel).GetAwaiter().GetResult())
        else:
            return PyMigrationResult(self._migrator.ExecuteAsync(plan._migration_plan, previous_manifest._migration_manifest, cancel).GetAwaiter().GetResult())
