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
from tableau_migration.migration import get_service
from tableau_migration.migration_engine_hooks import PyMigrationHookBuilder
from System import IServiceProvider, Func, ArgumentException, NotImplementedException
from System.Threading import CancellationToken
from Microsoft.Extensions.DependencyInjection import (
    ServiceCollection,
    ServiceCollectionContainerBuilderExtensions)
from Tableau.Migration.Interop.Hooks import ISyncMigrationHook
from Tableau.Migration.Engine.Hooks import MigrationHookBuilder, IMigrationHook

class ClassImplementation(ISyncMigrationHook[bool]):
    __namespace__ = "Tableau.Migration.Custom.Hooks"
    
    def Execute(self, ctx) -> bool:
        return not ctx

class SubClassImplementation(ClassImplementation):
    __namespace__ = "Tableau.Migration.Custom.Hooks"
    
    def Execute(self, ctx) -> bool:
        return not ctx

class SubSubClassImplementation(SubClassImplementation):
    __namespace__ = "Tableau.Migration.Custom.Hooks"
    
    def Execute(self, ctx) -> bool:
        return not ctx

class RawHook(IMigrationHook[str]):
    __namespace__ = "Tableau.Migration.Custom.Hooks"
    
    def Execute(self, ctx) -> str:
        return ctx

class WithoutImplementation(ISyncMigrationHook[bool]):
    __namespace__ = "Tableau.Migration.Custom.Hooks"

