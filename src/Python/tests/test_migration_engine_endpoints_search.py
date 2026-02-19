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


import pytest

from uuid import UUID

from tableau_migration.migration import (
    PyContentLocation
)

from tableau_migration.migration_engine_endpoints_search import (
    PyDestinationContentReferenceFinderFactory,
    PyDestinationContentReferenceFinder,
    PySourceContentReferenceFinderFactory,
    PySourceContentReferenceFinder,
    PyDestinationContentReferenceFinderBase,
    PySourceContentReferenceFinderBase
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

from Tableau.Migration.Engine import (
    IMigration
)

from Tableau.Migration.Engine.Endpoints.Search import (
    IDestinationContentReferenceFinder,
    ISourceContentReferenceFinder,
    DestinationContentReferenceFinderFactory,
    SourceContentReferenceFinderFactory
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

    @pytest.mark.parametrize("searched_type, expected_type", _test_data)
    def test_for_source_content_type(self, searched_type, expected_type):        
        factory = PySourceContentReferenceFinderFactory(SourceContentReferenceFinderFactory(self.create(IMigration)))

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

    @pytest.mark.parametrize("searched_type, expected_type", _test_data)
    def test_for_source_content_type(self, searched_type, expected_type):
        factory = PyDestinationContentReferenceFinderFactory(DestinationContentReferenceFinderFactory(self.create(IMigration)))

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


class TestCustomFinderBaseClasses(AutoFixtureTestBase):
    """Test the custom finder base classes functionality."""
    
    def test_destination_finder_base_class_methods(self):
        """Test that the destination finder base class has all required methods."""
        required_methods = [
            'find_by_source_location',
            'find_by_mapped_location', 
            'find_by_source_id',
            'find_by_id',
            'find_by_source_content_url',
            'find_all'
        ]
        
        for method_name in required_methods:
            assert hasattr(PyDestinationContentReferenceFinderBase, method_name)
            assert callable(getattr(PyDestinationContentReferenceFinderBase, method_name))
    
    def test_source_finder_base_class_methods(self):
        """Test that the source finder base class has all required methods."""
        required_methods = [
            'find_by_source_location',
            'find_by_id',
            'find_all'
        ]
        
        for method_name in required_methods:
            assert hasattr(PySourceContentReferenceFinderBase, method_name)
            assert callable(getattr(PySourceContentReferenceFinderBase, method_name))
    
    def test_destination_finder_base_class_default_behavior(self):
        """Test that the destination finder base class methods return expected defaults."""
        finder = PyDestinationContentReferenceFinderBase()
        
        # Test that all methods return None or empty list by default
        assert finder.find_by_source_location(None) is None
        assert finder.find_by_mapped_location(None) is None
        assert finder.find_by_source_id(None) is None
        assert finder.find_by_id(None) is None
        assert finder.find_by_source_content_url("") is None
        assert finder.find_all() == []
    
    def test_source_finder_base_class_default_behavior(self):
        """Test that the source finder base class methods return expected defaults."""
        finder = PySourceContentReferenceFinderBase()
        
        # Test that all methods return None or empty list by default
        assert finder.find_by_source_location(None) is None
        assert finder.find_by_id(None) is None
        assert finder.find_all() == []
    
    def test_custom_destination_finder_implementation(self):
        """Test implementing a custom destination finder."""
        class TestDestinationFinder(PyDestinationContentReferenceFinderBase):
            def __init__(self, test_data):
                self.test_data = test_data
                super().__init__()
            
            def find_by_source_location(self, source_location):
                if source_location and source_location.path == "test/path":
                    return "test-reference"  # Return a simple test value
                return None
            
            def find_by_mapped_location(self, mapped_location):
                if mapped_location and mapped_location.path == "mapped/path":
                    return "mapped-reference"
                return None
            
            def find_by_source_id(self, source_id):
                if source_id == UUID("12345678-1234-1234-1234-123456789012"):
                    return "source-id-reference"
                return None
            
            def find_by_id(self, id):
                if id == UUID("87654321-4321-4321-4321-210987654321"):
                    return "id-reference"
                return None
            
            def find_by_source_content_url(self, url):
                if url == "https://test.example.com/content":
                    return "url-reference"
                return None
            
            def find_all(self):
                return ["reference1", "reference2"]
        
        # Test instantiation
        finder = TestDestinationFinder("test-data")
        assert isinstance(finder, PyDestinationContentReferenceFinderBase)
        
        # Test methods work
        test_location = PyContentLocation(ContentLocation(["test", "path"]))
        result = finder.find_by_source_location(test_location)
        assert result == "test-reference"
        
        # Test find_all returns list
        all_results = finder.find_all()
        assert isinstance(all_results, list)
        assert len(all_results) == 2
        assert all_results == ["reference1", "reference2"]
    
    def test_custom_source_finder_implementation(self):
        """Test implementing a custom source finder."""
        class TestSourceFinder(PySourceContentReferenceFinderBase):
            def __init__(self, api_url):
                self.api_url = api_url
                super().__init__()
            
            def find_by_source_location(self, source_location):
                if source_location and source_location.path == "source/path":
                    return "source-reference"  # Return a simple test value
                return None
            
            def find_by_id(self, id):
                if id == UUID("11111111-2222-3333-4444-555555555555"):
                    return "source-id-reference"
                return None
            
            def find_all(self):
                return ["source-ref1", "source-ref2"]
        
        # Test instantiation
        finder = TestSourceFinder("https://api.test.com")
        assert isinstance(finder, PySourceContentReferenceFinderBase)
        
        # Test methods work
        test_location = PyContentLocation(ContentLocation(["source", "path"]))
        result = finder.find_by_source_location(test_location)
        assert result == "source-reference"
        
        # Test find_all returns list
        all_results = finder.find_all()
        assert isinstance(all_results, list)
        assert len(all_results) == 2
        assert all_results == ["source-ref1", "source-ref2"]