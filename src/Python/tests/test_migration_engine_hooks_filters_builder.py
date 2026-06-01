# Copyright (c) 2025, Salesforce, Inc.
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

from typing import Optional, TypeVar
from uuid import UUID

from tableau_migration.migration_content import PyUser
from tableau_migration.migration_engine import PyContentMigrationItem
from tableau_migration.migration_engine_hooks_filters import PyContentFilterContext, PyContentFilterContextItem, PyFilterStatus
from tableau_migration.migration_engine_hooks_filters_builder import PyContentFilterBuilder
from tableau_migration.migration_engine_hooks_filters_interop import PyContentFilterBase
from tableau_migration.migration_services import ScopedMigrationServices

from tests.helpers.autofixture import AutoFixtureTestBase

from System import IServiceProvider
from System.Collections.Generic import IEnumerable, List
from System.Threading import CancellationToken
from Tableau.Migration.Content import IUser
from Tableau.Migration.Engine import ContentMigrationItem
from Tableau.Migration.Engine.Hooks import IMigrationHook
from Tableau.Migration.Engine.Hooks.Filters import ContentFilterBuilder, ContentFilterContext, ContentFilterContextItem, IContentFilter

T = TypeVar("T")

class PyFilter(PyContentFilterBase[T]):
    
    search_id: Optional[UUID]  = None

    def should_migrate(self, item: PyContentMigrationItem[T]) -> bool:
        return self.search_id is None or item.source_item.id != self.search_id

class PyUserFilter(PyFilter[PyUser]):
    pass  

def create_filter_users(search_id: UUID):
    def filter_users(item: PyContentMigrationItem[PyUser]) -> bool:
        return item.source_item.id != search_id

    return filter_users

def create_filter_users_services(search_id: UUID):
    def filter_users_service(item: PyContentMigrationItem[PyUser], services: ScopedMigrationServices) -> bool:
        return item.source_item.id != search_id

    return filter_users_service

class TestFilterInterop(AutoFixtureTestBase):
    def test_filter_interop_class(self):
        hook_builder = PyContentFilterBuilder(ContentFilterBuilder())
        
        result = hook_builder.add(PyUserFilter)
        assert result is hook_builder
        
        hook_factories = hook_builder.build().get_hooks(IContentFilter[IUser])
        assert len(hook_factories) == 1

        services = self.create(IServiceProvider)
        ctx = PyContentFilterContext[PyUser](self.create(ContentFilterContext[IUser]))

        PyUserFilter.search_id = ctx.items[0].source_item.id

        hook = hook_factories[0].Create[IMigrationHook[ContentFilterContext[IUser]]](services)
        hook_result = PyContentFilterContext[PyUser](hook.ExecuteAsync(ctx._dotnet, CancellationToken(False)).GetAwaiter().GetResult())

        assert len([x for x in hook_result.items if x.status != PyFilterStatus.MIGRATE]) == 1

    def test_filter_interop_callback(self):
        hook_builder = PyContentFilterBuilder(ContentFilterBuilder())

        ctx = PyContentFilterContext[PyUser](self.create(ContentFilterContext[IUser]))
        search_id = ctx.items[0].source_item.id

        result = hook_builder.add(PyUser, create_filter_users(search_id))
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentFilter[IUser])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[ContentFilterContext[IUser]]](services)
        hook_result = PyContentFilterContext[PyUser](hook.ExecuteAsync(ctx._dotnet, CancellationToken(False)).GetAwaiter().GetResult())

        assert len([x for x in hook_result.items if x.status != PyFilterStatus.MIGRATE]) == 1
    
    def test_filter_interop_callback_services(self):
        hook_builder = PyContentFilterBuilder(ContentFilterBuilder())

        ctx = PyContentFilterContext[PyUser](self.create(ContentFilterContext[IUser]))
        search_id = ctx.items[0].source_item.id

        result = hook_builder.add(PyUser, create_filter_users_services(search_id))
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentFilter[IUser])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[ContentFilterContext[IUser]]](services)
        hook_result = PyContentFilterContext[PyUser](hook.ExecuteAsync(ctx._dotnet, CancellationToken(False)).GetAwaiter().GetResult())

        assert len([x for x in hook_result.items if x.status != PyFilterStatus.MIGRATE]) == 1