class TestMigrationHookBuilderTests():
    def test_clear_class_hook(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())
        
        result = hook_builder.add(ClassImplementation()).clear()
        
        assert result is hook_builder
        assert len(result.build().get_hooks(ISyncMigrationHook[bool])) == 0
        
    def test_clear_subclass_hook(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())
        
        result = hook_builder.add(SubClassImplementation()).clear()
        
        assert result is hook_builder
        assert len(result.build().get_hooks(ISyncMigrationHook[int])) == 0
        
    def test_clear_subsubclass_hook(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())
        
        result = hook_builder.add(SubSubClassImplementation()).clear()
        
        assert result is hook_builder
        assert len(result.build().get_hooks(ISyncMigrationHook[int])) == 0
        
    def test_add_object(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())
        
        result = hook_builder.add(SubSubClassImplementation())
        
        assert result is hook_builder
        
    def test_add_type_noinitializer(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())
        
        result = hook_builder.add(SubSubClassImplementation)
        
        assert result is hook_builder
        
    def test_add_type_initializer(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())
        
        def subclassinitialize(provider: IServiceProvider):
            return get_service(provider, SubClassImplementation)
        
        result = hook_builder.add(SubClassImplementation, Func[IServiceProvider, SubClassImplementation](subclassinitialize))
        
        assert result is hook_builder
        
    def test_add_callback(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())
        
        def classcallback(context: str):
            return context
        
        result = hook_builder.add(ISyncMigrationHook[str], str, Func[str, str](classcallback))
        
        assert result is hook_builder
        
    def test_add_ignores_raw_hook_interface(self):
        try:
            PyMigrationHookBuilder(MigrationHookBuilder()).add(RawHook())
        except ArgumentException as ae:
            assert ae.args[0] == "Type Tableau.Migration.Custom.Hooks.RawHook does not implement any migration hook types."
        else:
            assert False, "This test must generate an ArgumentException."
        
    def test_add_withoutimplementation(self):
        provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
        hookFactory = PyMigrationHookBuilder(MigrationHookBuilder()).add(WithoutImplementation()).build().get_hooks(ISyncMigrationHook[bool])
        assert len(hookFactory) == 1
        hook = hookFactory[0].Create[IMigrationHook[bool]](provider)
        try:
            hook.ExecuteAsync(False, CancellationToken(False))
        except NotImplementedException as nie:
            assert nie.args[0] == "Python object does not have a 'Execute' method"
        else:
            assert False, "This test must generate an NotImplementedException."
        finally:
            provider.Dispose()
        
    def test_add_withimplementation(self):
        provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
        hookFactory = PyMigrationHookBuilder(MigrationHookBuilder()).add(SubClassImplementation()).build().get_hooks(ISyncMigrationHook[bool])
        assert len(hookFactory) == 1
        hook = hookFactory[0].Create[IMigrationHook[bool]](provider)
        try:
            result = hook.ExecuteAsync(False, CancellationToken(False))
            
            assert result
        except:
            assert False, "This test must not generate an Exception."
        finally:
            provider.Dispose()
        
    def test_build_detects_interface_from_concrete_class(self):
        collection = PyMigrationHookBuilder(MigrationHookBuilder()).add(ClassImplementation()).build()
        hooks = collection.get_hooks(ISyncMigrationHook[bool])
        internalhooks = collection.get_hooks(IMigrationHook[bool])
        otherhooks = collection.get_hooks(ISyncMigrationHook[int])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)
        subsubclasshooks = collection.get_hooks(SubSubClassImplementation)
        
        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        assert len(subsubclasshooks) == 0
        
    def test_build_detects_interface_from_concrete_subclass(self):
        collection = PyMigrationHookBuilder(MigrationHookBuilder()).add(SubClassImplementation()).build()
        hooks = collection.get_hooks(ISyncMigrationHook[bool])
        internalhooks = collection.get_hooks(IMigrationHook[bool])
        otherhooks = collection.get_hooks(ISyncMigrationHook[int])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)
        subsubclasshooks = collection.get_hooks(SubSubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        assert len(subsubclasshooks) == 0
        
    def test_build_detects_interface_from_concrete_subsubclass(self):
        collection = PyMigrationHookBuilder(MigrationHookBuilder()).add(SubSubClassImplementation()).build()
        hooks = collection.get_hooks(ISyncMigrationHook[bool])
        internalhooks = collection.get_hooks(IMigrationHook[bool])
        otherhooks = collection.get_hooks(ISyncMigrationHook[int])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)
        subsubclasshooks = collection.get_hooks(SubSubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        assert len(subsubclasshooks) == 0
        
    def test_build_factory_class(self):
        collection = PyMigrationHookBuilder(MigrationHookBuilder()).add(ClassImplementation).build()
        hooks = collection.get_hooks(ISyncMigrationHook[bool])
        internalhooks = collection.get_hooks(IMigrationHook[bool])
        otherhooks = collection.get_hooks(ISyncMigrationHook[int])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)
        subsubclasshooks = collection.get_hooks(SubSubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        assert len(subsubclasshooks) == 0
        
    def test_build_factory_subclass_withinitializer(self):
        def subclassinitialize(provider: IServiceProvider):
            return get_service(provider, SubClassImplementation)
        
        collection = PyMigrationHookBuilder(MigrationHookBuilder()).add(SubClassImplementation, Func[IServiceProvider, SubClassImplementation](subclassinitialize)).build()
        hooks = collection.get_hooks(ISyncMigrationHook[bool])
        internalhooks = collection.get_hooks(IMigrationHook[bool])
        otherhooks = collection.get_hooks(ISyncMigrationHook[int])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)
        subsubclasshooks = collection.get_hooks(SubSubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        assert len(subsubclasshooks) == 0
        
    def test_build_factory_multipleclasses(self):
        collection = PyMigrationHookBuilder(MigrationHookBuilder()).add(ClassImplementation).add(SubClassImplementation).add(SubSubClassImplementation).build()
        hooks = collection.get_hooks(ISyncMigrationHook[bool])
        internalhooks = collection.get_hooks(IMigrationHook[bool])
        otherhooks = collection.get_hooks(ISyncMigrationHook[int])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)
        subsubclasshooks = collection.get_hooks(SubSubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 3
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        assert len(subsubclasshooks) == 0
        
    def test_build_callback(self):
        def classcallback(context: str):
            return context
        
        collection = PyMigrationHookBuilder(MigrationHookBuilder()).add(ISyncMigrationHook[str], str, Func[str, str](classcallback)).build()
        hooks = collection.get_hooks(ISyncMigrationHook[str])
        internalhooks = collection.get_hooks(IMigrationHook[str])
        otherhooks = collection.get_hooks(ISyncMigrationHook[int])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)
        subsubclasshooks = collection.get_hooks(SubSubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        assert len(subsubclasshooks) == 0
