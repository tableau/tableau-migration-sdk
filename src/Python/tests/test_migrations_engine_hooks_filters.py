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
from tableau_migration.migration import get_service
from tableau_migration.migration_engine_hooks_filters import PyContentFilterBuilder
from System import IServiceProvider, Func, ArgumentException, NotImplementedException
from System.Collections.Generic import IEnumerable, List
from System.Threading import CancellationToken
from Microsoft.Extensions.DependencyInjection import (
    ServiceCollection,
    ServiceCollectionContainerBuilderExtensions)
from Tableau.Migration.Content import IUser,IProject
from Tableau.Migration.Engine import ContentMigrationItem
from Tableau.Migration.Engine.Hooks import IMigrationHook
from Tableau.Migration.Engine.Hooks.Filters import ContentFilterBuilder,IContentFilter,ContentFilterBase
from Tableau.Migration.Interop.Hooks.Filters import ISyncContentFilter

class ClassImplementation(ISyncContentFilter[IUser]):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Filters"
    
    def Execute(self, ctx: IEnumerable[ContentMigrationItem[IUser]]) -> IEnumerable[ContentMigrationItem[IUser]]:
        return ctx


class SubClassImplementation(ClassImplementation):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Filters"
    
    def Execute(self, ctx: IEnumerable[ContentMigrationItem[IUser]]) -> IEnumerable[ContentMigrationItem[IUser]]:
        print("In Execute of SubClassImplementation(ClassImplementation)" )
        return ctx


class RawHook():
    __namespace__ = "Tableau.Migration.Custom.Hooks.Filters"
    
    def ShouldMigrate(self, ctx) -> bool:
        return True


class WithoutImplementation(ISyncContentFilter[IUser]):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Filters"


