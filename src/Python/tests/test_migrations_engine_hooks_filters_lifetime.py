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

# Make sure the test can find the module
import pytest
from tableau_migration.migration import get_service
from tableau_migration.migration_engine_hooks_filters import PyContentFilterBuilder
from System import IServiceProvider, Func
from Microsoft.Extensions.DependencyInjection import (
    ServiceProviderServiceExtensions,
    ServiceCollectionServiceExtensions,
    ServiceCollection,
    ServiceCollectionContainerBuilderExtensions)
from System.Collections.Generic import IEnumerable
from Tableau.Migration.Content import IUser,IProject
from Tableau.Migration.Engine import ContentMigrationItem
from Tableau.Migration.Engine.Hooks import IMigrationHook
from Tableau.Migration.Engine.Hooks.Filters import ContentFilterBuilder,IContentFilter
from Tableau.Migration.Interop.Hooks.Filters import ISyncContentFilter

class FilterImplementation(ISyncContentFilter[IUser]):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Filters.Lifetime"
    
    def Execute(self, ctx: IEnumerable[ContentMigrationItem[IUser]]) -> IEnumerable[ContentMigrationItem[IUser]]:
        return ctx

class SubFilterImplementation(FilterImplementation):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Filters.Lifetime"
    
    def Execute(self, ctx: IEnumerable[ContentMigrationItem[IUser]]) -> IEnumerable[ContentMigrationItem[IUser]]:
        return ctx

