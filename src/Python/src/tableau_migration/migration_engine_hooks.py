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

"""Wrapper for classes in Tableau.Migration.Engine.Hooks namespace."""

from typing import Type, TypeVar

from Tableau.Migration.Engine.Hooks import IMigrationHookFactoryCollection

T = TypeVar("T")

class PyMigrationHookFactoryCollection():
    """Interface for an object that contains MigrationHookFactorys registered for each hook type."""

    _dotnet_base = IMigrationHookFactoryCollection

    def __init__(self, migration_hook_factory_collection: IMigrationHookFactoryCollection) -> None:
        """Default init.

        Args:
            migration_hook_factory_collection: A collection that contains MigrationHookFactory registered for each hook type.
        
        Returns: None.
        """
        self._migration_hook_factory_collection = migration_hook_factory_collection

    def get_hooks(self, type_to_get: Type[T]):
        """Gets the MigrationHookFactorys for the given hook type.
        
        This type has to be an interface that inherits from IMigrationHook.

        Args:
            type_to_get: the hook type
        """
        return self._migration_hook_factory_collection.GetHooks[type_to_get]()