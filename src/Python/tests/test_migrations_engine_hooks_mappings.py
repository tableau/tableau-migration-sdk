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
from tableau_migration.migration_engine_hooks_mappings import PyContentMappingBuilder
from System import IServiceProvider, Func, ArgumentException
from Tableau.Migration.Content import IUser, IProject
from Tableau.Migration.Engine.Hooks import IMigrationHook
from Tableau.Migration.Engine.Hooks.Mappings import ContentMappingBuilder, IContentMapping, ContentMappingBase, ContentMappingContext
from Tableau.Migration.Interop.Hooks.Mappings import ISyncContentMapping

class ClassImplementation(ISyncContentMapping[IUser]):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Mappings"
    
    def Execute(self, ctx: ContentMappingContext[IUser]) -> ContentMappingContext[IUser]:
        return ctx

class SubClassImplementation(ClassImplementation):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Mappings"
    
    def Execute(self, ctx: ContentMappingContext[IUser]) -> ContentMappingContext[IUser]:
        return ctx

class RawHook(ContentMappingBase[IProject]):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Mappings"
    
    def ShouldMigrate(self, ctx) -> bool:
        return True

class TestContentMappingBuilderTests():
    def test_clear_class_hook(self):
        hook_builder = PyContentMappingBuilder(ContentMappingBuilder())
        
        result = hook_builder.add(IUser,ClassImplementation()).clear()
        
        assert result is hook_builder
        assert len(result.build().get_hooks(ISyncContentMapping[IUser])) == 0
        
    def test_clear_subclass_hook(self):
        hook_builder = PyContentMappingBuilder(ContentMappingBuilder())
        
        result = hook_builder.add(IUser,SubClassImplementation()).clear()
        
        assert result is hook_builder
        assert len(result.build().get_hooks(ISyncContentMapping[IUser])) == 0
        
    def test_add_object(self):
        hook_builder = PyContentMappingBuilder(ContentMappingBuilder())
        
        result = hook_builder.add(IUser,SubClassImplementation())
        
        assert result is hook_builder
        
    def test_add_type_noinitializer(self):
        hook_builder = PyContentMappingBuilder(ContentMappingBuilder())
        
        result = hook_builder.add(ClassImplementation,IUser)
        
        assert result is hook_builder
        
    def test_add_type_initializer(self):
        hook_builder = PyContentMappingBuilder(ContentMappingBuilder())
        
        def subclassinitialize(provider: IServiceProvider):
            return get_service(provider, SubClassImplementation)
        
        result = hook_builder.add(SubClassImplementation, IUser, Func[IServiceProvider, SubClassImplementation](subclassinitialize))
        
        assert result is hook_builder
        
    def test_add_callback(self):
        hook_builder = PyContentMappingBuilder(ContentMappingBuilder())
        
        def classcallback(context: ContentMappingContext[IProject]):
            return context
        
        result = hook_builder.add(IProject, Func[ContentMappingContext[IProject], ContentMappingContext[IProject]](classcallback))
        
        assert result is hook_builder
        
    def test_build_detects_interface_from_concrete_class(self):
        collection = PyContentMappingBuilder(ContentMappingBuilder()).add(IUser,ClassImplementation()).build()
        hooks = collection.get_hooks(IContentMapping[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[ContentMappingContext[IUser]])
        otherhooks = collection.get_hooks(IContentMapping[IProject])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)
        
        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        
    def test_build_detects_interface_from_concrete_subclass(self):
        collection = PyContentMappingBuilder(ContentMappingBuilder()).add(IUser,SubClassImplementation()).build()
        hooks = collection.get_hooks(IContentMapping[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[ContentMappingContext[IUser]])
        otherhooks = collection.get_hooks(IContentMapping[IProject])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        
    def test_build_factory_class(self):
        collection = PyContentMappingBuilder(ContentMappingBuilder()).add(ClassImplementation,IUser).build()
        hooks = collection.get_hooks(IContentMapping[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[ContentMappingContext[IUser]])
        otherhooks = collection.get_hooks(IContentMapping[IProject])
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
        
        collection = PyContentMappingBuilder(ContentMappingBuilder()).add(SubClassImplementation, IUser, Func[IServiceProvider, SubClassImplementation](subclassinitialize)).build()
        hooks = collection.get_hooks(IContentMapping[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[ContentMappingContext[IUser]])
        otherhooks = collection.get_hooks(IContentMapping[IProject])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        
    def test_build_factory_multipleclasses(self):
        collection = PyContentMappingBuilder(ContentMappingBuilder()).add(ClassImplementation,IUser).add(SubClassImplementation,IUser).build()
        hooks = collection.get_hooks(IContentMapping[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[ContentMappingContext[IUser]])
        otherhooks = collection.get_hooks(IContentMapping[IProject])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 2
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        
    def test_build_callback(self):
        def classcallback(context: ContentMappingContext[IProject]):
            return context
        
        collection = PyContentMappingBuilder(ContentMappingBuilder()).add(IProject, Func[ContentMappingContext[IProject], ContentMappingContext[IProject]](classcallback)).build()
        hooks = collection.get_hooks(IContentMapping[IProject])
        internalhooks = collection.get_hooks(IMigrationHook[ContentMappingContext[IProject]])
        otherhooks = collection.get_hooks(IContentMapping[IUser])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