class TestContentFilterBuilderLifetimeTests():
    @pytest.fixture(autouse=True, scope="class")
    def setup(self):
        TestContentFilterBuilderLifetimeTests.FilterImplementation = FilterImplementation
        TestContentFilterBuilderLifetimeTests.SubFilterImplementation = SubFilterImplementation
            
        yield
        
        del TestContentFilterBuilderLifetimeTests.SubFilterImplementation
        del TestContentFilterBuilderLifetimeTests.FilterImplementation
        
    def test_lifetime_buildandcreate_object(self, skip_by_python_lifetime_env_var):
        try:
            # Arrange
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
            hook = TestContentFilterBuilderLifetimeTests.SubFilterImplementation()
            hookFactory = PyContentFilterBuilder(ContentFilterBuilder()).add(IUser,hook).build().get_hooks(IContentFilter[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentFilter[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert hook == firstScopeHook1
            assert hook is not firstScopeHook1
            assert hook == firstScopeHook2
            assert hook is not firstScopeHook2
            assert hook == lastScopeHook
            assert hook is not lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_noinitializersingleton(self, skip_by_python_lifetime_env_var):
        try:
            # Arrange
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddSingleton[TestContentFilterBuilderLifetimeTests.SubFilterImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyContentFilterBuilder(ContentFilterBuilder()).add(TestContentFilterBuilderLifetimeTests.SubFilterImplementation,IUser).build().get_hooks(IContentFilter[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentFilter[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 == firstScopeHook2
            assert firstScopeHook1 is not firstScopeHook2
            assert firstScopeHook1 == lastScopeHook
            assert firstScopeHook1 is not lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_noinitializerscoped(self, skip_by_python_lifetime_env_var):
        try:
            # Arrange
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddScoped[TestContentFilterBuilderLifetimeTests.SubFilterImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyContentFilterBuilder(ContentFilterBuilder()).add(TestContentFilterBuilderLifetimeTests.SubFilterImplementation,IUser).build().get_hooks(IContentFilter[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentFilter[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 == firstScopeHook2
            assert firstScopeHook1 is not firstScopeHook2
            assert firstScopeHook1 != lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_noinitializertransient(self, skip_by_python_lifetime_env_var):
        try:
            # Arrange
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddTransient[TestContentFilterBuilderLifetimeTests.SubFilterImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyContentFilterBuilder(ContentFilterBuilder()).add(TestContentFilterBuilderLifetimeTests.SubFilterImplementation,IUser).build().get_hooks(IContentFilter[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentFilter[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 != firstScopeHook2
            assert firstScopeHook1 != lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_initializerobjectreference(self, skip_by_python_lifetime_env_var):
        try:
            # Arrange
            hook = TestContentFilterBuilderLifetimeTests.SubFilterImplementation()
            def subsubclassinitialize(provider: IServiceProvider):
                return hook
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
            hookFactory = PyContentFilterBuilder(ContentFilterBuilder()).add(TestContentFilterBuilderLifetimeTests.SubFilterImplementation, IUser, Func[IServiceProvider, TestContentFilterBuilderLifetimeTests.SubFilterImplementation](subsubclassinitialize)).build().get_hooks(IContentFilter[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentFilter[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 == firstScopeHook2
            assert firstScopeHook1 is not firstScopeHook2
            assert firstScopeHook1 == lastScopeHook
            assert firstScopeHook1 is not lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_initializernewobject(self, skip_by_python_lifetime_env_var):
        try:
            # Arrange
            def subsubclassinitialize(provider: IServiceProvider):
                return TestContentFilterBuilderLifetimeTests.SubFilterImplementation()
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
            hookFactory = PyContentFilterBuilder(ContentFilterBuilder()).add(TestContentFilterBuilderLifetimeTests.SubFilterImplementation, IUser, Func[IServiceProvider, TestContentFilterBuilderLifetimeTests.SubFilterImplementation](subsubclassinitialize)).build().get_hooks(IContentFilter[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentFilter[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 != firstScopeHook2
            assert firstScopeHook1 != lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_initializersingleton(self, skip_by_python_lifetime_env_var):
        try:
            # Arrange
            def subsubclassinitialize(provider: IServiceProvider):
                return get_service(provider,TestContentFilterBuilderLifetimeTests.SubFilterImplementation)
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddSingleton[TestContentFilterBuilderLifetimeTests.SubFilterImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyContentFilterBuilder(ContentFilterBuilder()).add(TestContentFilterBuilderLifetimeTests.SubFilterImplementation, IUser, Func[IServiceProvider, TestContentFilterBuilderLifetimeTests.SubFilterImplementation](subsubclassinitialize)).build().get_hooks(IContentFilter[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentFilter[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 == firstScopeHook2
            assert firstScopeHook1 is not firstScopeHook2
            assert firstScopeHook1 == lastScopeHook
            assert firstScopeHook1 is not lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_initializerscoped(self, skip_by_python_lifetime_env_var):
        try:
            # Arrange
            def classinitialize(provider: IServiceProvider):
                return get_service(provider,TestContentFilterBuilderLifetimeTests.FilterImplementation)
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddScoped[TestContentFilterBuilderLifetimeTests.FilterImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyContentFilterBuilder(ContentFilterBuilder()).add(TestContentFilterBuilderLifetimeTests.FilterImplementation, IUser, Func[IServiceProvider, TestContentFilterBuilderLifetimeTests.FilterImplementation](classinitialize)).build().get_hooks(IContentFilter[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentFilter[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 == firstScopeHook2
            assert firstScopeHook1 is not firstScopeHook2
            assert firstScopeHook1 != lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_initializertransient(self, skip_by_python_lifetime_env_var):
        try:
            # Arrange
            def subclassinitialize(provider: IServiceProvider):
                return get_service(provider,TestContentFilterBuilderLifetimeTests.SubFilterImplementation)
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddTransient[TestContentFilterBuilderLifetimeTests.SubFilterImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyContentFilterBuilder(ContentFilterBuilder()).add(TestContentFilterBuilderLifetimeTests.SubFilterImplementation, IUser, Func[IServiceProvider, TestContentFilterBuilderLifetimeTests.SubFilterImplementation](subclassinitialize)).build().get_hooks(IContentFilter[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentFilter[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentFilter[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 != firstScopeHook2
            assert firstScopeHook1 != lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_callback(self, skip_by_python_lifetime_env_var):
        try:
            # Arrange
            def classcallback(context: IEnumerable[ContentMigrationItem[IProject]]):
                return context
        
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
            hookFactory = PyContentFilterBuilder(ContentFilterBuilder()).add(IProject, Func[IEnumerable[ContentMigrationItem[IProject]], IEnumerable[ContentMigrationItem[IProject]]](classcallback)).build().get_hooks(IContentFilter[IProject])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IMigrationHook[IEnumerable[ContentMigrationItem[IProject]]]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IMigrationHook[IEnumerable[ContentMigrationItem[IProject]]]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IMigrationHook[IEnumerable[ContentMigrationItem[IProject]]]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 != firstScopeHook2
            assert firstScopeHook1 != lastScopeHook
        finally:
            provider.Dispose()
