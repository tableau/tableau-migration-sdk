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

from typing import TypeVar
from uuid import UUID

from tableau_migration.migration import PyContentLocation
from tableau_migration.migration_engine import PyMigrationPlanBuilder
from tableau_migration.migration_content import PyUser
from tableau_migration.migration_engine_hooks_mappings import PyContentMappingBuilder, PyContentMappingContext
from tableau_migration.migration_engine_hooks_mappings_interop import PyContentMappingBase, PyTableauCloudUsernameMappingBase
from tableau_migration.migration_services import ScopedMigrationServices

from tests.helpers.autofixture import AutoFixtureTestBase

from System import IServiceProvider
from System.Threading import CancellationToken
from Tableau.Migration import ContentLocation
from Tableau.Migration.Content import IUser
from Tableau.Migration.Engine.Hooks import IMigrationHook
from Tableau.Migration.Engine.Hooks.Mappings import ContentMappingBuilder, ContentMappingContext, IContentMapping

T = TypeVar("T")

class PyMapping(PyContentMappingBase[T]):
        
    def map(self, ctx: PyContentMappingContext[T]) -> PyContentMappingContext[T]:
        return ctx.map_to(ctx.mapped_location.rename(ctx.mapped_location.name + "2"))

class PyUserMapping(PyMapping[PyUser]):
    pass  

def map_users(ctx: PyContentMappingContext[PyUser]) -> PyContentMappingContext[PyUser]:
    return ctx.map_to(ctx.mapped_location.rename(ctx.mapped_location.name + "3"))

def map_users_services(ctx: PyContentMappingContext[PyUser], services: ScopedMigrationServices) -> PyContentMappingContext[PyUser]:
    return ctx.map_to(ctx.mapped_location.rename(ctx.mapped_location.name + "4"))

class TestMappingInterop(AutoFixtureTestBase):
    def test_mapping_interop_class(self):
        hook_builder = PyContentMappingBuilder(ContentMappingBuilder())
        
        result = hook_builder.add(PyUserMapping)
        assert result is hook_builder
        
        hook_factories = hook_builder.build().get_hooks(IContentMapping[IUser])
        assert len(hook_factories) == 1

        services = self.create(IServiceProvider)
        map_ctx = self.create(ContentMappingContext[IUser])
        
        hook = hook_factories[0].Create[IMigrationHook[ContentMappingContext[IUser]]](services)
        hook_result = hook.ExecuteAsync(map_ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.MappedLocation.Name == map_ctx.MappedLocation.Name + "2"

    def test_mapping_interop_callback(self):
        hook_builder = PyContentMappingBuilder(ContentMappingBuilder())

        map_ctx = self.create(ContentMappingContext[IUser])

        result = hook_builder.add(PyUser, map_users)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentMapping[IUser])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[ContentMappingContext[IUser]]](services)
        hook_result = hook.ExecuteAsync(map_ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.MappedLocation.Name == map_ctx.MappedLocation.Name + "3"
    
    def test_mapping_interop_callback_services(self):
        hook_builder = PyContentMappingBuilder(ContentMappingBuilder())

        map_ctx = self.create(ContentMappingContext[IUser])

        result = hook_builder.add(PyUser, map_users_services)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentMapping[IUser])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[ContentMappingContext[IUser]]](services)
        hook_result = hook.ExecuteAsync(map_ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.MappedLocation.Name == map_ctx.MappedLocation.Name + "4"        
        
class TestContentMigrationItem(AutoFixtureTestBase):
    def test_wrapper_init(self):
        dotnet_content_item = self.create(IUser)
        loc = ContentLocation.ForUsername("domain", "username")
        dotnet_ctx = ContentMappingContext[IUser](dotnet_content_item, loc)
        wrapper = PyContentMappingContext[PyUser](dotnet_ctx)

    def test_content_item(self):
        user = self.create(IUser)
        loc = ContentLocation.ForUsername("domain", "username")
        dotnet_ctx = ContentMappingContext[IUser](user, loc)
        wrapper = PyContentMappingContext[PyUser](dotnet_ctx)
        
        assert wrapper.content_item._dotnet == user

    def test_map_to(self):
        user = self.create(IUser)
        loc = ContentLocation.ForUsername("domain", "username")
        dotnet_ctx = ContentMappingContext[IUser](user, loc)
        wrapper = PyContentMappingContext[PyUser](dotnet_ctx)
        
        loc2 = PyContentLocation(ContentLocation.ForUsername("domain", "username2"))
        wrapper2 = wrapper.map_to(loc2)
        
        assert wrapper2.content_item._dotnet == wrapper.content_item._dotnet

        assert wrapper2.mapped_location.name == "username2"
        
class PyUsernameMapping(PyTableauCloudUsernameMappingBase):
    def map(self, ctx: PyContentMappingContext[PyUser]) -> PyContentMappingContext[PyUser]:
        return ctx.map_to(ctx.mapped_location.rename(ctx.mapped_location.name + "5"))
    
def map_usernames(ctx: PyContentMappingContext[PyUser]) -> PyContentMappingContext[PyUser]:
    return ctx.map_to(ctx.mapped_location.rename(ctx.mapped_location.name + "5"))

def map_usernames_services(ctx: PyContentMappingContext[PyUser], services: ScopedMigrationServices) -> PyContentMappingContext[PyUser]:
    return ctx.map_to(ctx.mapped_location.rename(ctx.mapped_location.name + "5"))

class TestTableauCloudUsernameMappingInterop(AutoFixtureTestBase):
    def test_mapping_interop_class(self):
        plan_builder = PyMigrationPlanBuilder().for_server_to_cloud()

        result = plan_builder.with_tableau_cloud_usernames(PyUsernameMapping)
        assert result is plan_builder
        
        hook_factories =  plan_builder.build().mappings.get_hooks(IContentMapping[IUser])
        assert len(hook_factories) == 1

        services = self.create(IServiceProvider)
        map_ctx = self.create(ContentMappingContext[IUser])
        
        hook = hook_factories[0].Create[IMigrationHook[ContentMappingContext[IUser]]](services)
        hook_result = hook.ExecuteAsync(map_ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.MappedLocation.Name == map_ctx.MappedLocation.Name + "5"

    def test_mapping_interop_callback(self):
        plan_builder = PyMigrationPlanBuilder().for_server_to_cloud()

        map_ctx = self.create(ContentMappingContext[IUser])

        result = plan_builder.with_tableau_cloud_usernames(map_usernames)
        assert result is plan_builder

        hook_factories = plan_builder.build().mappings.get_hooks(IContentMapping[IUser])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[ContentMappingContext[IUser]]](services)
        hook_result = hook.ExecuteAsync(map_ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.MappedLocation.Name == map_ctx.MappedLocation.Name + "5"
    
    def test_mapping_interop_callback_services(self):
        plan_builder = PyMigrationPlanBuilder().for_server_to_cloud()

        map_ctx = self.create(ContentMappingContext[IUser])

        result = plan_builder.with_tableau_cloud_usernames(map_usernames_services)
        assert result is plan_builder

        hook_factories = plan_builder.build().mappings.get_hooks(IContentMapping[IUser])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[ContentMappingContext[IUser]]](services)
        hook_result = hook.ExecuteAsync(map_ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.MappedLocation.Name == map_ctx.MappedLocation.Name + "5"    