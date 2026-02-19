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

from typing import Callable, TypeVar

from tableau_migration import (
    MigrationPlanBuilder,
    IWorkbook as PyWorkbook,
    IDataSource as PyDataSource
)

from tableau_migration.migration import get_service_provider
from tableau_migration.migration_content import PyWorkbook
from tableau_migration.migration_engine_services import (
    PyMigrationContentLoaderBase,
    PyMigrationServiceBuilder,
    PyMigrationServiceFactory,
    PyMigrationServiceFactoryCollection,
    PyMigrationServiceFactoryContext
)
from tableau_migration.migration_paging import empty_pager, memory_pager

from Tableau.Migration.Content import (
    IWorkbook
)

from Tableau.Migration.Engine.Endpoints import (
    IMigrationContentLoader
)

from Tableau.Migration.Engine.Services import (
    IMigrationServiceBuilderFactory,
    MigrationServiceFactoryContext
)

from tests.helpers.autofixture import AutoFixtureTestBase


class StubUnwrappedWorkbookLoader(IMigrationContentLoader[IWorkbook]):
    __namespace__ = "Tableau.Migration.Python"

    def GetMigrationContentPager(self, page_size: int):
        return None

def create_test_loader(ctx: PyMigrationServiceFactoryContext):
    return StubUnwrappedWorkbookLoader()

def track_factory_calls(calls: list[PyMigrationServiceFactoryContext], factory: PyMigrationServiceFactory) -> PyMigrationServiceFactory:
    def _tracked_factory(ctx: PyMigrationServiceFactoryContext):
        calls.append(ctx)
        return factory(ctx)

    return _tracked_factory
    

TContent = TypeVar("TContent")


class TestGenericLoader(PyMigrationContentLoaderBase[TContent]):
    def get_migration_content_pager(self, page_size: int):
        return None


class TestDataSourceLoader(PyMigrationContentLoaderBase[PyDataSource]):
    def get_migration_content_pager(self, page_size: int):
        return None

class TestPythonMigrationServices(AutoFixtureTestBase):

    def test_all_plan_services_wrapped(self):
        services = get_service_provider()
        plan_builder = MigrationPlanBuilder()
        builder = PyMigrationServiceBuilder(plan_builder._plan_builder.Services)

        missing_wrappers = [s.dotnet.Name for s in builder.supported_services if s.service is None]
        if missing_wrappers:
            assert not f"Missing plan-level service wrappers for {missing_wrappers}"


    def test_all_endpoint_services_wrapped(self):
        services = get_service_provider()
        plan_builder = MigrationPlanBuilder()
        builder = PyMigrationServiceBuilder(plan_builder._plan_builder.Source.Services)

        missing_wrappers = [s.dotnet.Name for s in builder.supported_services if s.service is None]
        if missing_wrappers:
            assert not f"Missing endpoint-level service wrappers for {missing_wrappers}"

class TestPyMigrationServiceFactoryContext(AutoFixtureTestBase):

    def test_services(self):
        dotnet_ctx = self.create(MigrationServiceFactoryContext)
        ctx = PyMigrationServiceFactoryContext(dotnet_ctx)

        services = ctx.services


class TestPyMigrationServiceBuilder(AutoFixtureTestBase):
    

    def test_manual_factory_function(self):
        services = get_service_provider()
        plan_builder = MigrationPlanBuilder()
        builder = PyMigrationServiceBuilder(plan_builder._plan_builder.Services)

        calls = []
        builder.set(PyMigrationContentLoaderBase[PyWorkbook], track_factory_calls(calls, create_test_loader))
        loader = builder.get_service(PyMigrationContentLoaderBase[PyWorkbook], services)
        pager = loader.GetMigrationContentPager(10)

        assert loader is not None
        assert len(calls) == 1


    def test_generic_wrapper_open_generic(self):
        services = get_service_provider()
        plan_builder = MigrationPlanBuilder()
        builder = PyMigrationServiceBuilder(plan_builder._plan_builder.Services)

        builder.set(PyMigrationContentLoaderBase, TestGenericLoader)
        loader = builder.get_service(PyMigrationContentLoaderBase[PyWorkbook], services)
        pager = loader.GetMigrationContentPager(10)

        assert loader is not None


    def test_specific_generic_override(self):
        services = get_service_provider()
        plan_builder = MigrationPlanBuilder()
        builder = PyMigrationServiceBuilder(plan_builder._plan_builder.Services)

        calls = []
        builder.set(PyMigrationContentLoaderBase, track_factory_calls(calls, create_test_loader))
        builder.set(PyMigrationContentLoaderBase[PyWorkbook], TestGenericLoader)

        loader = builder.get_service(PyMigrationContentLoaderBase[PyWorkbook], services)
        pager = loader.GetMigrationContentPager(10)

        assert loader is not None
        assert len(calls) == 0


    def test_non_generic_wrapper(self):
        services = get_service_provider()
        plan_builder = MigrationPlanBuilder()
        builder = PyMigrationServiceBuilder(plan_builder._plan_builder.Services)

        builder.set(PyMigrationContentLoaderBase[PyDataSource], TestDataSourceLoader)
        loader = builder.get_service(PyMigrationContentLoaderBase[PyDataSource], services)
        pager = loader.GetMigrationContentPager(10)

        assert loader is not None


