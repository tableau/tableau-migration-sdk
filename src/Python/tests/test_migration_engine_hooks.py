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

from uuid import UUID
from typing import TypeVar

T = TypeVar("T")

from tableau_migration.migration_content import PyUser
from tableau_migration.migration_engine_actions import PyMigrationActionResult
from tableau_migration.migration_engine_hooks import PyMigrationHookBuilder
from tableau_migration.migration_engine_hooks_interop import PyContentBatchMigrationCompletedHookBase, PyMigrationActionCompletedHookBase
from tableau_migration.migration_engine_migrators_batch import PyContentBatchMigrationResult
from tableau_migration.migration_services import ScopedMigrationServices

from tests.helpers.autofixture import AutoFixtureTestBase

from System import IServiceProvider
from System.Threading import CancellationToken
from Tableau.Migration.Content import IUser
from Tableau.Migration.Engine.Actions import IMigrationActionResult
from Tableau.Migration.Engine.Hooks import (
    IContentBatchMigrationCompletedHook, IMigrationActionCompletedHook, 
    IMigrationHook, MigrationHookBuilder
)
from Tableau.Migration.Engine.Migrators.Batch import IContentBatchMigrationResult

class PyActionCompletedHook(PyMigrationActionCompletedHookBase):
        
    def execute(self, ctx: PyMigrationActionResult) -> PyMigrationActionResult:
        return ctx.for_next_action(False)

def action_completed(ctx: PyMigrationActionResult) -> PyMigrationActionResult:
    return ctx.for_next_action(False)

def action_completed_services(ctx: PyMigrationActionResult, services: ScopedMigrationServices) -> PyMigrationActionResult:
    return ctx.for_next_action(False)

class TestActionCompletedHookInterop(AutoFixtureTestBase):
    def test_interop_class(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())
        
        result = hook_builder.add(PyActionCompletedHook)
        assert result is hook_builder
        
        hook_factories = hook_builder.build().get_hooks(IMigrationActionCompletedHook)
        assert len(hook_factories) == 1

        services = self.create(IServiceProvider)
        ctx = self.create(IMigrationActionResult)
        
        hook = hook_factories[0].Create[IMigrationHook[IMigrationActionResult]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.PerformNextAction == False

    def test_interop_callback(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())

        ctx = self.create(IMigrationActionResult)

        result = hook_builder.add(PyMigrationActionResult, action_completed)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IMigrationActionCompletedHook)
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[IMigrationActionResult]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.PerformNextAction == False
    
    def test_interop_callback_services(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())

        ctx = self.create(IMigrationActionResult)

        result = hook_builder.add(PyMigrationActionResult, action_completed_services)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IMigrationActionCompletedHook)
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[IMigrationActionResult]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.PerformNextAction == False

class PyBatchCompletedHookBase(PyContentBatchMigrationCompletedHookBase[T]):
        
    def execute(self, ctx: PyContentBatchMigrationResult[T]) -> PyContentBatchMigrationResult[T]:
        return ctx.for_next_batch(False)

class PyUserBatchCompletedHook(PyBatchCompletedHookBase[PyUser]):
    pass

def batch_completed(ctx: PyContentBatchMigrationResult[PyUser]) -> PyContentBatchMigrationResult[PyUser]:
    return ctx.for_next_batch(False)

def batch_completed_services(ctx: PyContentBatchMigrationResult[PyUser], services: ScopedMigrationServices) -> PyContentBatchMigrationResult[PyUser]:
    return ctx.for_next_batch(False)

class TestContentBatchCompletedHookInterop(AutoFixtureTestBase):
    def test_interop_class(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())
        
        result = hook_builder.add(PyUserBatchCompletedHook)
        assert result is hook_builder
        
        hook_factories = hook_builder.build().get_hooks(IContentBatchMigrationCompletedHook[IUser])
        assert len(hook_factories) == 1

        services = self.create(IServiceProvider)
        ctx = self.create(IContentBatchMigrationResult[IUser])
        
        hook = hook_factories[0].Create[IMigrationHook[IContentBatchMigrationResult[IUser]]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.PerformNextBatch == False

    def test_interop_callback(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())

        ctx = self.create(IContentBatchMigrationResult[IUser])

        result = hook_builder.add(PyContentBatchMigrationResult[PyUser], batch_completed)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentBatchMigrationCompletedHook[IUser])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[IContentBatchMigrationResult[IUser]]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.PerformNextBatch == False
    
    def test_interop_callback_services(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())

        ctx = self.create(IContentBatchMigrationResult[IUser])

        result = hook_builder.add(PyContentBatchMigrationResult[PyUser], batch_completed_services)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentBatchMigrationCompletedHook[IUser])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[IContentBatchMigrationResult[IUser]]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.PerformNextBatch == False