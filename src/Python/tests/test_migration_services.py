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

import pytest

from System import IServiceProvider

from tableau_migration.migration import (
    PyMigrationManifest
)
from tableau_migration.migration_content import (
    PyUser,
    PyGroup,
    PyProject,
    PyDataSource,
    PyWorkbook
)
from tableau_migration.migration_engine import (
    PyMigrationPlan
)

from tableau_migration.migration_engine_endpoints_search import (
    PyDestinationContentReferenceFinderFactory,
    PyDestinationContentReferenceFinder,
    PySourceContentReferenceFinderFactory,
    PySourceContentReferenceFinder
)

from tableau_migration.migration_services import (
    ScopedMigrationServices
)

from tests.helpers.autofixture import AutoFixtureTestBase

class TestPySourceContentReferenceFinder(AutoFixtureTestBase):
    _test_data = [
        PyUser,
        PyGroup,
        PyProject,
        PyDataSource,
        PyWorkbook
    ]

    def test_get_manifest(self):
        dotnet_provider = self.create(IServiceProvider)
        scoped_services = ScopedMigrationServices(dotnet_provider)

        result = scoped_services.get_manifest()
        
        assert result is not None
        assert isinstance(result, PyMigrationManifest)
        
    def test_get_plan(self):
        dotnet_provider = self.create(IServiceProvider)
        scoped_services = ScopedMigrationServices(dotnet_provider)

        result = scoped_services.get_plan()
        
        assert result is not None
        assert isinstance(result, PyMigrationPlan)
        
    def test_get_source_finder_factory(self):
        dotnet_provider = self.create(IServiceProvider)
        scoped_services = ScopedMigrationServices(dotnet_provider)

        result = scoped_services.get_source_finder_factory()
        
        assert result is not None
        assert isinstance(result, PySourceContentReferenceFinderFactory)
    
    @pytest.mark.parametrize("searched_type", _test_data)
    def test_get_source_finder(self, searched_type):
        dotnet_provider = self.create(IServiceProvider)
        scoped_services = ScopedMigrationServices(dotnet_provider)

        result = scoped_services.get_source_finder(searched_type)
        
        assert result is not None
        assert isinstance(result, PySourceContentReferenceFinder)
        assert result._content_type is searched_type
        
    def test_get_destination_finder_factory(self):
        dotnet_provider = self.create(IServiceProvider)
        scoped_services = ScopedMigrationServices(dotnet_provider)

        result = scoped_services.get_destination_finder_factory()
        
        assert result is not None
        assert isinstance(result, PyDestinationContentReferenceFinderFactory)
        
    @pytest.mark.parametrize("searched_type", _test_data)
    def test_get_destination_finder(self, searched_type):
        dotnet_provider = self.create(IServiceProvider)
        scoped_services = ScopedMigrationServices(dotnet_provider)

        result = scoped_services.get_destination_finder(searched_type)
        
        assert result is not None
        assert isinstance(result, PyDestinationContentReferenceFinder)
        assert result._content_type is searched_type