class TestPyMigrationContentLoaderBase(AutoFixtureTestBase):
    """Tests for the new PyMigrationContentLoaderBase implementation."""

    def test_custom_loader_implementation(self):
        """Test that users can create custom content loaders by inheriting from PyMigrationContentLoaderBase."""
        
        class CustomWorkbookLoader(PyMigrationContentLoaderBase[PyWorkbook]):
            def __init__(self, custom_data):
                self._custom_data = custom_data
            
            def get_migration_content_pager(self, page_size: int):
                return memory_pager(PyWorkbook, self._custom_data, page_size)
        
        # Test that the custom loader can be instantiated
        custom_data = []  # Empty list for testing
        loader = CustomWorkbookLoader(custom_data)
        
        # Test that it returns a pager (Python pager when called directly)
        pager = loader.get_migration_content_pager(10)
        assert pager is not None
        # When called directly, we get the wrapper pager object
        assert hasattr(pager, 'NextPageAsync')

    def test_loader_with_empty_data(self):
        """Test content loader with empty data."""
        
        class EmptyLoader(PyMigrationContentLoaderBase[PyWorkbook]):
            def get_migration_content_pager(self, page_size: int):
                return empty_pager(PyWorkbook)

        
        loader = EmptyLoader()
        pager = loader.get_migration_content_pager(5)
        
        assert pager is not None
        # When called directly, we get the wrapped pager object
        assert hasattr(pager, 'NextPageAsync')

    def test_loader_with_different_content_types(self):
        """Test content loader with different content types."""
        
        class DataSourceLoader(PyMigrationContentLoaderBase[PyDataSource]):
            def __init__(self, data_sources):
                self._data_sources = data_sources
            
            def get_migration_content_pager(self, page_size: int):
                return memory_pager(PyDataSource, self._data_sources, page_size)
        
        data_sources = []  # Empty list for testing
        loader = DataSourceLoader(data_sources)
        pager = loader.get_migration_content_pager(2)
        
        assert pager is not None
        # When called directly, we get the wrapped pager object
        assert hasattr(pager, 'NextPageAsync')

    def test_loader_wrapper_properties(self):
        """Test that the wrapper has correct properties for generic type handling."""
        from tableau_migration.migration_engine_services import _PyMigrationContentLoaderWrapperBuilder
        
        # Test that the wrapper correctly identifies content types
        wrapper = _PyMigrationContentLoaderWrapperBuilder(PyMigrationContentLoaderBase[PyWorkbook])
        
        # These would be set during actual instantiation
        assert wrapper.dotnet_service() is not None
        assert hasattr(wrapper, 'python_content_type')
        assert hasattr(wrapper, 'dotnet_content_type')
        assert hasattr(wrapper, 'get_wrapper_base_type')

    def test_loader_inheritance(self):
        """Test that the base class can be properly inherited."""
        
        class TestLoader(PyMigrationContentLoaderBase[PyWorkbook]):
            def get_migration_content_pager(self, page_size: int):
                return None
        
        loader = TestLoader()
        
        # Test that it has the correct wrapper
        assert hasattr(loader, '_wrapper_builder')
        assert loader._wrapper_builder is not None
        
        # Test that the method can be called
        result = loader.get_migration_content_pager(10)
        assert result is None

    def test_loader_with_service_registration(self):
        """Test that custom loaders can be registered with the service builder."""
        
        class CustomLoader(PyMigrationContentLoaderBase[PyWorkbook]):
            def get_migration_content_pager(self, page_size: int):
                return empty_pager(PyWorkbook)
        
        services = get_service_provider()
        plan_builder = MigrationPlanBuilder()
        builder = PyMigrationServiceBuilder(plan_builder._plan_builder.Services)
        
        # Register the custom loader
        builder.set(PyMigrationContentLoaderBase[PyWorkbook], CustomLoader)
        
        # Get the service
        loader = builder.get_service(PyMigrationContentLoaderBase[PyWorkbook], services)
        
        assert loader is not None
        # Note: The loader will be wrapped by the service system, so we can't check isinstance directly
        
        # Test that the service registration worked by checking the loader exists
        # We don't call GetMigrationContentPager since it would return None and cause issues