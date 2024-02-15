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
from tableau_migration.migration_engine_hooks_transformers import PyContentTransformerBuilder
from System import IServiceProvider, Func, ArgumentException, NotImplementedException
from System.Threading import CancellationToken
from Microsoft.Extensions.DependencyInjection import (
    ServiceCollection,
    ServiceCollectionContainerBuilderExtensions)
from Tableau.Migration.Content import IUser, IProject, IWorkbook
from Tableau.Migration.Engine.Hooks import IMigrationHook
from Tableau.Migration.Engine.Hooks.Transformers import ContentTransformerBuilder, IContentTransformer, ContentTransformerBase
from Tableau.Migration.Interop.Hooks.Transformers import ISyncContentTransformer, ISyncXmlContentTransformer
from Tableau.Migration.Tests import TestFileContentType as PyTestFileContentType # Needed as this class name starts with Test, which means pytest wants to pick it up

class ClassImplementation(ISyncContentTransformer[IUser]):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Transformers"
    
    def Execute(self, ctx: IUser) -> IUser:
        return ctx

class SubClassImplementation(ClassImplementation):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Transformers"
    
    def Execute(self, ctx: IUser) -> IUser:
        return ctx

class RawHook(IMigrationHook[float]):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Transformers"

class WithoutImplementation(ISyncContentTransformer[str]):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Transformers"

class WithImplementation(ISyncContentTransformer[str]):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Transformers"
    
    def Execute(self, ctx: str) -> str:
        return ctx
    
class TestXmlTransformer(ISyncXmlContentTransformer[PyTestFileContentType]):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Transformers"
    __test__ = False # Needed as this class name starts with Test, which means pytest wants to pick it up 

    def __init__(self):
        self.called = False

    def NeedsXmlTransforming(self, ctx: PyTestFileContentType) -> bool:
        return True
    
    def Execute(self, ctx: PyTestFileContentType, xml) -> None:
        self.called = True