class TestContentFilterBuilderTests():
    def test_clear_class_hook(self):
        hook_builder = PyContentFilterBuilder(ContentFilterBuilder())
        
        result = hook_builder.add(IUser,ClassImplementation()).clear()
        
        assert result is hook_builder
        assert len(result.build().get_hooks(ISyncContentFilter[IUser])) == 0
        
    def test_clear_subclass_hook(self):
        hook_builder = PyContentFilterBuilder(ContentFilterBuilder())
        
        result = hook_builder.add(IUser,SubClassImplementation()).clear()
        
        assert result is hook_builder
        assert len(result.build().get_hooks(ISyncContentFilter[IUser])) == 0

    def test_clear_raw_hook(self):
        try:
            hook_builder = PyContentFilterBuilder(ContentFilterBuilder())
        
            result = hook_builder.add(IProject,RawHook()).clear()
        
            assert result is hook_builder
            assert len(result.build().get_hooks(ISyncContentFilter[IUser])) == 0
        except Exception as e:
            print(e);

        
    def test_add_object(self):
        hook_builder = PyContentFilterBuilder(ContentFilterBuilder())
        
        result = hook_builder.add(IUser,SubClassImplementation())
        
        assert result is hook_builder
        
    def test_add_type_noinitializer(self):
        hook_builder = PyContentFilterBuilder(ContentFilterBuilder())
        
        result = hook_builder.add(ClassImplementation,IUser)
        
        assert result is hook_builder
        
    def test_add_type_initializer(self):
        hook_builder = PyContentFilterBuilder(ContentFilterBuilder())
        
        def subclassinitialize(provider: IServiceProvider):
            return get_service(provider, SubClassImplementation)
        
        result = hook_builder.add(SubClassImplementation, IUser, Func[IServiceProvider, SubClassImplementation](subclassinitialize))
        
        assert result is hook_builder
        
    def test_add_callback(self):
        hook_builder = PyContentFilterBuilder(ContentFilterBuilder())
        
        def classcallback(context: IEnumerable[ContentMigrationItem[IProject]]):
            return context
        
        result = hook_builder.add(IProject, Func[IEnumerable[ContentMigrationItem[IProject]], IEnumerable[ContentMigrationItem[IProject]]](classcallback))
        
        assert result is hook_builder
        
    def test_add_withoutimplementation(self):
        users = List[ContentMigrationItem[IUser]]();
        provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
        hookFactory = PyContentFilterBuilder(ContentFilterBuilder()).add(IUser, WithoutImplementation()).build().get_hooks(IContentFilter[IUser])
        assert len(hookFactory) == 1
        hook = hookFactory[0].Create[IMigrationHook[IEnumerable[ContentMigrationItem[IUser]]]](provider)
        try:
            hook.ExecuteAsync(users, CancellationToken(False))
        except NotImplementedException as nie:
            assert nie.args[0] == "Python object does not have a 'Execute' method"
        else:
            assert False, "This test must generate a NotImplementedException."
        finally:
            provider.Dispose()
        
    def test_add_withimplementation(self):
        users = List[ContentMigrationItem[IUser]]();
        provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
        hookFactory = PyContentFilterBuilder(ContentFilterBuilder()).add(IUser, SubClassImplementation()).build().get_hooks(IContentFilter[IUser])
        assert len(hookFactory) == 1
        hook = hookFactory[0].Create[IMigrationHook[IEnumerable[ContentMigrationItem[IUser]]]](provider)
        try:
            result = hook.ExecuteAsync(users, CancellationToken(False))
            print(result)
        except:
            assert False, "This test must not generate an Exception."
        finally:
            provider.Dispose()
        
    def test_build_detects_interface_from_concrete_class(self):
        collection = PyContentFilterBuilder(ContentFilterBuilder()).add(IUser,ClassImplementation()).build()
        hooks = collection.get_hooks(IContentFilter[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[IEnumerable[ContentMigrationItem[IUser]]])
        otherhooks = collection.get_hooks(IContentFilter[IProject])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)
        
        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        
    def test_build_detects_interface_from_concrete_subclass(self):
        collection = PyContentFilterBuilder(ContentFilterBuilder()).add(IUser,SubClassImplementation()).build()
        hooks = collection.get_hooks(IContentFilter[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[IEnumerable[ContentMigrationItem[IUser]]])
        otherhooks = collection.get_hooks(IContentFilter[IProject])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        
    def test_build_factory_class(self):
        collection = PyContentFilterBuilder(ContentFilterBuilder()).add(ClassImplementation,IUser).build()
        hooks = collection.get_hooks(IContentFilter[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[IEnumerable[ContentMigrationItem[IUser]]])
        otherhooks = collection.get_hooks(IContentFilter[IProject])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        
    def test_build_factory_subclass_withinitializer(self):
        def subclassinitialize(provider: IServiceProvider):
            return get_service(provider, SubClassImplementation)
        
        collection = PyContentFilterBuilder(ContentFilterBuilder()).add(SubClassImplementation, IUser, Func[IServiceProvider, SubClassImplementation](subclassinitialize)).build()
        hooks = collection.get_hooks(IContentFilter[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[IEnumerable[ContentMigrationItem[IUser]]])
        otherhooks = collection.get_hooks(IContentFilter[IProject])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        
    def test_build_factory_multipleclasses(self):
        collection = PyContentFilterBuilder(ContentFilterBuilder()).add(ClassImplementation,IUser).add(SubClassImplementation,IUser).build()
        hooks = collection.get_hooks(IContentFilter[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[IEnumerable[ContentMigrationItem[IUser]]])
        otherhooks = collection.get_hooks(IContentFilter[IProject])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 2
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        
    def test_build_callback(self):
        def classcallback(context: IEnumerable[ContentMigrationItem[IProject]]):
            return context
        
        collection = PyContentFilterBuilder(ContentFilterBuilder()).add(IProject, Func[IEnumerable[ContentMigrationItem[IProject]], IEnumerable[ContentMigrationItem[IProject]]](classcallback)).build()
        hooks = collection.get_hooks(IContentFilter[IProject])
        internalhooks = collection.get_hooks(IMigrationHook[IEnumerable[ContentMigrationItem[IProject]]])
        otherhooks = collection.get_hooks(IContentFilter[IUser])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
