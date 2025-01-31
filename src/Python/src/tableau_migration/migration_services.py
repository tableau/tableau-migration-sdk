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

"""Service-related Python utility for the Migration SDK."""

# Not in migration.py to avoid circular references.

from typing import Type, TypeVar

from System import IServiceProvider
from Tableau.Migration import (
    IMigrationManifest, 
    IMigrationPlan
)
from Tableau.Migration.Engine.Endpoints.Search import (
    IDestinationContentReferenceFinderFactory,
    ISourceContentReferenceFinderFactory
)

from tableau_migration.migration import (
    get_service,
    PyMigrationManifest
)
from tableau_migration.migration_engine import (
    PyMigrationPlan
)
from tableau_migration.migration_engine_endpoints_search import (
    PyDestinationContentReferenceFinder,
    PyDestinationContentReferenceFinderFactory,
    PySourceContentReferenceFinder,
    PySourceContentReferenceFinderFactory
)

T = TypeVar("T")
TContent = TypeVar("TContent")

class ScopedMigrationServices():
    """Class that represents a migration-scoped service provider."""

    def __init__(self, scoped_services: IServiceProvider) -> None:
        """Default init.
        
        Args:
            scoped_services: The scoped service provider.
        
        Returns: None.
        """
        self._scoped_services = scoped_services

    def _get_service(self, t: Type[T]) -> T:
        """Get the service from the service provider.

        Returns: The service.
        """
        return get_service(self._scoped_services, t)
    
    def get_manifest(self) -> PyMigrationManifest:
        """Get the current migration manifest.

        Returns: The current IMigrationManifest.
        """
        return PyMigrationManifest(self._get_service(IMigrationManifest))

    def get_plan(self) -> PyMigrationPlan:
        """Get the current migration plan.

        Returns: The current IMigrationPlan.
        """
        return PyMigrationPlan(self._get_service(IMigrationPlan))

    def get_source_finder_factory(self) -> PySourceContentReferenceFinderFactory:
        """Get the current source finder factory.

        Returns: The current ISourceContentReferenceFinderFactory.
        """
        return PySourceContentReferenceFinderFactory(self._get_service(ISourceContentReferenceFinderFactory))

    def get_source_finder(self, t: Type[TContent]) -> PySourceContentReferenceFinder[TContent]:
        """Get the TContent source finder.

        Args:
            t: The content type.
        
        Returns: The current PySourceContentReferenceFinder for TContent.
        """
        return self.get_source_finder_factory().for_source_content_type(t)

    def get_destination_finder_factory(self) -> PyDestinationContentReferenceFinderFactory:
        """Get the current destination finder factory.

        Returns: The current IDestinationContentReferenceFinderFactory.
        """
        return PyDestinationContentReferenceFinderFactory(self._get_service(IDestinationContentReferenceFinderFactory))

    def get_destination_finder(self, t: Type[TContent]) -> PyDestinationContentReferenceFinder[TContent]:
        """Get the TContent destination finder.
        
        Args:
            t: The content type.
        
        Returns: The current PyDestinationContentReferenceFinder for TContent.
        """
        return self.get_destination_finder_factory().for_destination_content_type(t)
