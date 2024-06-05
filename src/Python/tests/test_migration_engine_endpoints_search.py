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

from uuid import UUID

from tableau_migration.migration import (
    PyContentLocation
)

from tableau_migration.migration_engine_endpoints_search import (
    PyDestinationContentReferenceFinderFactory,
    PyDestinationContentReferenceFinder,
    PySourceContentReferenceFinderFactory,
    PySourceContentReferenceFinder
)

from tableau_migration.migration_content import (
    PyUser,
    PyGroup,
    PyProject,
    PyDataSource,
    PyWorkbook
)

from tests.helpers.autofixture import AutoFixtureTestBase

from System import (
    IServiceProvider
)

from System.Threading import (
    CancellationToken
)

from Microsoft.Extensions.DependencyInjection import (
    ServiceCollectionContainerBuilderExtensions, 
    ServiceCollection
)

import Moq

from Tableau.Migration import (
    ContentLocation
)

from Tableau.Migration.Content import ( 
    IUser,
    IGroup,
    IProject,
    IDataSource,
    IWorkbook
)

from Tableau.Migration.Engine.Endpoints.Search import (
    IDestinationContentReferenceFinder,
    ISourceContentReferenceFinder,
    ManifestDestinationContentReferenceFinderFactory,
    ManifestSourceContentReferenceFinderFactory
)

class TestPySourceContentReferenceFinderFactory(AutoFixtureTestBase):
    _test_data = [
        (IUser, None),
        (PyUser, PySourceContentReferenceFinder),
        (IGroup, None),
        (PyGroup, PySourceContentReferenceFinder),
        (IProject, None),
        (PyProject, PySourceContentReferenceFinder),
        (IDataSource, None),
        (PyDataSource, PySourceContentReferenceFinder),
        (IWorkbook, None),
        (PyWorkbook, PySourceContentReferenceFinder)
    ]

    @pytest.mark.parametrize("searched_type", _test_data)
    def test_for_source_content_type_nothing_registered(self, searched_type):
        service_collection = ServiceCollection()
        provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(service_collection)
        factory = PySourceContentReferenceFinderFactory(ManifestSourceContentReferenceFinderFactory(provider))

        finder = factory.for_source_content_type(searched_type)

        assert finder is None

    @pytest.mark.parametrize("searched_type, expected_type", _test_data)
    def test_for_source_content_type(self, searched_type, expected_type):
        provider = self.create(IServiceProvider)
        
        factory = PySourceContentReferenceFinderFactory(ManifestSourceContentReferenceFinderFactory(provider))

        finder = factory.for_source_content_type(searched_type)

        if expected_type is None:
            assert finder is None
        else:
            assert isinstance(finder, expected_type)
            assert finder._content_type is searched_type

class TestPyDestinationContentReferenceFinderFactory(AutoFixtureTestBase):
    _test_data = [
        (IUser, None),
        (PyUser, PyDestinationContentReferenceFinder),
        (IGroup, None),
        (PyGroup, PyDestinationContentReferenceFinder),
        (IProject, None),
        (PyProject, PyDestinationContentReferenceFinder),
        (IDataSource, None),
        (PyDataSource, PyDestinationContentReferenceFinder),
        (IWorkbook, None),
        (PyWorkbook, PyDestinationContentReferenceFinder)
    ]

    @pytest.mark.parametrize("searched_type", _test_data)
    def test_for_source_content_type_nothing_registered(self, searched_type):
        service_collection = ServiceCollection()
        provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(service_collection)
        factory = PyDestinationContentReferenceFinderFactory(ManifestDestinationContentReferenceFinderFactory(provider))

        finder = factory.for_destination_content_type(searched_type)

        assert finder is None

    @pytest.mark.parametrize("searched_type, expected_type", _test_data)
    def test_for_source_content_type(self, searched_type, expected_type):
        provider = self.create(IServiceProvider)
        
        factory = PyDestinationContentReferenceFinderFactory(ManifestDestinationContentReferenceFinderFactory(provider))

        finder = factory.for_destination_content_type(searched_type)

        if expected_type is None:
            assert finder is None
        else:
            assert isinstance(finder, expected_type)
            assert finder._content_type is searched_type
            
class TestPySourceContentReferenceFinder(AutoFixtureTestBase):
    def test_find_by_id_with_none_id(self):
        dotnet_source_finder = Moq.Mock[ISourceContentReferenceFinder[IUser]]()
        finder = PySourceContentReferenceFinder(dotnet_source_finder.Object, IUser)

        result = finder.find_by_id(None)
        
        assert result is None

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in dotnet_source_finder.Invocations]

        assert invokedMethodNames == []
        
    def test_find_by_id(self):
        dotnet_source_finder = self.create(ISourceContentReferenceFinder[IUser])
        finder = PySourceContentReferenceFinder(dotnet_source_finder, IUser)

        result = finder.find_by_id(UUID('{12345678-0000-0000-0000-000000000000}'))
        
        assert result is not None
        
    def test_find_by_id_with_cancel(self):
        dotnet_source_finder = self.create(ISourceContentReferenceFinder[IUser])
        finder = PySourceContentReferenceFinder(dotnet_source_finder, IUser)

        result = finder.find_by_id(UUID('{12345678-0000-0000-0000-000000000000}'), CancellationToken(True))
        
        assert result is not None
        
    def test_find_by_source_location_with_none_location(self):
        dotnet_source_finder = Moq.Mock[ISourceContentReferenceFinder[IProject]]()
        finder = PySourceContentReferenceFinder(dotnet_source_finder.Object, IProject)

        result = finder.find_by_source_location(None)
        
        assert result is None

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in dotnet_source_finder.Invocations]

        assert invokedMethodNames == []
        
    def test_find_by_source_location(self):
        dotnet_source_finder = self.create(ISourceContentReferenceFinder[IProject])
        finder = PySourceContentReferenceFinder(dotnet_source_finder, IProject)

        result = finder.find_by_source_location(PyContentLocation(ContentLocation(["parent", "child", "item"])))
        
        assert result is not None
        
    def test_find_by_source_location_with_cancel(self):
        dotnet_source_finder = self.create(ISourceContentReferenceFinder[IProject])
        finder = PySourceContentReferenceFinder(dotnet_source_finder, IProject)

        result = finder.find_by_source_location(PyContentLocation(ContentLocation(["parent", "child", "item"])), CancellationToken(True))
        
        assert result is not None
            
