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

"""Wrapper for result classes in Tableau.Migration.Engine.Hooks namespace."""

# Not in migration_engine_hooks.py to avoid circular references.

from typing import Sequence
from typing_extensions import Self

from tableau_migration.migration import PyResult
from migration_services import ScopedMigrationServices

from System import Exception as DotNetException
from Tableau.Migration.Engine.Hooks import IInitializeMigrationHookResult

class PyInitializeMigrationHookResult(PyResult):
    """IInitializeMigrationHook."""
    
    _dotnet_base = IInitializeMigrationHookResult
    
    def __init__(self, initialize_migration_hook_result: IInitializeMigrationHookResult) -> None:
        """Creates a new PyInitializeMigrationHookResult object.
        
        Args:
            initialize_migration_hook_result: A IInitializeMigrationHookResult object.
        
        Returns: None.
        """
        self._dotnet = initialize_migration_hook_result
        
    @property
    def scoped_services(self) -> ScopedMigrationServices:
        """Gets the migration-scoped service provider."""
        return ScopedMigrationServices(self._dotnet.ScopedServices)
    
    def to_failure(self, errors: Sequence[DotNetException]=None) -> Self:
        """Creates a new IInitializeMigrationHookResult object with the given errors.
        
        Args:
            errors: The errors that caused the failure.
        
        Returns: The new IInitializeMigrationHookResult object.
        """
        result = self._dotnet.ToFailure(errors)
        return None if result is None else PyInitializeMigrationHookResult(result)