class TestContentTransformerBuilderTests():
    def test_clear_class_hook(self):
        hook_builder = PyContentTransformerBuilder(ContentTransformerBuilder())
        
        result = hook_builder.add(IUser,ClassImplementation()).clear()
        
        assert result is hook_builder
        assert len(result.build().get_hooks(ISyncContentTransformer[IUser])) == 0
        
    def test_clear_subclass_hook(self):
        hook_builder = PyContentTransformerBuilder(ContentTransformerBuilder())
        
        result = hook_builder.add(IUser,SubClassImplementation()).clear()
        
        assert result is hook_builder
        assert len(result.build().get_hooks(ISyncContentTransformer[IUser])) == 0
        
    def test_add_object(self):
        hook_builder = PyContentTransformerBuilder(ContentTransformerBuilder())
        
        result = hook_builder.add(IUser,SubClassImplementation())
        
        assert result is hook_builder
        
    def test_add_type_noinitializer(self):
        hook_builder = PyContentTransformerBuilder(ContentTransformerBuilder())
        
        result = hook_builder.add(ClassImplementation,IUser)
        
        assert result is hook_builder
        
    def test_add_type_initializer(self):
        hook_builder = PyContentTransformerBuilder(ContentTransformerBuilder())
        
        def subclassinitialize(provider: IServiceProvider):
            return get_service(provider, SubClassImplementation)
        
        result = hook_builder.add(SubClassImplementation, IUser, Func[IServiceProvider, SubClassImplementation](subclassinitialize))
        
        assert result is hook_builder
        
    def test_add_callback(self):
        hook_builder = PyContentTransformerBuilder(ContentTransformerBuilder())
        
        def classcallback(context: IProject):
            return context
        
        result = hook_builder.add(IProject, Func[IProject, IProject](classcallback))
        
        assert result is hook_builder
        
    def test_add_withoutimplementation(self):
        value = "testvalue";
        provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
        hookFactory = PyContentTransformerBuilder(ContentTransformerBuilder()).add(str,WithoutImplementation()).build().get_hooks(IContentTransformer[str])
        assert len(hookFactory) == 1
        hook = hookFactory[0].Create[IMigrationHook[str]](provider)
        try:
            hook.ExecuteAsync(value, CancellationToken(False))
        except NotImplementedException as nie:
            assert nie.args[0] == "Python object does not have a 'Execute' method"
        else:
            assert False, "This test must generate a NotImplementedException."
        finally:
            provider.Dispose()
        
    def test_add_withimplementation(self):
        value = "testvalue";
        provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
        hookFactory = PyContentTransformerBuilder(ContentTransformerBuilder()).add(str,WithImplementation()).build().get_hooks(IContentTransformer[str])
        assert len(hookFactory) == 1
        hook = hookFactory[0].Create[IMigrationHook[str]](provider)
        try:
            result = hook.ExecuteAsync(value, CancellationToken(False))
        except:
            assert False, "This test must not generate an Exception."
        finally:
            provider.Dispose()
        
    def test_build_detects_interface_from_concrete_class(self):
        collection = PyContentTransformerBuilder(ContentTransformerBuilder()).add(IUser,ClassImplementation()).build()
        hooks = collection.get_hooks(IContentTransformer[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[IUser])
        otherhooks = collection.get_hooks(IContentTransformer[IProject])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)
        
        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        
    def test_build_detects_interface_from_concrete_subclass(self):
        collection = PyContentTransformerBuilder(ContentTransformerBuilder()).add(IUser,SubClassImplementation()).build()
        hooks = collection.get_hooks(IContentTransformer[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[IUser])
        otherhooks = collection.get_hooks(IContentTransformer[IProject])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        
    def test_build_factory_class(self):
        collection = PyContentTransformerBuilder(ContentTransformerBuilder()).add(ClassImplementation,IUser).build()
        hooks = collection.get_hooks(IContentTransformer[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[IUser])
        otherhooks = collection.get_hooks(IContentTransformer[IProject])
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
        
        collection = PyContentTransformerBuilder(ContentTransformerBuilder()).add(SubClassImplementation, IUser, Func[IServiceProvider, SubClassImplementation](subclassinitialize)).build()
        hooks = collection.get_hooks(IContentTransformer[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[IUser])
        otherhooks = collection.get_hooks(IContentTransformer[IProject])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        
    def test_build_factory_multipleclasses(self):
        collection = PyContentTransformerBuilder(ContentTransformerBuilder()).add(ClassImplementation,IUser).add(SubClassImplementation,IUser).build()
        hooks = collection.get_hooks(IContentTransformer[IUser])
        internalhooks = collection.get_hooks(IMigrationHook[IUser])
        otherhooks = collection.get_hooks(IContentTransformer[IProject])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 2
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        
    def test_build_callback(self):
        def classcallback(context: IProject):
            return context
        
        collection = PyContentTransformerBuilder(ContentTransformerBuilder()).add(IProject, Func[IProject, IProject](classcallback)).build()
        hooks = collection.get_hooks(IContentTransformer[IProject])
        internalhooks = collection.get_hooks(IMigrationHook[IProject])
        otherhooks = collection.get_hooks(IContentTransformer[IUser])
        classhooks = collection.get_hooks(ClassImplementation)
        subclasshooks = collection.get_hooks(SubClassImplementation)

        # Can't do a direct object comparison, so let's just count the right number is returned
        assert len(hooks) == 1
        assert len(internalhooks) == 0
        assert len(otherhooks) == 0
        assert len(classhooks) == 0
        assert len(subclasshooks) == 0
        
    def test_xml_execute(self):

        content = PyTestFileContentType()

        provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
        pyTransformer = TestXmlTransformer()
        hookFactory = PyContentTransformerBuilder(ContentTransformerBuilder()).add(PyTestFileContentType, pyTransformer).build().get_hooks(IContentTransformer[PyTestFileContentType])
        assert len(hookFactory) == 1
        hook = hookFactory[0].Create[IMigrationHook[PyTestFileContentType]](provider)
        try:
            result = hook.ExecuteAsync(content, CancellationToken(False)).GetAwaiter().GetResult()
        except Exception:
            assert False, "This test must not generate an Exception."
        finally:
            provider.Dispose()
        
        assert pyTransformer.called