class TestPyDestinationContentReferenceFinder(AutoFixtureTestBase):
    _empty_test_data = [
        (""),
        (None),
        ("   ")
    ]

    def test_find_by_id_with_none_id(self):
        dotnet_destination_finder = Moq.Mock[IDestinationContentReferenceFinder[IGroup]]()
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder.Object, IGroup)

        result = finder.find_by_id(None)
        
        assert result is None

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in dotnet_destination_finder.Invocations]

        assert invokedMethodNames == []
        
    def test_find_by_id(self):
        dotnet_destination_finder = self.create(IDestinationContentReferenceFinder[IGroup])
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder, IGroup)

        result = finder.find_by_id(UUID('{12345678-0000-0000-0000-000000000000}'))
        
        assert result is not None
        
    def test_find_by_id_with_cancel(self):
        dotnet_destination_finder = self.create(IDestinationContentReferenceFinder[IGroup])
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder, IGroup)

        result = finder.find_by_id(UUID('{12345678-0000-0000-0000-000000000000}'), CancellationToken(True))
        
        assert result is not None
        
    def test_find_by_source_id_with_none_id(self):
        dotnet_destination_finder = Moq.Mock[IDestinationContentReferenceFinder[IGroup]]()
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder.Object, IGroup)

        result = finder.find_by_source_id(None)
        
        assert result is None

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in dotnet_destination_finder.Invocations]

        assert invokedMethodNames == []
        
    def test_find_by_source_id(self):
        dotnet_destination_finder = self.create(IDestinationContentReferenceFinder[IGroup])
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder, IGroup)

        result = finder.find_by_source_id(UUID('{12345678-0000-0000-0000-000000000000}'))
        
        assert result is not None
        
    def test_find_by_source_id_with_cancel(self):
        dotnet_destination_finder = self.create(IDestinationContentReferenceFinder[IGroup])
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder, IGroup)

        result = finder.find_by_source_id(UUID('{12345678-0000-0000-0000-000000000000}'), CancellationToken(True))
        
        assert result is not None
        
    def test_find_by_source_location_with_none_location(self):
        dotnet_destination_finder = Moq.Mock[IDestinationContentReferenceFinder[IProject]]()
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder.Object, IProject)

        result = finder.find_by_source_location(None)
        
        assert result is None

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in dotnet_destination_finder.Invocations]

        assert invokedMethodNames == []
        
    def test_find_by_source_location(self):
        dotnet_destination_finder = self.create(IDestinationContentReferenceFinder[IProject])
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder, IProject)

        result = finder.find_by_source_location(PyContentLocation(ContentLocation(["parent", "child", "item"])))
        
        assert result is not None
        
    def test_find_by_source_location_with_cancel(self):
        dotnet_destination_finder = self.create(IDestinationContentReferenceFinder[IProject])
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder, IProject)

        result = finder.find_by_source_location(PyContentLocation(ContentLocation(["parent", "child", "item"])), CancellationToken(True))
        
        assert result is not None
        
    def test_find_by_mapped_location_with_none_location(self):
        dotnet_destination_finder = Moq.Mock[IDestinationContentReferenceFinder[IUser]]()
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder.Object, IUser)

        result = finder.find_by_mapped_location(None)
        
        assert result is None

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in dotnet_destination_finder.Invocations]

        assert invokedMethodNames == []
        
    def test_find_by_mapped_location(self):
        dotnet_destination_finder = self.create(IDestinationContentReferenceFinder[IUser])
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder, IUser)

        result = finder.find_by_mapped_location(PyContentLocation(ContentLocation(["parent", "child", "item"])))
        
        assert result is not None
        
    def test_find_by_mapped_location_with_cancel(self):
        dotnet_destination_finder = self.create(IDestinationContentReferenceFinder[IUser])
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder, IUser)

        result = finder.find_by_mapped_location(PyContentLocation(ContentLocation(["parent", "child", "item"])), CancellationToken(True))
        
        assert result is not None
    
    @pytest.mark.parametrize("empty_value", _empty_test_data)
    def test_find_by_source_content_url_with_empty_url(self, empty_value):
        dotnet_destination_finder = Moq.Mock[IDestinationContentReferenceFinder[IUser]]()
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder.Object, IUser)

        result = finder.find_by_source_content_url(empty_value)
        
        assert result is None

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in dotnet_destination_finder.Invocations]

        assert invokedMethodNames == []
        
    def test_find_by_source_content_url(self):
        dotnet_destination_finder = self.create(IDestinationContentReferenceFinder[IUser])
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder, IUser)

        result = finder.find_by_source_content_url("test")
        
        assert result is not None
        
    def test_find_by_source_content_url_with_cancel(self):
        dotnet_destination_finder = self.create(IDestinationContentReferenceFinder[IUser])
        finder = PyDestinationContentReferenceFinder(dotnet_destination_finder, IUser)

        result = finder.find_by_source_content_url("test2", CancellationToken(True))
        
        assert result is not None
        