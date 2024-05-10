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

# Make sure the test can find the module
import pytest
from tableau_migration.migration import get_service
from tableau_migration.migration_engine_hooks import PyMigrationHookBuilder
from System import IServiceProvider, Func
from Microsoft.Extensions.DependencyInjection import (
    ServiceProviderServiceExtensions,
    ServiceCollectionServiceExtensions,
    ServiceCollection,
    ServiceCollectionContainerBuilderExtensions)
from Tableau.Migration.Engine.Hooks import MigrationHookBuilder, IMigrationHook
from Tableau.Migration.Interop.Hooks import ISyncMigrationHook

class HookImplementation(ISyncMigrationHook[bool]):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Lifetime"
    
    def Execute(self, ctx) -> bool:
        return ctx

class SubHookImplementation(HookImplementation):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Lifetime"

    def Execute(self, ctx) -> bool:
        return ctx

class SubSubHookImplementation(SubHookImplementation):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Lifetime"
    
    def Execute(self, ctx) -> bool:
        return ctx

class TestMigrationHookBuilderLifetimeTests():
    @pytest.fixture(autouse=True, scope="class")
    def setup(self):
        TestMigrationHookBuilderLifetimeTests.HookImplementation = HookImplementation
        TestMigrationHookBuilderLifetimeTests.SubHookImplementation = SubHookImplementation
        TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation = SubSubHookImplementation
            
        yield
        
        del TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation
        del TestMigrationHookBuilderLifetimeTests.SubHookImplementation
        del TestMigrationHookBuilderLifetimeTests.HookImplementation
        
    def test_lifetime_buildandcreate_object(self, skip_by_python_lifetime_env_var):
        try:
            # Arrange
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
            hook = TestMigrationHookBuilderLifetimeTests.SubHookImplementation()
            hookFactory = PyMigrationHookBuilder(MigrationHookBuilder()).add(hook).build().get_hooks(ISyncMigrationHook[bool])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IMigrationHook[bool]](scope2.ServiceProvider)
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
            ServiceCollectionServiceExtensions.AddSingleton[TestMigrationHookBuilderLifetimeTests.SubHookImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyMigrationHookBuilder(MigrationHookBuilder()).add(TestMigrationHookBuilderLifetimeTests.SubHookImplementation).build().get_hooks(ISyncMigrationHook[bool])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IMigrationHook[bool]](scope2.ServiceProvider)
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
            ServiceCollectionServiceExtensions.AddScoped[TestMigrationHookBuilderLifetimeTests.SubHookImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyMigrationHookBuilder(MigrationHookBuilder()).add(TestMigrationHookBuilderLifetimeTests.SubHookImplementation).build().get_hooks(ISyncMigrationHook[bool])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IMigrationHook[bool]](scope2.ServiceProvider)
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
            ServiceCollectionServiceExtensions.AddTransient[TestMigrationHookBuilderLifetimeTests.SubHookImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyMigrationHookBuilder(MigrationHookBuilder()).add(TestMigrationHookBuilderLifetimeTests.SubHookImplementation).build().get_hooks(ISyncMigrationHook[bool])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IMigrationHook[bool]](scope2.ServiceProvider)
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
            hook = TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation()
            def subsubclassinitialize(provider: IServiceProvider):
                return hook
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
            hookFactory = PyMigrationHookBuilder(MigrationHookBuilder()).add(TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation, Func[IServiceProvider, TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation](subsubclassinitialize)).build().get_hooks(ISyncMigrationHook[bool])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IMigrationHook[bool]](scope2.ServiceProvider)
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
                return TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation()
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
            hookFactory = PyMigrationHookBuilder(MigrationHookBuilder()).add(TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation, Func[IServiceProvider, TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation](subsubclassinitialize)).build().get_hooks(ISyncMigrationHook[bool])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IMigrationHook[bool]](scope2.ServiceProvider)
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
                return get_service(provider,TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation)
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddSingleton[TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyMigrationHookBuilder(MigrationHookBuilder()).add(TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation, Func[IServiceProvider, TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation](subsubclassinitialize)).build().get_hooks(ISyncMigrationHook[bool])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IMigrationHook[bool]](scope2.ServiceProvider)
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
            def subsubclassinitialize(provider: IServiceProvider):
                return get_service(provider,TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation)
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddScoped[TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyMigrationHookBuilder(MigrationHookBuilder()).add(TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation, Func[IServiceProvider, TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation](subsubclassinitialize)).build().get_hooks(ISyncMigrationHook[bool])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IMigrationHook[bool]](scope2.ServiceProvider)
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
            def subsubclassinitialize(provider: IServiceProvider):
                return get_service(provider,TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation)
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddTransient[TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyMigrationHookBuilder(MigrationHookBuilder()).add(TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation, Func[IServiceProvider, TestMigrationHookBuilderLifetimeTests.SubSubHookImplementation](subsubclassinitialize)).build().get_hooks(ISyncMigrationHook[bool])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IMigrationHook[bool]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IMigrationHook[bool]](scope2.ServiceProvider)
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
            def classcallback(context: str):
                return context
        
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
            hookFactory = PyMigrationHookBuilder(MigrationHookBuilder()).add(ISyncMigrationHook[str], str, Func[str, str](classcallback)).build().get_hooks(ISyncMigrationHook[str])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IMigrationHook[str]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IMigrationHook[str]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IMigrationHook[str]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 != firstScopeHook2
            assert firstScopeHook1 != lastScopeHook
        finally:
            provider.Dispose()
