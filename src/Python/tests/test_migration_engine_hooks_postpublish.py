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

from tableau_migration.migration_content import PyUser, PyPublishableWorkbook, PyWorkbookDetails
from tableau_migration.migration_engine_hooks import PyMigrationHookBuilder
from tableau_migration.migration_engine_hooks_postpublish import PyBulkPostPublishContext, PyContentItemPostPublishContext
from tableau_migration.migration_engine_hooks_postpublish_interop import PyBulkPostPublishHookBase, PyContentItemPostPublishHookBase
from tableau_migration.migration_services import ScopedMigrationServices

from tests.helpers.autofixture import AutoFixtureTestBase

from System import IServiceProvider
from System.Threading import CancellationToken
from Tableau.Migration.Content import IUser, IPublishableWorkbook, IWorkbookDetails
from Tableau.Migration.Engine.Hooks import IMigrationHook, MigrationHookBuilder
from Tableau.Migration.Engine.Hooks.PostPublish import (
    BulkPostPublishContext, BulkPostPublishHookBase, 
    ContentItemPostPublishContext, ContentItemPostPublishHookBase, 
    IBulkPostPublishHook, IContentItemPostPublishHook
)

T = TypeVar("T")
U = TypeVar("U")

class PyItemPostPublishHook(PyContentItemPostPublishHookBase[T, U]):
        
    def execute(self, ctx: PyContentItemPostPublishContext[T, U]) -> PyContentItemPostPublishContext[T, U]:
        return None

class PyWorkbookPostPublishHook(PyItemPostPublishHook[PyPublishableWorkbook, PyWorkbookDetails]):
    pass  

def workbook_post_publish(ctx: PyContentItemPostPublishContext[PyPublishableWorkbook, PyWorkbookDetails]) -> PyContentItemPostPublishContext[PyPublishableWorkbook, PyWorkbookDetails]:
    return None

def workbook_post_publish_no_return(ctx: PyContentItemPostPublishContext[PyPublishableWorkbook, PyWorkbookDetails]):
    pass

def workbook_post_publish_services(ctx: PyContentItemPostPublishContext[PyPublishableWorkbook, PyWorkbookDetails], services: ScopedMigrationServices) -> PyContentItemPostPublishContext[PyPublishableWorkbook, PyWorkbookDetails]:
    return None

class TestItemPostPublishInterop(AutoFixtureTestBase):
    def test_item_interop_class(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())
        
        result = hook_builder.add(PyWorkbookPostPublishHook)
        assert result is hook_builder
        
        hook_factories = hook_builder.build().get_hooks(IContentItemPostPublishHook[IPublishableWorkbook, IWorkbookDetails])
        assert len(hook_factories) == 1

        services = self.create(IServiceProvider)
        ctx = self.create(ContentItemPostPublishContext[IPublishableWorkbook, IWorkbookDetails])
        
        hook = hook_factories[0].Create[IMigrationHook[ContentItemPostPublishContext[IPublishableWorkbook, IWorkbookDetails]]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result is None

    def test_item_interop_callback(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())

        ctx = self.create(ContentItemPostPublishContext[IPublishableWorkbook, IWorkbookDetails])

        result = hook_builder.add(PyContentItemPostPublishContext[PyPublishableWorkbook, PyWorkbookDetails], workbook_post_publish)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentItemPostPublishHook[IPublishableWorkbook, IWorkbookDetails])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[ContentItemPostPublishContext[IPublishableWorkbook, IWorkbookDetails]]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result is None
    
    def test_item_interop_callback_services(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())

        ctx = self.create(ContentItemPostPublishContext[IPublishableWorkbook, IWorkbookDetails])

        result = hook_builder.add(PyContentItemPostPublishContext[PyPublishableWorkbook, PyWorkbookDetails], workbook_post_publish_services)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentItemPostPublishHook[IPublishableWorkbook, IWorkbookDetails])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[ContentItemPostPublishContext[IPublishableWorkbook, IWorkbookDetails]]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result is None

    def test_item_interop_no_return(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())

        ctx = self.create(ContentItemPostPublishContext[IPublishableWorkbook, IWorkbookDetails])

        result = hook_builder.add(PyContentItemPostPublishContext[PyPublishableWorkbook, PyWorkbookDetails], workbook_post_publish_no_return)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentItemPostPublishHook[IPublishableWorkbook, IWorkbookDetails])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[ContentItemPostPublishContext[IPublishableWorkbook, IWorkbookDetails]]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result is None
        
class PyBulkPostPublishHook(PyBulkPostPublishHookBase[T]):
        
    def execute(self, ctx: PyBulkPostPublishContext[T]) -> PyBulkPostPublishContext[T]:
        return None

class PyUserPostPublishHook(PyBulkPostPublishHook[PyUser]):
    pass  

def user_post_publish(ctx: PyBulkPostPublishContext[PyUser]) -> PyBulkPostPublishContext[PyUser]:
    return None

def user_post_publish_services(ctx: PyBulkPostPublishContext[PyUser], services: ScopedMigrationServices) -> PyBulkPostPublishContext[PyUser]:
    return None

class TestBulkPostPublishInterop(AutoFixtureTestBase):
    def test_bulk_interop_class(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())
        
        result = hook_builder.add(PyUserPostPublishHook)
        assert result is hook_builder
        
        hook_factories = hook_builder.build().get_hooks(IBulkPostPublishHook[IUser])
        assert len(hook_factories) == 1

        services = self.create(IServiceProvider)
        ctx = self.create(BulkPostPublishContext[IUser])
        
        hook = hook_factories[0].Create[IMigrationHook[BulkPostPublishContext[IUser]]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result is None

    def test_bulk_interop_callback(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())

        ctx = self.create(BulkPostPublishContext[IUser])

        result = hook_builder.add(PyBulkPostPublishContext[PyUser], user_post_publish)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IBulkPostPublishHook[IUser])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[BulkPostPublishContext[IUser]]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result is None
    
    def test_bulk_interop_callback_services(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())

        ctx = self.create(BulkPostPublishContext[IUser])

        result = hook_builder.add(PyBulkPostPublishContext[PyUser], user_post_publish_services)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IBulkPostPublishHook[IUser])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[BulkPostPublishContext[IUser]]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result is None

        