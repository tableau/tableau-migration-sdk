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

"""Wrapper for classes in Tableau.Migration.Engine.Options namespace."""
from typing import Type
from typing_extensions import Self, TypeVar

import System

from Tableau.Migration.Engine.Options import(
    IMigrationPlanOptionsBuilder,
    IMigrationPlanOptionsCollection)
from tableau_migration.migration import (
    get_service_provider
)

T = TypeVar("T")
class PyMigrationPlanOptionsCollection:
    """Default IMigrationPlanOptionsCollection implementation."""
    
    _dotnet_base = IMigrationPlanOptionsCollection
    
    def __init__(self, migration_plan_options_collection: IMigrationPlanOptionsCollection):
        """Default init.

        Args:
            migration_plan_options_collection: An object that contains plan-specific options objects
        
        Returns: None.
        """
        self._migration_plan_options_collection = migration_plan_options_collection


    def get(self, type_to_get: Type[T], services: System.IServiceProvider = None) -> Type[T]:
        """Gets the options for the given type.
        
        or null if no options for the given type have been registered.
        

        Args:
            type_to_get: The option type
            services: A service provider

        Returns:
            The options for the given type, or null.
        """
        if services is None:
            services = get_service_provider()

        return self._migration_plan_options_collection.Get[type_to_get](services)

class PyMigrationPlanOptionsBuilder():
    """Default IMigrationPlanOptionsBuilder implementation."""

    _dotnet_base = IMigrationPlanOptionsBuilder

    def __init__(self, migration_plan_options_builder: IMigrationPlanOptionsBuilder):
        """Default init.

        Args:
            migration_plan_options_builder: An object that can build a set of per-plan options.
        
        Returns: None.
        """
        self._migration_plan_options_builder = migration_plan_options_builder

    
    def configure(self, options: Type[T]) -> Self:
        """Sets the configuration for a given options type.

        Args:
            options: The option type

        Returns:
            The same options builder, for fluent API usage.
        """
        self._migration_plan_options_builder.Configure(options)
        return self


    def build(self) -> PyMigrationPlanOptionsCollection:
        """Builds the options collection.

        Returns:
            The options collection.
        """
        return PyMigrationPlanOptionsCollection(self._migration_plan_options_builder.Build()) 



