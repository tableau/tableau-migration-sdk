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

# region _generated

from tableau_migration.migration import _generic_wrapper # noqa: E402, F401
from tableau_migration.migration_engine_hooks_filters import PyFilterStatus # noqa: E402, F401
from tableau_migration.migration_engine_manifest import PyMigrationManifestEntryEditor # noqa: E402, F401
from typing import (  # noqa: E402, F401
    Generic,
    TypeVar
)

from Tableau.Migration.Engine.Hooks.Filters import FilterStatus # noqa: E402, F401
from Tableau.Migration.Engine.Hooks.Pulled import ContentItemPulledContext # noqa: E402, F401

from tableau_migration.migration_engine_hooks_pulled import PyContentItemPulledContext # noqa: E402, F401
# Extra imports for tests.
from Tableau.Migration import IContentReference # noqa: E402, F401
from tableau_migration.migration import PyContentReference # noqa: E402, F401
from tests.helpers.autofixture import AutoFixtureTestBase # noqa: E402, F401

class TestPyContentItemPulledContextGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(ContentItemPulledContext[IContentReference])
        py = PyContentItemPulledContext[PyContentReference](dotnet)
        assert py._dotnet == dotnet
    
    def test_manifest_entry_getter(self):
        dotnet = self.create(ContentItemPulledContext[IContentReference])
        py = PyContentItemPulledContext[PyContentReference](dotnet)
        assert py.manifest_entry == None if dotnet.ManifestEntry is None else PyMigrationManifestEntryEditor(dotnet.ManifestEntry)
    
    def test_pulled_item_getter(self):
        dotnet = self.create(ContentItemPulledContext[IContentReference])
        py = PyContentItemPulledContext[PyContentReference](dotnet)
        assert py.pulled_item == None if dotnet.PulledItem is None else _generic_wrapper(dotnet.PulledItem)
    
    def test_status_getter(self):
        dotnet = self.create(ContentItemPulledContext[IContentReference])
        py = PyContentItemPulledContext[PyContentReference](dotnet)
        assert py.status.value == (None if dotnet.Status is None else PyFilterStatus(dotnet.Status.value__)).value
    
    def test_status_setter(self):
        dotnet = self.create(ContentItemPulledContext[IContentReference])
        py = PyContentItemPulledContext[PyContentReference](dotnet)
        
        # create test data
        testValue = self.create(FilterStatus)
        
        # set property to new test value
        py.status = None if testValue is None else PyFilterStatus(testValue.value__)
        
        # assert value
        assert py.status == None if testValue is None else PyFilterStatus(testValue.value__)
    

# endregion

from typing import Optional, TypeVar # noqa: E402, F401

from tableau_migration.migration_content import PyPublishableWorkbook # noqa: E402, F401
from tableau_migration.migration_engine_hooks_builder import PyMigrationHookBuilder # noqa: E402, F401
from tableau_migration.migration_engine_hooks_pulled_interop import PyContentItemPulledHookBase # noqa: E402, F401
from tableau_migration.migration_services import ScopedMigrationServices # noqa: E402, F401

from System import IServiceProvider # noqa: E402, F401
from System.Threading import CancellationToken # noqa: E402, F401
from Tableau.Migration.Content import IPublishableWorkbook # noqa: E402, F401
from Tableau.Migration.Engine.Hooks import IMigrationHook, MigrationHookBuilder # noqa: E402, F401
from Tableau.Migration.Engine.Hooks.Pulled import ContentItemPulledContext, IContentItemPulledHook # noqa: E402, F401

T = TypeVar("T")

class PyItemPulledHook(PyContentItemPulledHookBase[T]):
        
    def execute(self, ctx: PyContentItemPulledContext[T]) -> Optional[PyContentItemPulledContext[T]]:
        return None

class PyWorkbookPulledHook(PyItemPulledHook[PyPublishableWorkbook]):
    pass

def workbook_pulled(ctx: PyContentItemPulledContext[PyPublishableWorkbook]) -> Optional[PyContentItemPulledContext[PyPublishableWorkbook]]:
    return None

def workbook_pulled_services(ctx: PyContentItemPulledContext[PyPublishableWorkbook], services: ScopedMigrationServices) -> Optional[PyContentItemPulledContext[PyPublishableWorkbook]]:
    return None

def workbook_pulled_no_return(ctx: PyContentItemPulledContext[PyPublishableWorkbook]) -> Optional[PyContentItemPulledContext[PyPublishableWorkbook]]:
    return None

class TestPulledHookInterop(AutoFixtureTestBase):
    def test_interop_class(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())
        
        result = hook_builder.add(PyWorkbookPulledHook)
        assert result is hook_builder
        
        hook_factories = hook_builder.build().get_hooks(IContentItemPulledHook[IPublishableWorkbook])
        assert len(hook_factories) == 1

        services = self.create(IServiceProvider)
        ctx = self.create(ContentItemPulledContext[IPublishableWorkbook])
        
        hook = hook_factories[0].Create[IMigrationHook[ContentItemPulledContext[IPublishableWorkbook]]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result is None

    def test_interop_callback(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())

        ctx = self.create(ContentItemPulledContext[IPublishableWorkbook])

        result = hook_builder.add(PyContentItemPulledContext[PyPublishableWorkbook], workbook_pulled)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentItemPulledHook[IPublishableWorkbook])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[ContentItemPulledContext[IPublishableWorkbook]]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result is None
    
    def test_interop_callback_services(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())

        ctx = self.create(ContentItemPulledContext[IPublishableWorkbook])

        result = hook_builder.add(PyContentItemPulledContext[PyPublishableWorkbook], workbook_pulled_services)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentItemPulledHook[IPublishableWorkbook])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[ContentItemPulledContext[IPublishableWorkbook]]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result is None

    def test_interop_no_return(self):
        hook_builder = PyMigrationHookBuilder(MigrationHookBuilder())

        ctx = self.create(ContentItemPulledContext[IPublishableWorkbook])

        result = hook_builder.add(PyContentItemPulledContext[PyPublishableWorkbook], workbook_pulled_no_return)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentItemPulledHook[IPublishableWorkbook])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[ContentItemPulledContext[IPublishableWorkbook]]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result is None
